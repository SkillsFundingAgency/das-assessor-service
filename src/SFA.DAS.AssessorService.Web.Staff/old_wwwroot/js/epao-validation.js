GOVUK.epaoValidate = function(formElement, validationRulesObject, forwardToForm) {
  var documentTitle = $(document).attr('title');
  var validator = formElement
    .bind('invalid-form.validate', function() {
      $(document).attr('title', 'Error: ' + documentTitle);
      if ($('.js-error-summary').length) {
        $('.js-error-summary-list').empty();
        validator.errorList.forEach(function(error) {
          $('.js-error-summary-list').append(
            '<li><a href="#' +
              error.element.id +
              '">' +
              error.message +
              '</a></li>'
          );
        });
        $('.js-error-summary')
          .show()
          .focus();
        $('.js-error-summary a').click(function(e) {
          e.preventDefault();
          var href = $(this).attr('href');
          $(href).is(':visible')
            ? $(href).focus()
            : $('input.form-control:first').focus();
        });
      } else {
        // Otherwise, set focus to the field with the error
        $('input.form-control:first').focus();
      }
    })
    .validate({
      normalizer: function(value) {
        return $.trim(value);
      },
      ignore: validationRulesObject.ignore,
      focusInvalid: false,
      onkeyup: false,
      onclick: false,
      onfocusout: false,
      errorElement: 'span',
      errorClass: 'error-message',
      highlight: function(element) {
        if ($(element).hasClass('date-input')) {
          $(element).addClass('form-control-error');
          return false;
        }

        $(element)
          .addClass('form-control-error')
          .closest('.form-group')
          .addClass('form-group-error');
      },
      unhighlight: function(element) {
        if ($(element).hasClass('date-input')) {
          $(element).removeClass('form-control-error');
          return false;
        }

        $(element)
          .removeClass('form-control-error')
          .closest('.form-group')
          .removeClass('form-group-error');
      },
      rules: validationRulesObject.rules,
      groups: validationRulesObject.groups,
      messages: validationRulesObject.messages,
      errorPlacement: function(error, element) {
        if (element.attr('type') == 'radio') {
          $('.error-message-container')
            .show()
            .append(error);
        } else if (element.hasClass('date-input')) {
          $('.error-message-container')
            .addClass('form-group-error')
            .show()
            .find('.form-date')
            .before(error);
        } else {
          error.insertBefore(element);
        }
      },
      submitHandler: function (form) {
          if (forwardToForm === undefined)
              form.submit();
          else
              forwardToForm();
      }
    });

  // Ensures date is valid
  jQuery.validator.addMethod(
    'isValidDate',
    function(value, element, params) {
      var dateString = getFullDate();
      return dateString
        ? this.optional(element) || parseDate(dateString.toString())
        : true;
    },
    'Please enter a valid user date'
  );

  // Ensures date is not in the future
  jQuery.validator.addMethod(
    'noFutureDate',
    function(value, element) {
      var now = new Date().getTime();
      var userDate = parseDate(getFullDate())
        ? parseDate(getFullDate()).getTime()
        : false;
      return userDate ? this.optional(element) || userDate < now : true;
    },
    'Please enter a date in the past'
  );

  // Ensures date is not before a set date
  jQuery.validator.addMethod(
    'earliestDate',
    function(value, element, params) {
      var userDate = parseDate(getFullDate())
        ? parseDate(getFullDate()).getTime()
        : false;
      var earliestDate = parseDate(params).getTime();
      return userDate
        ? this.optional(element) || userDate >= earliestDate
        : true;
    },
    'The entered date cannot be before the set date'
  );
  
  // Ensures address has been found
  jQuery.validator.addMethod(
    'addressFound',
    function(value, element) {
      var addressFound = $('.info-highlighted').is(':visible');
      return this.optional(element) || addressFound;
    },
    'Please select an address'
  );

  // Matches UK postcode. Does not match to UK Channel Islands that have their own postcodes (non standard UK)
  jQuery.validator.addMethod(
    'postcodeUK',
    function(value, element) {
      return (
        this.optional(element) ||
        /^((([A-PR-UWYZ][0-9])|([A-PR-UWYZ][0-9][0-9])|([A-PR-UWYZ][A-HK-Y][0-9])|([A-PR-UWYZ][A-HK-Y][0-9][0-9])|([A-PR-UWYZ][0-9][A-HJKSTUW])|([A-PR-UWYZ][A-HK-Y][0-9][ABEHMNPRVWXY]))\s?([0-9][ABD-HJLNP-UW-Z]{2})|(GIR)\s?(0AA))$/i.test(
          value
        )
      );
    },
    'Please specify a valid UK postcode'
  );

  // Matches a valid GUID. An empty GUID is not valid.
  jQuery.validator.addMethod(
    'validGuid',
    function(value, element) {
        return (
            this.optional(element) || value !== '00000000-0000-0000-0000-000000000000' &&
            /^([0-9a-f]{8}-?[0-9a-f]{4}-?[0-9a-f]{4}-?[0-9a-f]{4}-?[0-9a-f]{12})$/i.test(
                value
            )
      );
    },
    'Please specify a valid GUID'
  );

  // Helper to ensure date input is correct format
  function parseDate(str) {
    var t = str.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})$/);
    if (t !== null) {
      var d = +t[1],
        m = +t[2],
        y = +t[3];
      var date = new Date(y, m - 1, d);
      if (date.getFullYear() === y && date.getMonth() === m - 1) {
        return date;
      }
    }
    return false;
  }

  function getFullDate() {
    var day = $('[name="Day"]').val();
    var month = $('[name="Month"]').val();
    var year = $('[name="Year"]').val();
    return day + '/' + month + '/' + year;
  }
};

