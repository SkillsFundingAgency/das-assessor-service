GOVUK.epaoValidate = function(formElement, validationRulesObject) {
  var documentTitle = $(document).attr('title');
  var validator = formElement
    .bind('invalid-form.validate', function() {
      $(document).attr('title', 'Error: ' + documentTitle);
      if ($('.js-error-summary').length) {
        $('.js-error-summary-list').empty();
        validator.errorList.forEach(function(error) {
          $('.js-error-summary-list').append(
            '<li><a href="#' + error.element.id + '">' + error.message + '</a></li>'
          );
        });
        $('.js-error-summary')
          .show()
          .focus();
        $('.js-error-summary a').click(function(e) {
          e.preventDefault();
          var href = $(this).attr('href');
          $(href).is(':visible') ? $(href).focus() : $('input.form-control:first').focus();
        });
      } else {
        // Otherwise, set focus to the field with the error
        $('input.form-control:first').focus();
      }
    })
    .validate({
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

        if (
          $(element)
            .closest('fieldset')
            .prev()
            .hasClass('js-error-summary')
        ) {
          $(element)
            .closest('fieldset')
            .addClass('after-error-summary');
        }
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

        if (
          $(element)
            .closest('fieldset')
            .prev()
            .hasClass('js-error-summary')
        ) {
          $(element)
            .closest('fieldset')
            .removeClass('after-error-summary');
        }
      },
      rules: validationRulesObject.rules,
      messages: validationRulesObject.messages,
      errorPlacement: function(error, element) {
        if (element.attr('type') == 'radio') {
          $('.error-message-container')
            .addClass('form-group-error')
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
      submitHandler: function(form) {
        form.submit();
      }
    });

  // Ensures date is not in the future
  jQuery.validator.addMethod(
    'noFutureDate',
    function(value, element) {
      var now = new Date();
      var userDate = parseDate(value);
      return userDate ? this.optional(element) || userDate < now : true;
    },
    'Please enter a date in the past'
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

  // Helper to confirm date input is correct format
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
};
