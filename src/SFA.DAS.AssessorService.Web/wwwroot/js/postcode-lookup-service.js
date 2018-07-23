// provides the matching addresses from postcode
(function($) {
  var searchContext = '',
    uri = $('form').attr('action'),
    findAddressVal = $('#postcode-search').val();

  // enable when service is available
  $('#address-lookup').removeClass('disabled');
  $('#postcode-search').prop('disabled', false);

  // grey out manual input
  // if (!hasValue('#address-details input')) {
  // $('#address-details').addClass('hidden');
  // }

  $('#enterAddressManually').on('click', function(e) {
    e.preventDefault();
    $('#addressManualWrapper').unbind('click');
    $('.js-address-panel').addClass('hidden');

    $('#address-details').removeClass('js-hidden');
    $('#AddressLine1').focus();
  });

  $('#addressManualWrapper, button[type=submit]').bind('click', function() {
    $(this).unbind('click');
    $('#AddressLine1').focus();
  });

  $('#postcode-search').keyup(function() {
    findAddressVal = $(this).val();
  });

  $('#postcode-search')
    .autocomplete({
      search: function() {
        $('#addressLoading').show();
        $('#enterAddressManually').hide();
      },
      source: function(request, response) {
        $.ajax({
          url:
            '//services.postcodeanywhere.co.uk/CapturePlus/Interactive/Find/v2.10/json3.ws',
          dataType: 'jsonp',
          data: {
            key: 'JY37-NM56-JA37-WT99',
            country: 'GB',
            searchTerm: request.term,
            lastId: searchContext
          },
          timeout: 5000,
          success: function(data) {
            $('#postcodeServiceUnavailable').hide();
            $('#addressLoading').hide();
            $('#enterAddressManually').show();

            $('#postcode-search').one('blur', function() {
              $('#enterAddressManually').show();
              $('#addressLoading').hide();
            });

            response(
              $.map(data.Items, function(suggestion) {
                return {
                  label: suggestion.Text,
                  value: '',
                  data: suggestion
                };
              })
            );
          },
          error: function() {
            $('#postcodeServiceUnavailable').show();
            $('#enterAddressManually').show();
            $('#addressLoading').hide();
            $('#address-details').removeClass('js-hidden');
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
            (amount > 1 ? ' addresses' : ' address') +
            ' that match ' +
            findAddressVal +
            '. Use up and down arrow keys to navigate'
          );
        }
      },
      select: function(event, ui) {
        var item = ui.item.data;

        if (item.Next == 'Retrieve') {
          //retrieve the address
          retrieveAddress(item.Id);
          searchContext = '';
        } else {
          var field = $(this);
          searchContext = item.Id;

          $('#addressLoading').show();
          $('#enterAddressManually').hide();
          $('#postcodeServiceUnavailable').hide();

          if (searchContext === 'GBR|') {
            window.setTimeout(function() {
              field.autocomplete('search', item.Text);
            });
          } else {
            window.setTimeout(function() {
              field.autocomplete('search', item.Id);
            });
          }
        }
      },
      focus: function(event, ui) {
        $('#addressInputWrapper')
          .find('.ui-helper-hidden-accessible')
          .text('To select ' + ui.item.label + ', press enter');
      },
      autoFocus: true,
      minLength: 1,
      delay: 100
    })
    .focus(function() {
      searchContext = '';
    });

  function hasValue(elem) {
    return (
      $(elem).filter(function() {
        return $(this).val();
      }).length > 0
    );
  }

  function retrieveAddress(id) {
    $('#addressLoading').show();
    $('#enterAddressManually').hide();
    $('#postcodeServiceUnavailable').hide();
    $('#address-details').addClass('js-hidden');

    $.ajax({
      url:
        '//services.postcodeanywhere.co.uk/CapturePlus/Interactive/Retrieve/v2.10/json3.ws',
      dataType: 'jsonp',
      data: {
        key: 'JY37-NM56-JA37-WT99',
        id: id
      },
      timeout: 5000,
      success: function(data) {
        if (data.Items.length) {
          $('#addressLoading').hide();
          $('#enterAddressManually').show();
          $('#addressManualWrapper').unbind('click');
          $('#postcode-search').val('');
          populateAddress(data.Items[0]);
        }
      },
      error: function() {
        $('#postcodeServiceUnavailable').show();
        $('#enterAddressManually').hide();
        $('#addressLoading').hide();
        $('#address-details').removeClass('js-hidden');
      }
    });
  }

  function populateAddress(address) {
    var addressFields = {
      Employer: address.Company,
      AddressLine1: address.Line1,
      AddressLine2: address.Line2,
      AddressLine3: address.Line3,
      City: address.City,
      Postcode: address.PostalCode
    };

    $('.js-address-panel').removeClass('hidden');
    $('.js-address-panel ul').empty();
    $.each(addressFields, function(index, value) {
      $('#' + index).val(value);
      $('.js-address-panel ul').append('<li>' + value + '</li>');
    });

    // populate hidden field for accessibility
    $('#ariaAddressEntered').text(
      'Your address has been entered into the fields below.'
    );
  }
})(jQuery);