$(document).ready(function() {
  // Only allow integers
  $('.js-integers-only').on('keydown', function(e) {
    // Allow: backspace, delete, tab, escape and enter
    if (
      $.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
      // Allow: Ctrl/cmd+A
      (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
      // Allow: Ctrl/cmd+C
      (e.keyCode == 67 && (e.ctrlKey === true || e.metaKey === true)) ||
      // Allow: Ctrl/cmd+V
      (e.keyCode == 86 && (e.ctrlKey === true || e.metaKey === true)) ||
      // Allow: Ctrl/cmd+X
      (e.keyCode == 88 && (e.ctrlKey === true || e.metaKey === true)) ||
      // Allow: home, end, left, right
      (e.keyCode >= 35 && e.keyCode <= 39)
    ) {
      // let it happen, don't do anything
      return;
    }
    // Ensure that it is a number and stop the keypress
    if (
      (e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) &&
      (e.keyCode < 96 || e.keyCode > 105)
    ) {
      e.preventDefault();
    }
    });

  // Only allow alphanumeric characters
  $('.js-alphanumeric-only').on('keydown', function (e) {
      // Allow: backspace, delete, tab, escape and enter
      if (
          $.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
              // Allow: Ctrl/cmd+A
              (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
              // Allow: Ctrl/cmd+C
              (e.keyCode == 67 && (e.ctrlKey === true || e.metaKey === true)) ||
              // Allow: Ctrl/cmd+V
              (e.keyCode == 86 && (e.ctrlKey === true || e.metaKey === true)) ||
              // Allow: Ctrl/cmd+X
              (e.keyCode == 88 && (e.ctrlKey === true || e.metaKey === true)) ||
              // Allow: home, end, left, right
              (e.keyCode >= 35 && e.keyCode <= 39)
      ) {
          // let it happen, don't do anything
          return;
      }
      // Ensure that it is a letter or number and stop the keypress
      if (
          (e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) &&
          (e.keyCode < 65 || e.keyCode > 90) &&
          (e.keyCode < 96 || e.keyCode > 105)
      ) {
          e.preventDefault();
      }
  });
});
