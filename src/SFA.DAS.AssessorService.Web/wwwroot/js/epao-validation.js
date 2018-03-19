GOVUK.epaoValidate = function(formElement, validationRulesObject) {
  var documentTitle = $(document).attr('title');
  var validator = formElement
    .bind('invalid-form.validate', function() {
      // Skip if no summary box in dom (single question)
      if ($('.js-error-summary-list').length === 0) return false;
      $('.js-error-summary-list').empty();
      $(document).attr('title', 'Error: ' + documentTitle);
      validator.errorList.forEach(function(error) {
        $('.js-error-summary-list').append(
          '<li><a href="#' + error.element.id + '">' + error.message + '</a></li>'
        );
      });
      $('.js-error-summary').show();
    })
    .validate({
      onkeyup: false,
      onclick: false,
      onfocusout: false,
      errorElement: 'span',
      errorClass: 'error-message',
      highlight: function(element) {
        $(element)
          .closest('.form-group')
          .addClass('form-group-error');
      },
      unhighlight: function(element) {
        $(element)
          .closest('.form-group')
          .removeClass('form-group-error');
      },
      rules: validationRulesObject.rules,
      messages: validationRulesObject.messages,
      submitHandler: function(form) {
        form.submit();
      }
    });
};
