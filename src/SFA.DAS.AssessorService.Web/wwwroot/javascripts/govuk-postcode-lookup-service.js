// This version of the postcode-lookup-service uses the 5 address fields; building and street (1 & 2), 
// town or city, county and postcode which are recommended by the gov uk style guidelines
// see https://design-system.service.gov.uk/patterns/addresses/
(function ($) {
    var findAddressVal = $("#postcode-search").val();

    var hasAddressValidationErrors = ($("div.govuk-form-group--error").length > 0);

    // when errors present from previous selection the manual input must be used
    if ((restorePreviousAddress() === true && hasAddressValidationErrors) || hasAddressValidationErrors) {
        enableEnterAddressManually();
        highlightAddressIfNotSet();
        return;
    }

    $("#addressLookupWrapper").removeClass("disabled");
    $("#postcode-search").prop("disabled", false);

    $("#enterAddressManually").on("click", function (e) {
        e.preventDefault();
        enableEnterAddressManually();
    });

    $("#searchAgain").on("click", function (e) {
        e.preventDefault();
        searchAgain();
    });

    $("#postcode-search").on('keyup', function () {
        findAddressVal = $(this).val();
    });

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
                        query: request.term
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
        var addressFields = {
            '.address-manual-input-address-line-1': address.addressLine1,
            '.address-manual-input-address-line-2': address.addressLine2,
            '.address-manual-input-address-line-3': address.town,
            '.address-manual-input-address-line-4': '',
            '.address-manual-input-postcode': address.postcode
        };

        $(".js-address-panel").removeClass("hidden");
        $(".js-address-panel ul").empty();
        $.each(addressFields, function (index, value) {
            $(index).val(value);
            $(".js-address-panel ul").append("<li>" + value + "</li>");
        });

        // populate hidden field for accessibility
        $("#ariaAddressEntered").text(
            "Your address has been entered into the fields below."
        );

        $("#address-manual, #address-lookup").addClass("hidden");
        $("#search-again").removeClass("hidden");
    }

    function restorePreviousAddress() {
        var hasPreviousValues = false;

        var addressFields = {
            '.address-manual-input-address-line-1': '',
            '.address-manual-input-address-line-2': '',
            '.address-manual-input-address-line-3': '',
            '.address-manual-input-address-line-4': '',
            '.address-manual-input-postcode': ''
        };

        $.each(addressFields, function (index, value) {
            $(".js-address-panel ul").append("<li>" + $(index).val() + "</li>");
            if (!($(index).val().length === 0)) {
                if (!($("#postcode-search").val().length === 0)) {
                    $("#postcode-search").val($("#postcode-search").val() + ', ');
                }
                $("#postcode-search").val($("#postcode-search").val() + $(index).val());
                hasPreviousValues = true;
            }
        });

        if (hasPreviousValues) {
            $("#address-manual, #address-lookup").addClass("hidden");
            $("#search-again").removeClass("hidden");
            $(".js-address-panel").removeClass("hidden");
        }

        return hasPreviousValues;
    }

    function enableEnterAddressManually() {
        $(".js-address-panel, #addressLookupWrapper").addClass("hidden");
        $("#addressLookupWrapper").removeClass("hide-nojs");
        $(".address-manual-input").removeClass("js-hidden");
        $(".address-manual-input-focus").focus();
    }

    function highlightAddressIfNotSet() {
        if ($(".address-manual-input-address-line-1").val().length === 0)
            $(".address-manual-input-address-line-1").addClass("govuk-input--error");
        if ($(".address-manual-input-address-line-3").val().length === 0)
            $(".address-manual-input-address-line-3").addClass("govuk-input--error");
        if ($(".address-manual-input-postcode").val().length === 0)
            $(".address-manual-input-postcode").addClass("govuk-input--error");
    }

    function searchAgain() {
        var addressFields = {
            '.address-manual-input-address-line-1': '',
            '.address-manual-input-address-line-2': '',
            '.address-manual-input-address-line-3': '',
            '.address-manual-input-address-line-4': '',
            '.address-manual-input-postcode': ''
        };

        $.each(addressFields, function (index, value) {
            $(index).val(value);
        });

        $("#address-manual, #address-lookup").removeClass("hidden");
        $("#search-again, .js-address-panel").addClass("hidden");
        $("#postcode-search").val("");
    }
})(jQuery);
