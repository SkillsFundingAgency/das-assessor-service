(function($) {
    var findAddressVal = $("#postcode-search").val();

  // enable when service is available
  $(".js-search-address-heading").removeClass("hidden");
  $("#address-lookup").removeClass("disabled");
  $("#postcode-search").prop("disabled", false);

  $("#enterAddressManually").on("click", function(e) {
    e.preventDefault();
    $("#addressManualWrapper").unbind("click");
    $(
      ".js-address-panel, .js-select-previous-address, .js-search-address-heading, #address-lookup"
    ).addClass("hidden");
    $(".js-manual-address-heading").removeClass("hidden");
    $("#address-lookup").removeClass("hide-nojs");
    $(".address-manual-input").removeClass("js-hidden");
    $("#Employer").focus();
  });

  $("#addressManualWrapper, button[type=submit]").bind("click", function() {
    $(this).unbind("click");
  });

  $("#postcode-search").keyup(function() {
    findAddressVal = $(this).val();
  });

  $("#postcode-search")
    .autocomplete({
      classes: {
        "ui-autocomplete": "govuk-list"
      },
      search: function() {
        $("#addressLoading").show();
        $("#enterAddressManually").hide();
      },
      source: function(request, response) {
        $.ajax({
          url:
              "/locations",
          dataType: "json",
          data: {
            query: request.term
          },
          timeout: 5000,
          success: function(data) {
            $("#postcodeServiceUnavailable").hide();
            $("#addressLoading").hide();
            $("#enterAddressManually").show();

            $("#postcode-search").one("blur", function() {
              $("#enterAddressManually").show();
              $("#addressLoading").hide();
            });

            response(
              $.map(data, function(suggestion) {
                return {
                  label: suggestion.text,
                  value: "",
                  data: suggestion
                };
              })
            );
          },
          error: function() {
            $("#postcodeServiceUnavailable").show();
            $("#enterAddressManually").show();
            $("#addressLoading").hide();
            $("#address-details").removeClass("js-hidden");
          }
        });
      },
      messages: {
        noResults: function() {
          return "We can't find an address matching " + findAddressVal;
        },
        results: function(amount) {
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
      select: function(event, ui) {
          var item = ui.item.data;
          populateAddress(item);
      },
      focus: function(_, ui) {
        $("#addressInputWrapper")
          .find(".ui-helper-hidden-accessible")
          .text("To select " + ui.item.label + ", press enter");
      },
      autoFocus: true,
      minLength: 1,
      delay: 100
    });
    
  function populateAddress(address) {
    var addressFields = {
      Employer: address.organisation,
      AddressLine1: address.addressLine1,
      AddressLine2: address.addressLine2,
      AddressLine3: address.addressLine3,
      City: address.town,
      Postcode: address.postcode
    };

    $(".js-address-panel").removeClass("hidden");
    $(".js-address-panel ul").empty();
    $.each(addressFields, function(index, value) {
      $("#" + index).val(value);
      $(".js-address-panel ul").append("<li>" + value + "</li>");
    });

    // populate hidden field for accessibility
    $("#ariaAddressEntered").text(
      "Your address has been entered into the fields below."
    );
  }
})(jQuery);
