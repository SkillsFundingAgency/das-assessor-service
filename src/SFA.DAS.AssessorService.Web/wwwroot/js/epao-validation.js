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
        // console.log('h', element.id);

        if ($(element).hasClass('date-input')) return;

        $(element)
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
        // console.log('u', element.id);

        if ($(element).hasClass('date-input')) return;

        $(element)
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

  // date only future
  jQuery.validator.addMethod(
    'noFutureDate',
    function(value, element) {
      var now = new Date();
      var userDate = new Date(value);
      if (
        Object.prototype.toString.call(userDate) === '[object Date]' &&
        !isNaN(userDate.getTime())
      ) {
        return this.optional(element) || userDate < now;
      } else {
        return true;
      }
    },
    'Please enter a date in the past'
  );

  $(window).load(function() {
    // If there is an error summary, set focus to the summary
    if ($('.error-summary').length) {
      $('.error-summary').focus();
      $('.error-summary a').click(function(e) {
        e.preventDefault();
        var href = $(this).attr('href');
        $(href).is(':visible') ? $(href).focus() : $('input.form-control:first').focus();
      });
    } else {
      // Otherwise, set focus to the field with the error
      $('input.form-control:first').focus();
    }
  });
};
