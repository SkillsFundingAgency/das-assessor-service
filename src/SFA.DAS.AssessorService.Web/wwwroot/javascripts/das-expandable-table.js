(function(global) {
  "use strict";

  var $ = global.jQuery;
  var DASFrontend = global.DASFrontend || {};

  DASFrontend.expandableTable = {
    init: function init() {
      // loop over table row and add relevant attributes
      $(".js-expand-table-row").each(function() {
        var ariaControlId = "table-content-" + $(this).data("expand-id");
        var $columnsAboveRow = $(this)
          .closest("tr")
          .find("td");
        var $expandable = $(this)
          .closest("tr")
          .nextAll("tr.js-expandble-cell:first");
        var $arrow = $(this).find("i.arrow");
        $(this).attr({
          "aria-expanded": "false",
          "aria-controls": ariaControlId
        });
        $expandable.attr({ "aria-hidden": "true", id: ariaControlId });
        $arrow.attr("aria-hidden", "true");

        // show and hide based on click and update aria tags
        $(this).on("click keypress", function(event) {
          event.preventDefault();
          if (event.type === "keypress" && event.keyCode !== 13) return;
          if ($expandable.hasClass("expandble-cell--hidden")) {
            // SHOW CONTENT
            $(this).attr({
              "aria-expanded": "true",
              "aria-controls": ariaControlId
            });
            $expandable
              .removeClass("expandble-cell--hidden")
              .attr("aria-hidden", "false");
            $arrow
              .attr({ class: "arrow arrow-open", "aria-hidden": "false" })
              .text("\u25bc");
            $columnsAboveRow.addClass("hidden-bottom-border");
          } else {
            // HIDE CONTENT
            $(this).attr("aria-expanded", "false");
            $expandable
              .addClass("expandble-cell--hidden")
              .attr({ "aria-hidden": "true", id: ariaControlId });
            $arrow
              .attr({ class: "arrow arrow-closed", "aria-hidden": "true" })
              .text("\u25ba");
            $columnsAboveRow.removeClass("hidden-bottom-border");
          }
        });
      });
    }
  };

  global.DASFrontend = DASFrontend;
})(window);
