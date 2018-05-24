// provides the matching addresses from postcode
(function($) {
    var searchContext = '',
        uri = $('form').attr('action'),
        findAddressVal = $('#postcode-search').val();

    // enable when service is available
    $('#address-lookup').removeClass('disabled');
    $('#postcode-search').prop('disabled', false);

    // grey out manual input
    if (!hasValue('#address-details input')) {
        $('#address-details').addClass('disabled');
    }

    $('#enterAddressManually').on('click', function(e) {
        e.preventDefault();
        $('#addressManualWrapper').unbind('click');

        $('#address-details').removeClass('disabled');
        $('#AddressLine1').focus();
    });

    $('#addressManualWrapper, button[type=submit]').bind('click', function() {
        $(this).unbind('click');
        $('#address-details').removeClass('disabled');
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
                        $('#address-details').removeClass('disabled');
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
        $('#address-details').addClass('disabled');

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
                    $('#address-details').removeClass('disabled');
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
                $('#address-details').removeClass('disabled');
            }
        });
    }

    function populateAddress(address) {
        // If we decide to autopopulate the #Employer field.
        // if (!$('#Employer').val()) {
        //   $('#Employer').val(address.Company);
        // }
        $('#AddressLine1').val(address.Line1);
        $('#AddressLine2').val(address.Line2);
        $('#AddressLine3').val(address.Line3);
        $('#City').val(address.City);
        $('#Postcode').val(address.PostalCode);

        $('#ariaAddressEntered').text('Your address has been entered into the fields below.');
    }

})(jQuery);
