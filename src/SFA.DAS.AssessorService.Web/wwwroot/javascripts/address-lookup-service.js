// This version of the address lookup service can populate the 5 address fields; building and street (1, 2),
// town or city, county and postcode which are recommended by the govuk style guidelines; it can also populate the
// more correct 5 address fields building and street (1, 2, 3), town or city and postcode
// see https://design-system.service.gov.uk/patterns/addresses/
(function ($) {
    var findAddressVal = $("#postcode-search").val();
    var hasAddressValidationErrors = ($("div.govuk-form-group--error").length > 0);

    var addressFields = {
        '.address-manual-input-organisation': '',
        '.address-manual-input-address-line-1': '',
        '.address-manual-input-address-line-2': '',
        '.address-manual-input-address-line-3': '',
        '.address-manual-input-address-line-4': '',
        '.address-manual-input-town': '',
        '.address-manual-input-county': '',
        '.address-manual-input-postcode': ''
    };

    // when errors are present from a previous selection the manual input must be used
    if ((restorePreviousAddress() === true && hasAddressValidationErrors) || hasAddressValidationErrors) {
        enableEnterAddressManually(false);
        highlightAddressErrorsIfNotSet();
        return;
    }

    $("#addressLookupWrapper").removeClass("disabled");
    $("#postcode-search").prop("disabled", false);

    $("#enterAddressManually").on("click", function (e) {
        e.preventDefault();
        enableEnterAddressManually(true);

        if ($('.address-manual-input-organisation').length > 0 && $('#EmployerName').length > 0) {
            // populated the organisation with the default employer name
            $('.address-manual-input-organisation').val($('#EmployerName').val());
        }
    });

    $("#searchAgain").on("click", function (e) {
        e.preventDefault();
        searchAgain();
    });

    $("#postcode-search").on('keyup', function () {
        findAddressVal = $(this).val();

        if (findAddressVal.length == 0) {
            searchAgain();
        }
    });

    var includeOrganisations = $("#postcode-search").hasClass('include-organisations');

    $("#postcode-search")
        .autocomplete({
            classes: {
                "ui-autocomplete": "govuk-list"
            },
            search: function () {
                $("#addressLoading").show();
                $("#enterAddressManually").hide();
            },
            source: function (request, response) {
                $.ajax({
                    url:
                        "/locations",
                    dataType: "json",
                    data: {
                        query: request.term,
                        includeOrganisations: includeOrganisations
                    },
                    timeout: 5000,
                    success: function (data) {
                        $("#postcodeServiceUnavailable").hide();
                        $("#addressLoading").hide();
                        $("#enterAddressManually").show();

                        $("#postcode-search").one("blur", function () {
                            $("#enterAddressManually").show();
                            $("#addressLoading").hide();
                        });

                        response(
                            $.map(data, function (suggestion) {
                                return {
                                    label: suggestion.text,
                                    value: "",
                                    data: suggestion
                                };
                            })
                        );
                    },
                    error: function () {
                        $("#postcodeServiceUnavailable").show();
                        $("#enterAddressManually").show();
                        $("#addressLoading").hide();
                        $("#address-details").removeClass("js-hidden");
                    }
                });
            },
            messages: {
                noResults: function () {
                    return "We can't find an address matching " + findAddressVal;
                },
                results: function (amount) {
                    return (
                        "We've found " +
                        amount +
                        (amount > 1 ? " addresses" : " address") +
                        " that match " +
                        findAddressVal +
                        ". Use up and down arrow keys to navigate"
                    );
                }
            },
            select: function (event, ui) {
                var item = ui.item.data;
                if (item.text === 'No results found')
                    return false;

                populateAddress(item);
            },
            focus: function (_, ui) {
                $("#address-lookup")
                    .find(".ui-helper-hidden-accessible")
                    .text("To select " + ui.item.label + ", press enter");
            },
            autoFocus: true,
            minLength: 1,
            delay: 100
        });

    function populateAddress(address) {
        var showAddressSelectionPanel = !$(".js-address-panel").hasClass("js-address-panel-never-show");
        if (showAddressSelectionPanel) {
            $(".js-address-panel").removeClass("hidden");
        }

        $(".js-address-panel ul").empty();

        if (address.organisation.length > 0) {
            populateAddressField('.address-manual-input-organisation', address.organisation);
        }
        else if ($('#EmployerName').length > 0) {
            populateAddressField('.address-manual-input-organisation', $('#EmployerName').val());
        }

        populateAddressField('.address-manual-input-address-line-1', address.addressLine1);
        populateAddressField('.address-manual-input-address-line-2', address.addressLine2);
        populateAddressField('.address-manual-input-address-line-3', address.addressLine3);

        if ($('.address-manual-input-town').length) {
            populateAddressField('.address-manual-input-town', address.town);
        }
        else {
            // town is currently defined as address-line-3 in the QnA _Address.cshtml
            populateAddressField('.address-manual-input-address-line-3', address.town);
        }

        if ($('.address-manual-input-county').length) {
            populateAddressField('.address-manual-input-county', address.county);
        }
        else {
            // county is currently defined as address-line-4 in the QnA _Address.cshtml
            populateAddressField('.address-manual-input-address-line-4', address.county);
        }

        populateAddressField('.address-manual-input-postcode', address.postcode);

        // populate hidden field for accessibility
        $("#ariaAddressEntered").text(
            "Your address has been entered into the fields below."
        );

        if (showAddressSelectionPanel) {
            $("#address-manual, #address-lookup").addClass("hidden");
            $("#search-again").removeClass("hidden");
        }
    }

    function populateAddressField(className, addressValue) {
        if ($(className).length) {
            $(className).val(addressValue);
            $(".js-address-panel ul").append("<li>" + htmlEncode(addressValue) + "</li>");
        }
    }

    function htmlEncode(str) {
        return String(str).replace(/&/g, '&amp;')
                          .replace(/</g, '&lt;')
                          .replace(/>/g, '&gt;')
                          .replace(/"/g, '&quot;');
    }

    function restorePreviousAddress() {
        var hasPreviousValues = false;
        var showAddressSelectionPanel = !$(".js-address-panel").hasClass("js-address-panel-never-show");

        $.each(addressFields, function (index, value) {
            if ($(index).length && !($(index).val().length === 0)) {
                $(".js-address-panel ul").append("<li>" + htmlEncode($(index).val()) + "</li>");

                if ($("#postcode-search").length > 0 && $("#postcode-search").val().length > 0) {
                    $("#postcode-search").val($("#postcode-search").val() + ', ');
                }

                $("#postcode-search").val($("#postcode-search").val() + $(index).val());

                hasPreviousValues = true;
            }
        });

        if (hasPreviousValues && showAddressSelectionPanel) {
            $("#address-manual, #address-lookup").addClass("hidden");
            $("#search-again").removeClass("hidden");
            $(".js-address-panel").removeClass("hidden");
        }

        return hasPreviousValues;
    }

    function enableEnterAddressManually(clearInputFields) {
        $(".js-error-summary").hide();
        $(".js-address-panel, #addressLookupWrapper").addClass("hidden");
        $("#addressLookupWrapper").removeClass("hide-nojs");
        $(".js-search-address-heading").addClass("hidden");
        $(".js-manual-address-heading").removeClass("hidden");
        $(".address-manual-input").removeClass("js-hidden");

        // clear all the manual input fields
        if (clearInputFields) {
            $.each(addressFields, function (index, value) {
                $(index).val("");
            });
        }

        $(".address-manual-input-focus").focus();
    }

    function highlightAddressErrorsIfNotSet() {
        if ($(".address-manual-input-address-line-1").val().length === 0)
            $(".address-manual-input-address-line-1").addClass("govuk-input--error");

        if ($(".address-manual-input-town").length) {
            if ($(".address-manual-input-town").val().length === 0)
                $(".address-manual-input-town").addClass("govuk-input--error");
        }
        else {
            // town is currently defined as address-line-3 in the QnA _Address.cshtml
            if ($(".address-manual-input-address-line-3").val().length === 0)
                $(".address-manual-input-address-line-3").addClass("govuk-input--error");
        }

        if ($(".address-manual-input-postcode").val().length === 0)
            $(".address-manual-input-postcode").addClass("govuk-input--error");
    }

    function searchAgain() {
        $.each(addressFields, function (index, value) {
            $(index).val(value);
        });

        $("#address-manual, #address-lookup").removeClass("hidden");
        $("#search-again, .js-address-panel").addClass("hidden");
        $(".js-address-panel ul li").remove();
        $("#postcode-search").val("");
    }
})(jQuery);
