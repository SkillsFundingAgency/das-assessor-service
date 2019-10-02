(function($) {
  /* using the route override for the OppFinder controller */
  var controller = "find-an-assessment-opportunity";

  refreshClickHandlers();

  $("#search").click(function(event) {
    var searchTerm = $("#search-term").val();
    initSearchPartial(
      { searchTerm: searchTerm },
      "#standard-filters",
      searchPartial
    );
    document.getElementById("main-content").scrollIntoView();
    event.preventDefault();
  });

  $(".js-toggle-filters-button").on("click", function(event) {
    var showHideFilterButton = $(this).find(".govuk-details__summary-text");
    var showHideFilterText = showHideFilterButton.text();

    showHideFilterText.indexOf("Show filters") !== -1
      ? showHideFilterButton.text("Hide filters")
      : showHideFilterButton.text("Show filters");
  });

  $("#apply-filters").click(function(event) {
    var sectorFilters = [];
    var levelFilters = [];

    $("input:checkbox[name=sectorFilters]:checked").each(function() {
      sectorFilters.push($(this).val());
    });

    $("input:checkbox[name=levelFilters]:checked").each(function() {
      levelFilters.push($(this).val());
    });

    let filterViewModel = {
      SectorFilters: sectorFilters,
      LevelFilters: levelFilters
    };

    applyFiltersPartial(filterViewModel, "#standard-filters", searchPartial);
    event.preventDefault();
  });

  $("#reset-filters").click(function(event) {
    resetFiltersPartial("#standard-filters", searchPartial);
    event.preventDefault();
  });

  function searchPartial() {
    changeStandardsPartial(
      "ChangePageApprovedStandardsPartial",
      { pageIndex: 1 },
      "#approved-standards",
      refreshApprovedClickHandlers
    );
    changeStandardsPartial(
      "ChangePageInDevelopmentStandardsPartial",
      { pageIndex: 1 },
      "#in-development-standards",
      refreshInDevelopmentClickHandlers
    );
    changeStandardsPartial(
      "ChangePageProposedStandardsPartial",
      { pageIndex: 1 },
      "#proposed-standards",
      refreshProposedClickHandlers
    );
  }

  function refreshClickHandlers() {
    refreshApprovedClickHandlers();
    refreshInDevelopmentClickHandlers();
    refreshProposedClickHandlers();
  }

  function refreshApprovedClickHandlers() {
    onClickChangePage(
      ".approved-page",
      "ChangePageApprovedStandardsPartial",
      "#approved-standards",
      refreshApprovedClickHandlers
    );
    onSelectStandardsPerPage(
      ".approved-per-page",
      "ShowApprovedStandardsPerPagePartial",
      "#approved-standards",
      refreshApprovedClickHandlers
    );
    onSelectSortableColumnHeader(
      ".approved-sort",
      "SortApprovedStandardsPartial",
      "#approved-standards",
      refreshApprovedClickHandlers
    );
  }

  function refreshInDevelopmentClickHandlers() {
    onClickChangePage(
      ".in-development-page",
      "ChangePageInDevelopmentStandardsPartial",
      "#in-development-standards",
      refreshInDevelopmentClickHandlers
    );
    onSelectStandardsPerPage(
      ".in-development-per-page",
      "ShowInDevelopmentStandardsPerPagePartial",
      "#in-development-standards",
      refreshInDevelopmentClickHandlers
    );
    onSelectSortableColumnHeader(
      ".in-development-sort",
      "SortInDevelopmentStandardsPartial",
      "#in-development-standards",
      refreshInDevelopmentClickHandlers
    );
  }

  function refreshProposedClickHandlers() {
    onClickChangePage(
      ".proposed-page",
      "ChangePageProposedStandardsPartial",
      "#proposed-standards",
      refreshProposedClickHandlers
    );
    onSelectStandardsPerPage(
      ".proposed-per-page",
      "ShowProposedStandardsPerPagePartial",
      "#proposed-standards",
      refreshProposedClickHandlers
    );
    onSelectSortableColumnHeader(
      ".proposed-sort",
      "SortProposedStandardsPartial",
      "#proposed-standards",
      refreshProposedClickHandlers
    );
  }

  function onClickChangePage(
    linkClass,
    actionMethod,
    containerId,
    refreshFunction
  ) {
    $(linkClass).each(function(i, obj) {
      $(obj).click(function(event) {
        var pageIndex = $(obj).attr("data-pageIndex");
        changeStandardsPartial(
          actionMethod,
          { pageIndex: pageIndex },
          containerId,
          refreshFunction
        );
        event.preventDefault();
      });
    });
  }

  function onSelectStandardsPerPage(
    selectClass,
    actionMethod,
    containerId,
    refreshFunction
  ) {
    $(selectClass).change(function() {
      var selectedVal = $(this)
        .find(":selected")
        .val();
      changeStandardsPartial(
        actionMethod,
        { standardsPerPage: selectedVal },
        containerId,
        refreshFunction
      );
      event.preventDefault();
    });
  }

  function onSelectSortableColumnHeader(
    linkClass,
    actionMethod,
    containerId,
    refreshFunction
  ) {
    $(linkClass).each(function(i, obj) {
      $(obj).click(function(event) {
        var sortColumn = $(obj).attr("data-sortColumn");
        var sortDirection = $(obj).attr("data-sortDirection");
        changeStandardsPartial(
          actionMethod,
          { sortColumn: sortColumn, sortDirection: sortDirection },
          containerId,
          refreshFunction
        );
        event.preventDefault();
      });
    });
  }

  function changeStandardsPartial(
    actionMethod,
    data,
    containerId,
    refreshFunction
  ) {
    $.ajax({
      url: "/" + controller + "/" + actionMethod,
      type: "get",
      data: data,
      success: function(response) {
        var jqContainer = $(containerId);
        jqContainer.html(response);
        refreshFunction();
      },
      error: function() {
        window.location = "/" + controller;
      }
    });
  }

  function initSearchPartial(data, containerId, searchPartialFunction) {
    $.ajax({
      url: "/" + controller + "/SearchPartial",
      type: "get",
      data: data,
      success: function(response) {
        var jqContainer = $(containerId);
        jqContainer.html(response);
        searchPartialFunction();
      },
      error: function() {
        window.location = "/" + controller;
      }
    });
  }

  function resetFiltersPartial(containerId, searchPartialFunction) {
    $.ajax({
      url: "/" + controller + "/ResetFiltersPartial",
      type: "get",
      data: null,
      success: function(response) {
        var jqContainer = $(containerId);
        jqContainer.html(response);
        searchPartialFunction();
      },
      error: function() {
        window.location = "/" + controller;
      }
    });
  }

  function applyFiltersPartial(data, containerId, searchPartialFunction) {
    $.ajax({
      type: "POST",
      contentType: "application/json; charset=utf-8",
      url: "/" + controller + "/ApplyFiltersPartial",
      data: JSON.stringify(data),
      success: function(response) {
        var jqContainer = $(containerId);
        jqContainer.html(response);
        searchPartialFunction();
      },
      error: function() {
        window.location = "/" + controller;
      }
    });
  }
})(jQuery);
