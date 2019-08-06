using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels;
using SFA.DAS.AssessorService.Web.ViewModels.Account;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [Authorize]
    [CheckSession]
    public class OrganisationController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IEmailApiClient _emailApiClient;
        private readonly IValidationApiClient _validationApiClient;
        private readonly ILogger<OrganisationController> _logger;

        public OrganisationController(ILogger<OrganisationController> logger, IHttpContextAccessor contextAccessor, 
            IOrganisationsApiClient organisationsApiClient, IContactsApiClient contactsApiClient,
            IEmailApiClient emailApiClient, IValidationApiClient validationApiClient)
        {
            _contextAccessor = contextAccessor;
            _organisationsApiClient = organisationsApiClient;
            _contactsApiClient = contactsApiClient;
            _emailApiClient = emailApiClient;
            _validationApiClient = validationApiClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ukprn = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")?.Value;
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;

            OrganisationResponse organisation;

            try
            {
                if (ukprn != null)
                    organisation = await _organisationsApiClient.Get(ukprn);
                else
                {
                    var epaOrganisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                    organisation = new OrganisationResponse
                    {
                        EndPointAssessorName = epaOrganisation.Name,
                        EndPointAssessorOrganisationId = epaoid,
                        EndPointAssessorUkprn = null,
                        Id = epaOrganisation.Id,
                        PrimaryContact = epaOrganisation.PrimaryContact,
                        Status = epaOrganisation.Status
                    };
                }
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }

            return View(organisation);
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        public async Task<IActionResult> OrganisationDetails()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                var viewModel = MapOrganisationModel(organisation);

                var userId = _contextAccessor.HttpContext.User.FindFirst("UserId").Value;
                var userPrivileges = await _contactsApiClient.GetContactPrivileges(Guid.Parse(userId));
                
                viewModel.UserHasChangeOrganisationPrivilege =
                    userPrivileges.Any(cp => cp.Privilege.Key == Privileges.ChangeOrganisationPrivilege);

                if(viewModel.UserHasChangeOrganisationPrivilege == false)
                {
                    var changeOrganisationPriviledge = (await _contactsApiClient.GetPrivileges()).First(p => p.Key == Privileges.ChangeOrganisationPrivilege);
                    viewModel.AccessDeniedViewModel = new AccessDeniedViewModel
                    {
                        Title = Privileges.ChangeOrganisationPrivilege,
                        Description = changeOrganisationPriviledge.Description,
                        PrivilegeId = changeOrganisationPriviledge.Id,
                        ContactId = Guid.Parse(userId),
                        UserHasUserManagement = userPrivileges.Any(up => up.Privilege.Key == Privileges.ManageUsers),
                        ReturnController = ControllerContext.ActionDescriptor.ControllerName,
                        ReturnAction = ControllerContext.ActionDescriptor.ActionName
                    };
                }

                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
        public async Task<IActionResult> SelectOrChangeContactName()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                var contacts = await _contactsApiClient.GetAllContactsForOrganisation(organisation.OrganisationId);

                var viewModel = new SelectOrChangeContactNameViewModel
                {
                    Contacts = contacts,
                    PrimaryContact = ModelState.IsValid
                        ? organisation.PrimaryContact
                        : ModelState["PrimaryContact"].AttemptedValue
                };
           
                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
        public async Task<IActionResult> SelectOrChangeContactName(SelectOrChangeContactNameViewModel vm)
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                var contacts = await _contactsApiClient.GetAllContactsForOrganisation(organisation.OrganisationId);

                var primaryContact = !string.IsNullOrEmpty(vm.PrimaryContact)
                    ? await _contactsApiClient.GetByUsername(vm.PrimaryContact)
                    : null;

                if (vm.ActionChoice == "Save")
                {
                    if (organisation.PrimaryContact == vm.PrimaryContact)
                    {
                        ModelState.AddModelError("PrimaryContact", "The contact name has not been changed.");
                    }

                    if (string.IsNullOrEmpty(vm.PrimaryContact))
                    {
                        ModelState.AddModelError("PrimaryContact", "The contact name cannot be removed");
                    }

                    if (organisation.Id != primaryContact?.OrganisationId)
                    {
                        ModelState.AddModelError("PrimaryContact", "The contact name cannot be changed to a contact of a different organisation");
                    }

                    if (!ModelState.IsValid)
                    {
                        return RedirectToAction("SelectOrChangeContactName");
                    }

                    vm = new SelectOrChangeContactNameViewModel
                    {
                        Contacts = contacts,
                        PrimaryContact = vm.PrimaryContact,
                        PrimaryContactName = contacts.FirstOrDefault(p => p.Username == vm.PrimaryContact)?.DisplayName
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

                    if (await _organisationsApiClient.UpdateEpaOrganisationPrimaryContact(request))
                    {
                        var contactsWithPrivileges = await _contactsApiClient.GetContactsWithPrivileges(organisation.Id);
                        var contactsWithManageUserPrivilege = contactsWithPrivileges?
                            .Where(c => c.Privileges.Any(p => p.Key == Privileges.ManageUsers))
                            .Select(p => contacts.First(c => c.Id.Equals(p.Contact.Id)))
                            .ToList();

                        vm = new SelectOrChangeContactNameViewModel
                        {
                            Contacts = contactsWithManageUserPrivilege,
                            PrimaryContact = vm.PrimaryContact,
                            PrimaryContactName = contacts.FirstOrDefault(p => p.Username == vm.PrimaryContact)?.DisplayName
                        };

                        return View("SelectOrChangeContactNameUpdated", vm);
                    }
                    else
                    {
                        ModelState.AddModelError("PrimaryContact", "Unable to update the contact name at this time.");
                        return RedirectToAction("SelectOrChangeContactName");
                    }
                }
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }

            return RedirectToAction("OrganisationDetails");
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
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
                        : ModelState["PhoneNumber"]?.AttemptedValue
                };

                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
        public async Task<IActionResult> ChangePhoneNumber(ChangePhoneNumberViewModel vm)
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);

                if (vm.ActionChoice == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        // only check if a phone number has been entered - model has required validator
                        if (await _validationApiClient.ValidatePhoneNumber(vm.PhoneNumber) == false)
                        {
                            ModelState.AddModelError("PhoneNumber", "Enter a valid phone number");
                        }

                        if (vm.PhoneNumber.Equals(organisation.OrganisationData?.PhoneNumber))
                        {
                            ModelState.AddModelError("PhoneNumber", "Enter a different phone number");
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        return RedirectToAction("ChangePhoneNumber");
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

                    if (await _organisationsApiClient.UpdateEpaOrganisationPhoneNumber(request))
                    {
                        var contactsWithPrivileges = await _contactsApiClient.GetContactsWithPrivileges(organisation.Id);
                        var contactsWithManageUserPrivilege = contactsWithPrivileges?
                            .Where(c => c.Privileges.Any(p => p.Key == Privileges.ManageUsers))
                            .ToList();

                        vm = new ChangePhoneNumberViewModel
                        {
                            PhoneNumber = vm.PhoneNumber,
                            Contacts = contactsWithManageUserPrivilege
                        };

                        return View("ChangePhoneNumberUpdated", vm);
                    }
                    else
                    {
                        ModelState.AddModelError("PrimaryContact", "Unable to update the contact phone number at this time.");
                        return RedirectToAction("ChangePhoneNumber");
                    }
                }
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }

            return RedirectToAction("OrganisationDetails");
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
        public async Task<IActionResult> ChangeAddress()
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);
                return RedirectToAction("OrganisationDetails");
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
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
                        : ModelState["Email"]?.AttemptedValue
                };

                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel vm)
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);

                if (vm.ActionChoice == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        // only check if an email address has been entered - model has required validator
                        if (await _validationApiClient.ValidateEmailAddress(vm.Email) == false)
                        {
                            ModelState.AddModelError("Email", "Enter a valid email address");
                        }

                        // user can change case of an email only
                        if(vm.Email.Equals(organisation.OrganisationData?.Email))
                        {
                            ModelState.AddModelError("Email", "Enter a different email address");
                        }
                    }
                    
                    if (!ModelState.IsValid)
                    {
                        return RedirectToAction("ChangeEmail");
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

                    if (await _organisationsApiClient.UpdateEpaOrganisationEmail(request))
                    {
                        var contactsWithPrivileges = await _contactsApiClient.GetContactsWithPrivileges(organisation.Id);
                        var contactsWithManageUserPrivilege = contactsWithPrivileges?
                            .Where(c => c.Privileges.Any(p => p.Key == Privileges.ManageUsers))
                            .ToList();

                        vm = new ChangeEmailViewModel
                        {
                            Email = vm.Email,
                            Contacts = contactsWithManageUserPrivilege
                        };

                        return View("ChangeEmailUpdated", vm);
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "Unable to update the email address at this time.");
                        return RedirectToAction("ChangeEmail");
                    }
                }
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }

            return RedirectToAction("OrganisationDetails");
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Organisations })]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
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
                        : ModelState["WebsiteLink"]?.AttemptedValue
                };

                return View(viewModel);
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [PrivilegeAuthorize(Privileges.ChangeOrganisationPrivilege)]
        public async Task<IActionResult> ChangeWebsite(ChangeWebsiteViewModel vm)
        {
            var epaoid = _contextAccessor.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")?.Value;
            try
            {
                var organisation = await _organisationsApiClient.GetEpaOrganisation(epaoid);

                if (vm.ActionChoice == "Save")
                {
                    // only check if an web site link has been entered - model has required validator
                    if (ModelState.IsValid)
                    {
                        if (await _validationApiClient.ValidateWebsiteLink(vm.WebsiteLink) == false)
                        {
                            ModelState.AddModelError("WebsiteLink", "Enter a valid website address");
                        }

                        // user can change case of an email only
                        if (vm.WebsiteLink.Equals(organisation.OrganisationData?.WebsiteLink))
                        {
                            ModelState.AddModelError("WebsiteLink", "Enter a different website address");
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        return RedirectToAction("ChangeWebsite");
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

                    if (await _organisationsApiClient.UpdateEpaOrganisationWebsiteLink(request))
                    {
                        var contactsWithPrivileges = await _contactsApiClient.GetContactsWithPrivileges(organisation.Id);
                        var contactsWithManageUserPrivilege = contactsWithPrivileges?
                            .Where(c => c.Privileges.Any(p => p.Key == Privileges.ManageUsers))
                            .ToList();

                        vm = new ChangeWebsiteViewModel
                        {
                            WebsiteLink = vm.WebsiteLink,
                            Contacts = contactsWithManageUserPrivilege
                        };

                        return View("ChangeWebsiteUpdated", vm);
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "Unable to update the website address at this time.");
                        return RedirectToAction("ChangeWebsite");
                    }
                }
            }
            catch (EntityNotFoundException e)
            {
                _logger.LogWarning(e, "Failed to find organisation");
                return RedirectToAction("NotRegistered", "Home");
            }

            return RedirectToAction("OrganisationDetails");
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
    }
}