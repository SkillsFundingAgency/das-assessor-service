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

  // Use GOV.UK shim-links-with-button-role.js to trigger a link styled to look like a button,
  // with role="button" when the space key is pressed.
  // GOVUK.shimLinksWithButtonRole.init();

  // Details/summary polyfill from frontend toolkit
  // GOVUK.details.init();

  // stop input when more than set number of characters when data-maxlength attribute is present
  // e.g. data-maxlength="4"
  // $('*[data-maxlength]').on('keydown', function(event) {
  //   var keys = [69, 107, 109, 110, 187, 189, 190];
  //   if ($.inArray(event.which, keys) >= 0) event.preventDefault();

  //   if ($(this).val().length >= $(this).data('maxlength')) {
  //     if (event.keyCode >= 48 && event.keyCode <= 57) event.preventDefault();
  //   }
  // });
});
