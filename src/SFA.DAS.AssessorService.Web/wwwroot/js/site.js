// Write your JavaScript code.

$(function() {
  var documentTitle = $(document).attr('title');
  var validator = $('.apprentice-search-form')
    .bind('invalid-form.validate', function() {
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
      rules: {
        Surname: {
          required: true,
          maxlength: 100
        },
        Uln: {
          required: true,
          number: true,
          minlength: 10,
          maxlength: 10
        }
      },
      messages: {
        Surname: 'The last name field should not be empty',
        Uln: {
          required: 'The ULN field should not be empty',
          number: 'A ULN should contain exactly 10 numbers',
          minlength: 'A ULN should contain exactly 10 numbers',
          maxlength: 'A ULN should contain exactly 10 numbers'
        }
      },
      submitHandler: function(form) {
        form.submit();
      }
    });
});
