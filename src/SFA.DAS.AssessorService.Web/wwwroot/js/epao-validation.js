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
          $(href).focus();
        });
      } else {
        // Otherwise, set focus to the field with the error
        $('.error input:first').focus();
      }
    })
    .validate({
      focusInvalid: false,
      onkeyup: false,
      onclick: false,
      onfocusout: false,
      errorElement: 'span',
      errorClass: 'error-message',
      highlight: function(element) {
        console.log('h', element.id);
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
        console.log('u', element.id);
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
        } else {
          error.insertBefore(element);
        }
      },
      submitHandler: function(form) {
        form.submit();
      }
    });
};
