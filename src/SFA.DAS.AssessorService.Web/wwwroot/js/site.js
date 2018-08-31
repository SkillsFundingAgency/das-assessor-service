/* global $ */
/* global jQuery */
/* global GOVUK */

$(document).ready(function() {
  // Turn off jQuery animation
  jQuery.fx.off = true;

  // Where .multiple-choice uses the data-target attribute
  // to toggle hidden content
  var showHideContent = new GOVUK.ShowHideContent();
  showHideContent.init();

  GOVUK.expandableTable.init();

  // Use GOV.UK shim-links-with-button-role.js to trigger a link styled to look like a button,
  // with role="button" when the space key is pressed.
  GOVUK.shimLinksWithButtonRole.init();

  // Details/summary polyfill from frontend toolkit
  GOVUK.details.init();

  // Prevent multiple form submissions
  $('form').on('submit', function() {
    if (!$.validator || $(this).valid()) {
      $('form .button[type=submit]').prop('disabled', true);
    }
  });
});

$(window).load(function() {
  // If there is an error summary, set focus to the summary
  if ($('.error-summary').length) {
    $('.error-summary').focus();
    $('.error-summary a').click(function(e) {
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
});

// trim fields on submit
$('main#content form').on('submit', function() {
  $('input[type=text]').val(function(_, value) {
    return $.trim(value);
  });
});
