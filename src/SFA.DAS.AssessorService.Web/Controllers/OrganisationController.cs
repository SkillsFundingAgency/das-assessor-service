using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Azure;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Api.Client.Azure;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels;
using SFA.DAS.AssessorService.Web.ViewModels.Account;
using SFA.DAS.AssessorService.Web.ViewModels.Organisation;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    public class OrganisationController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IAzureApiClient _externalApiClient;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IEmailApiClient _emailApiClient;
        private readonly IValidationApiClient _validationApiClient;
        private readonly ILogger<OrganisationController> _logger;
        private readonly IWebConfiguration _webConfiguration;

        public OrganisationController(IHttpContextAccessor contextAccessor, 
            IOrganisationsApiClient organisationsApiClient, IAzureApiClient externalApiClient, IContactsApiClient contactsApiClient,
            IEmailApiClient emailApiClient, IValidationApiClient validationApiClient, ILogger<OrganisationController> logger, 
            IWebConfiguration webConfiguration)
        {
            _contextAccessor = contextAccessor;
            _organisationsApiClient = organisationsApiClient;
            _externalApiClient = externalApiClient;
            _contactsApiClient = contactsApiClient;
            _emailApiClient = emailApiClient;
            _validationApiClient = validationApiClient;
            _logger = logger;
            _webConfiguration = webConfiguration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            OrganisationResponse organisation;

            try
            {
                var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
                if (ukprn != null)
                {
                    organisation = await _organisationsApiClient.Get(ukprn);
                }
                else
                {
                    var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
                    var epaOrganisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);

                    organisation = new OrganisationResponse
                    {
                        EndPointAssessorName = epaOrganisation.Name,
                        EndPointAssessorOrganisationId = epaOrganisation.OrganisationId,
                        EndPointAssessorUkprn = (int?)epaOrganisation.Ukprn,
                        Id = epaOrganisation.Id,
                        PrimaryContact = epaOrganisation.PrimaryContact,
                        Status = epaOrganisation.Status
                    };
                }
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction(nameof(HomeController.NotRegistered), nameof(HomeController).RemoveController());
            }

            return View(organisation);
        }

        [HttpGet]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        public async Task<IActionResult> OrganisationDetails()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;

            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                var viewModel = MapOrganisationModel(organisation);
                viewModel.ExternalApiSubscriptions = await GetExternalApiSubscriptions(_webConfiguration.AzureApiAuthentication.ProductId, ukprn);

                var userId = _contextAccessor.HttpContext.User.FindFirst("UserId").Value;
                var userPrivileges = await _contactsApiClient.GetContactPrivileges(Guid.Parse(userId));
                
                viewModel.UserHasChangeOrganisationPrivilege =
                    userPrivileges.Any(cp => cp.Privilege.Key == Privileges.ChangeOrganisationDetails);

                if(viewModel.UserHasChangeOrganisationPrivilege == false)
                {
                    var changeOrganisationPriviledge = (await _contactsApiClient.GetPrivileges()).First(p => p.Key == Privileges.ChangeOrganisationDetails);
                    viewModel.AccessDeniedViewModel = new AccessDeniedViewModel
                    {
                        PrivilegeId = changeOrganisationPriviledge.Id,
                        ContactId = Guid.Parse(userId),
                        UserHasUserManagement = userPrivileges.Any(up => up.Privilege.Key == Privileges.ManageUsers),
                        ReturnController = nameof(OrganisationController).RemoveController(),
                        ReturnAction = nameof(OrganisationDetails)
                    };
                }

                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction(nameof(HomeController.NotRegistered), nameof(HomeController).RemoveController());
            }
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public async Task<IActionResult> OrganisationDetails(ViewAndEditOrganisationViewModel vm)
        {
            var ukprn = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.portal.com/ukprn")?.Value;

            if (vm.ActionChoice == "Enable")
            {
                await _externalApiClient.CreateUser(ukprn);
                return RedirectToAction(nameof(OrganisationDetails), nameof(OrganisationController).RemoveController(), "api-subscription");
            }

            return RedirectToAction(nameof(OrganisationDetails), nameof(OrganisationController).RemoveController(), "register-details");
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public async Task<IActionResult> RenewApiKey(Guid subscriptionId)
        {
            var ukprn = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.portal.com/ukprn")?.Value;

            try
            {
                var externalApiSubscriptions = await GetExternalApiSubscriptions(_webConfiguration.AzureApiAuthentication.ProductId, ukprn);
                var subscription = externalApiSubscriptions?.Where(p => p.Id == subscriptionId.ToString()).FirstOrDefault();

                if (subscription == null)
                    throw new Exception($"The subscription {subscriptionId} is invalid or does not belong to organsiation identified by {ukprn}");

                var viewModel = new RenewApiKeyViewModel
                {
                    SubscriptionId = subscription.Id,
                    CurrentKey = subscription.PrimaryKey,
                    LastRenewedDate = subscription.CreatedDate,
                    LastRenewedTicks = subscription.CreatedDate.Ticks
                };

                return View(viewModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to renew API key");
            }

            return RedirectToAction(nameof(OrganisationDetails), nameof(OrganisationController).RemoveController(), "api-subscription");
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public async Task<IActionResult> RenewApiKey(RenewApiKeyViewModel vm)
        {
            var ukprn = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.portal.com/ukprn")?.Value;
            try
            {
                var externalApiSubscriptions = await GetExternalApiSubscriptions(_webConfiguration.AzureApiAuthentication.ProductId, ukprn);
                var subscription = externalApiSubscriptions?.FirstOrDefault(p => p.Id == vm.SubscriptionId.ToString());

                if (subscription == null || !subscription.CreatedDate.Ticks.Equals(vm.LastRenewedTicks) || !subscription.PrimaryKey.Equals(vm.CurrentKey))
                {
                    TempData.SetAlert(new Alert { Message = "Your API key could not be renewed, please check the current value and retry if necessary.", Type = AlertType.Warning });
                    return RedirectToAction(nameof(RenewApiKey), nameof(OrganisationController).RemoveController(), "api-subscription");
                }

                // delete and re-subscribe so that the created date can be used to track a 'renewed' key
                if (await _externalApiClient.DeleteSubscriptionAndResubscribe(ukprn, subscription.Id))
                {
                    TempData.SetAlert(new Alert { Message = "Your API key has been renewed", Type = AlertType.Success });
                }
                else
                {
                    TempData.SetAlert(new Alert { Message = "Your API key could not be renewed, please check the current value and retry if necessary.", Type = AlertType.Warning });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to renew API key");
            }

            return RedirectToAction(nameof(OrganisationDetails), nameof(OrganisationController).RemoveController(), "api-subscription");
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public async Task<IActionResult> SelectOrChangeContactName()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                var contacts = await _contactsApiClient.GetAllContactsWhoCanBePrimaryForOrganisation(organisation.OrganisationId);

                var viewModel = new SelectOrChangeContactNameViewModel
                {
                    Contacts = contacts,
                    PrimaryContact = ModelState.IsValid
                        ? organisation.PrimaryContact
                        : ModelState[nameof(SelectOrChangeContactNameViewModel.PrimaryContact)]?.AttemptedValue
                };
           
                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction(nameof(HomeController.NotRegistered), nameof(HomeController).RemoveController());
            }
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public async Task<IActionResult> SelectOrChangeContactName(SelectOrChangeContactNameViewModel vm)
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                var primaryContact = !string.IsNullOrEmpty(vm.PrimaryContact)
                    ? await _contactsApiClient.GetByUsername(vm.PrimaryContact)
                    : null;

                if (vm.ActionChoice == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        if (string.Equals(vm.PrimaryContact, organisation.PrimaryContact))
                        {
                            return RedirectToAction(nameof(OrganisationDetails));
                        }

                        if (organisation.Id != primaryContact?.OrganisationId)
                        {
                            ModelState.AddModelError(nameof(SelectOrChangeContactNameViewModel.PrimaryContact), "The contact name cannot be changed to a contact of a different organisation");
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        return RedirectToAction(nameof(SelectOrChangeContactName));
                    }                   

                    vm = new SelectOrChangeContactNameViewModel
                    {
                        Contacts = null,
                        PrimaryContact = vm.PrimaryContact,
                        PrimaryContactName = primaryContact.DisplayName
                    };

                    return View("SelectOrChangeContactNameConfirm", vm);
                }
                else if (vm.ActionChoice == "Confirm")
                {
                    var userId = _contextAccessor.HttpContext.User.FindFirst("UserId").Value;
                    var request = new UpdateEpaOrganisationPrimaryContactRequest
                    {
                        PrimaryContactId = primaryContact.Id,
                        OrganisationId = organisation.OrganisationId,
                        UpdatedBy = Guid.Parse(userId)
                    };

                    var notifiedContacts = await _organisationsApiClient.UpdateEpaOrganisationPrimaryContact(request);
                    if (notifiedContacts != null)
                    {
                        vm = new SelectOrChangeContactNameViewModel
                        {
                            Contacts = notifiedContacts,
                            PrimaryContact = vm.PrimaryContact,
                            PrimaryContactName = primaryContact.DisplayName
                        };

                        return View("SelectOrChangeContactNameUpdated", vm);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(SelectOrChangeContactNameViewModel.PrimaryContact), "Unable to update the contact name at this time.");
                        return RedirectToAction(nameof(SelectOrChangeContactName));
                    }
                }
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction(nameof(HomeController.NotRegistered), nameof(HomeController).RemoveController());
            }

            return RedirectToAction(nameof(OrganisationDetails));
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public async Task<IActionResult> ChangePhoneNumber()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);

                var viewModel = new ChangePhoneNumberViewModel
                {
                    PhoneNumber = ModelState.IsValid
                        ? organisation.OrganisationData?.PhoneNumber
                        : ModelState[nameof(ChangePhoneNumberViewModel.PhoneNumber)]?.AttemptedValue
                };

                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction(nameof(HomeController.NotRegistered), nameof(HomeController).RemoveController());
            }
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public async Task<IActionResult> ChangePhoneNumber(ChangePhoneNumberViewModel vm)
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await GetEpaOrganisation(epaoid);
                if (organisation == null)
                {
                    return RedirectToAction(nameof(HomeController.NotRegistered), nameof(HomeController).RemoveController());
                }

                if (vm.ActionChoice == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        if (string.Equals(vm.PhoneNumber, organisation.OrganisationData?.PhoneNumber))
                        {
                            return RedirectToAction(nameof(OrganisationDetails));
                        }

                        // only check if a phone number has been entered - model has required validator
                        if (await _validationApiClient.ValidatePhoneNumber(vm.PhoneNumber) == false)
                        {
                            ModelState.AddModelError(nameof(ChangePhoneNumberViewModel.PhoneNumber), "Enter a valid phone number");
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        return RedirectToAction(nameof(ChangePhoneNumber));
                    }

                    vm = new ChangePhoneNumberViewModel
                    {
                        PhoneNumber = vm.PhoneNumber
                    };

                    return View("ChangePhoneNumberConfirm", vm);
                }
                else if (vm.ActionChoice == "Confirm")
                {
                    var userId = _contextAccessor.HttpContext.User.FindFirst("UserId").Value;
                    var request = new UpdateEpaOrganisationPhoneNumberRequest
                    {
                        PhoneNumber = vm.PhoneNumber,
                        OrganisationId = organisation.OrganisationId,
                        UpdatedBy = Guid.Parse(userId)
                    };

                    var notifiedContacts = await _organisationsApiClient.UpdateEpaOrganisationPhoneNumber(request);
                    if (notifiedContacts == null)
                    {
                        throw new Exception("Unable to update phone number");
                    }

                    vm = new ChangePhoneNumberViewModel
                    {
                        PhoneNumber = vm.PhoneNumber,
                        Contacts = notifiedContacts
                    };

                    return View("ChangePhoneNumberUpdated", vm);
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to update phone number");
                ModelState.AddModelError(nameof(ChangePhoneNumberViewModel.PhoneNumber), "Unable to update the contact phone number at this time.");
                return RedirectToAction(nameof(ChangePhoneNumber));
            }

            return RedirectToAction(nameof(OrganisationDetails));
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public async Task<IActionResult> ChangeAddress()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);

                var viewModel = new ChangeAddressViewModel
                {
                    AddressLine1 = ModelState.IsValid
                        ? organisation.OrganisationData?.Address1
                        : ModelState[nameof(ChangeAddressViewModel.AddressLine1)]?.AttemptedValue,
                    AddressLine2 = ModelState.IsValid
                        ? organisation.OrganisationData?.Address2
                        : ModelState[nameof(ChangeAddressViewModel.AddressLine2)]?.AttemptedValue,
                    AddressLine3 = ModelState.IsValid
                        ? organisation.OrganisationData?.Address3
                        : ModelState[nameof(ChangeAddressViewModel.AddressLine3)]?.AttemptedValue,
                    AddressLine4 = ModelState.IsValid
                        ? organisation.OrganisationData?.Address4
                        : ModelState[nameof(ChangeAddressViewModel.AddressLine4)]?.AttemptedValue,
                    Postcode = ModelState.IsValid
                        ? organisation.OrganisationData?.Postcode
                        : ModelState[nameof(ChangeAddressViewModel.Postcode)]?.AttemptedValue
                };

                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction(nameof(HomeController.NotRegistered), nameof(HomeController).RemoveController());
            }
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public async Task<IActionResult> ChangeAddress(ChangeAddressViewModel vm)
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await GetEpaOrganisation(epaoid);
                if (organisation == null)
                {
                    return RedirectToAction(nameof(HomeController.NotRegistered), nameof(HomeController).RemoveController());
                }

                if (vm.ActionChoice == "Save")
                {
                    // regardless of whether the model is invalid, when the values have not been changed do not update
                    if (string.Equals(vm.AddressLine1, organisation.OrganisationData?.Address1) &&
                        string.Equals(vm.AddressLine2, organisation.OrganisationData?.Address2) &&
                        string.Equals(vm.AddressLine3, organisation.OrganisationData?.Address3) &&
                        string.Equals(vm.AddressLine4, organisation.OrganisationData?.Address4) &&
                        string.Equals(vm.Postcode, organisation.OrganisationData?.Postcode))
                    {
                        ModelState.Clear();
                        return RedirectToAction(nameof(OrganisationDetails));
                    }

                    if (!ModelState.IsValid)
                    {
                        return RedirectToAction(nameof(ChangeAddress));
                    }

                    vm = new ChangeAddressViewModel
                    {
                        AddressLine1 = vm.AddressLine1,
                        AddressLine2 = vm.AddressLine2,
                        AddressLine3 = vm.AddressLine3,
                        AddressLine4 = vm.AddressLine4,
                        Postcode = vm.Postcode
                    };

                    return View("ChangeAddressConfirm", vm);
                }
                else if (vm.ActionChoice == "Confirm")
                {
                    var userId = _contextAccessor.HttpContext.User.FindFirst("UserId").Value;
                    var request = new UpdateEpaOrganisationAddressRequest
                    {
                        AddressLine1 = vm.AddressLine1,
                        AddressLine2 = vm.AddressLine2,
                        AddressLine3 = vm.AddressLine3,
                        AddressLine4 = vm.AddressLine4,
                        Postcode = vm.Postcode,
                        OrganisationId = organisation.OrganisationId,
                        UpdatedBy = Guid.Parse(userId)
                    };

                    var notifiedContacts = await _organisationsApiClient.UpdateEpaOrganisationAddress(request);
                    if (notifiedContacts == null)
                    {
                        throw new Exception("Unable to update the address");
                    }

                    vm = new ChangeAddressViewModel
                    {
                        AddressLine1 = vm.AddressLine1,
                        AddressLine2 = vm.AddressLine2,
                        AddressLine3 = vm.AddressLine3,
                        AddressLine4 = vm.AddressLine4,
                        Postcode = vm.Postcode,
                        Contacts = notifiedContacts
                    };

                    return View("ChangeAddressUpdated", vm);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to change address");
                ModelState.AddModelError(nameof(ChangeAddressViewModel.AddressLine1), "Unable to update the address at this time.");
                return RedirectToAction(nameof(ChangeAddress));
            }

            return RedirectToAction(nameof(OrganisationDetails));
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public async Task<IActionResult> ChangeEmail()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);

                var viewModel = new ChangeEmailViewModel
                {
                    Email = ModelState.IsValid
                        ? organisation.OrganisationData?.Email
                        : ModelState[nameof(ChangeEmailViewModel.Email)]?.AttemptedValue
                };

                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction(nameof(HomeController.NotRegistered), nameof(HomeController).RemoveController());
            }
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel vm)
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await GetEpaOrganisation(epaoid);
                if (organisation == null)
                {
                    return RedirectToAction(nameof(HomeController.NotRegistered), nameof(HomeController).RemoveController());
                }

                if (vm.ActionChoice == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        if (string.Equals(vm.Email, organisation.OrganisationData?.Email))
                        {
                            return RedirectToAction(nameof(OrganisationDetails));
                        }

                        // only check if an email address has been entered - model has required validator
                        if (await _validationApiClient.ValidateEmailAddress(vm.Email) == false)
                        {
                            ModelState.AddModelError(nameof(ChangeEmailViewModel.Email), "Enter a valid email address");
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        return RedirectToAction(nameof(ChangeEmail));
                    }

                    vm = new ChangeEmailViewModel
                    {
                        Email = vm.Email
                    };

                    return View("ChangeEmailConfirm", vm);
                }
                else if (vm.ActionChoice == "Confirm")
                {
                    var userId = _contextAccessor.HttpContext.User.FindFirst("UserId").Value;
                    var request = new UpdateEpaOrganisationEmailRequest
                    {
                        Email = vm.Email,
                        OrganisationId = organisation.OrganisationId,
                        UpdatedBy = Guid.Parse(userId)
                    };

                    var notifiedContacts = await _organisationsApiClient.UpdateEpaOrganisationEmail(request);
                    if (notifiedContacts == null)
                    {
                        throw new Exception("Unable to update the email address");
                    }

                    vm = new ChangeEmailViewModel
                    {
                        Email = vm.Email,
                        Contacts = notifiedContacts
                    };

                    return View("ChangeEmailUpdated", vm);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to change email address");
                ModelState.AddModelError(nameof(ChangeEmailViewModel.Email), "Unable to update the email address at this time.");
                return RedirectToAction(nameof(ChangeEmail));
            }

            return RedirectToAction(nameof(OrganisationDetails));
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public async Task<IActionResult> ChangeWebsite()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);

                var viewModel = new ChangeWebsiteViewModel
                {
                    WebsiteLink = ModelState.IsValid
                        ? organisation.OrganisationData?.WebsiteLink
                        : ModelState[nameof(ChangeWebsiteViewModel.WebsiteLink)]?.AttemptedValue
                };

                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction(nameof(HomeController.NotRegistered), nameof(HomeController).RemoveController());
            }
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public async Task<IActionResult> ChangeWebsite(ChangeWebsiteViewModel vm)
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await GetEpaOrganisation(epaoid);
                if (organisation == null)
                {
                    return RedirectToAction(nameof(HomeController.NotRegistered), nameof(HomeController).RemoveController());
                }

                if (vm.ActionChoice == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        if (string.Equals(vm.WebsiteLink, organisation.OrganisationData?.WebsiteLink))
                        {
                            return RedirectToAction(nameof(OrganisationDetails));
                        }

                        // only check if an web site link has been entered - model has required validator
                        
                        var encodedWebsiteUrl = HttpUtility.UrlEncode(vm.WebsiteLink);
                        _logger.LogInformation($"VALIDATEWEBSITELINK - OrganisationController.ChangeWebsite: {vm.WebsiteLink}, {encodedWebsiteUrl}");
                        if (await _validationApiClient.ValidateWebsiteLink(encodedWebsiteUrl) == false)
                        {
                            ModelState.AddModelError(nameof(ChangeWebsiteViewModel.WebsiteLink), "Enter a valid website address");
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        return RedirectToAction(nameof(ChangeWebsite));
                    }

                    vm = new ChangeWebsiteViewModel
                    {
                        WebsiteLink = vm.WebsiteLink
                    };

                    return View("ChangeWebsiteConfirm", vm);
                }
                else if (vm.ActionChoice == "Confirm")
                {
                    var userId = _contextAccessor.HttpContext.User.FindFirst("UserId").Value;
                    var request = new UpdateEpaOrganisationWebsiteLinkRequest
                    {
                        WebsiteLink = vm.WebsiteLink,
                        OrganisationId = organisation.OrganisationId,
                        UpdatedBy = Guid.Parse(userId)
                    };

                    var notifiedContacts = await _organisationsApiClient.UpdateEpaOrganisationWebsiteLink(request);
                    if (notifiedContacts == null)
                    {
                        throw new Exception("Unable to update the website address.");
                    }

                    vm = new ChangeWebsiteViewModel
                    {
                        WebsiteLink = vm.WebsiteLink,
                        Contacts = notifiedContacts
                    };

                    return View("ChangeWebsiteUpdated", vm);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to change website link");
                ModelState.AddModelError(nameof(ChangeWebsiteViewModel.WebsiteLink), "Unable to update the website address at this time.");
                return RedirectToAction(nameof(ChangeWebsite));
            }

            return RedirectToAction(nameof(OrganisationDetails));
        }

        [HttpGet]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationDetails)]
        public IActionResult ApiSubscriptionRequestAccess()
        {
            return RedirectToAction(nameof(OrganisationDetails), nameof(OrganisationController).RemoveController(), "api-subscription");
        }

        private async Task<List<AzureSubscription>> GetExternalApiSubscriptions(string productId, string ukprn)
        {
          var users = await _externalApiClient.GetUserDetailsByUkprn(ukprn, true);
          return users.SelectMany(u => u.Subscriptions.Where(s => s.IsActive && s.ProductId == productId)).ToList();
        }

        private ViewAndEditOrganisationViewModel MapOrganisationModel(EpaOrganisation organisation)
        {
            var viewModel = new ViewAndEditOrganisationViewModel
            {
                OrganisationId = organisation.OrganisationId,
                Email = organisation.OrganisationData?.Email,
                PhoneNumber = organisation.OrganisationData?.PhoneNumber,
                WebsiteLink = organisation.OrganisationData?.WebsiteLink,
                Address1 = organisation.OrganisationData?.Address1,
                Address2 = organisation.OrganisationData?.Address2,
                Address3 = organisation.OrganisationData?.Address3,
                Address4 = organisation.OrganisationData?.Address4,
                Postcode = organisation.OrganisationData?.Postcode,
                PrimaryContact = !string.IsNullOrEmpty(organisation.PrimaryContact)
                    ? organisation.PrimaryContact
                    : string.Empty,
                PrimaryContactName = !string.IsNullOrEmpty(organisation.PrimaryContactName)
                    ? organisation.PrimaryContactName
                    : string.Empty
            };

            return viewModel;
        }

        private async Task<EpaOrganisation> GetEpaOrganisation(string epaoid)
        {
            try
            {
                return await _organisationsApiClient.GetEpaOrganisation(epaoid);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
            }

            return null;
        }
    }
}