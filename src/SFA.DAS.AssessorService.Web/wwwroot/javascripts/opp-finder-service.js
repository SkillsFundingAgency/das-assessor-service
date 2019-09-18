(function ($) {    
    refreshClickHandlers();
    $('#search').click(function (event) {
        var searchTerm = $('#searchTerm').val();
        initSearchPartial({ searchTerm: searchTerm }, searchPartial);
        event.preventDefault();
    });

    function searchPartial() {
        changeStandardsPartial('ChangePageSetApprovedStandardsPartial', { pageSetIndex: 1 }, '#approved-standards', refreshApprovedClickHandlers);
        changeStandardsPartial('ChangePageSetInDevelopmentStandardsPartial', { pageSetIndex: 1 }, '#in-development-standards', refreshInDevelopmentClickHandlers);
        changeStandardsPartial('ChangePageSetProposedStandardsPartial', { pageSetIndex: 1 }, '#proposed-standards', refreshProposedClickHandlers);
    }

    function refreshClickHandlers() {
        refreshApprovedClickHandlers();
        refreshInDevelopmentClickHandlers();
        refreshProposedClickHandlers();
    }

    function refreshApprovedClickHandlers() {
        onClickChangePageSet('.approved-page-set', 'ChangePageSetApprovedStandardsPartial', '#approved-standards', refreshApprovedClickHandlers);
        onClickChangePage('.approved-page', 'ChangePageApprovedStandardsPartial', '#approved-standards', refreshApprovedClickHandlers);
        onSelectStandardsPerPage('.approved-per-page', 'ShowApprovedStandardsPerPagePartial', '#approved-standards', refreshApprovedClickHandlers);
        onSelectSortableColumnHeader('.approved-sort', 'SortApprovedStandardsPartial', '#approved-standards', refreshApprovedClickHandlers);
    }

    function refreshInDevelopmentClickHandlers() {
        onClickChangePageSet('.in-development-page-set', 'ChangePageSetInDevelopmentStandardsPartial', '#in-development-standards', refreshInDevelopmentClickHandlers);
        onClickChangePage('.in-development-page', 'ChangePageInDevelopmentStandardsPartial', '#in-development-standards', refreshInDevelopmentClickHandlers);
        onSelectStandardsPerPage('.in-development-per-page', 'ShowInDevelopmentStandardsPerPagePartial', '#in-development-standards', refreshInDevelopmentClickHandlers);
        onSelectSortableColumnHeader('.in-development-sort', 'SortInDevelopmentStandardsPartial', '#in-development-standards', refreshInDevelopmentClickHandlers);
    }

    function refreshProposedClickHandlers() {
        onClickChangePageSet('.proposed-page-set', 'ChangePageSetProposedStandardsPartial', '#proposed-standards', refreshProposedClickHandlers);
        onClickChangePage('.proposed-page', 'ChangePageProposedStandardsPartial', '#proposed-standards', refreshProposedClickHandlers);
        onSelectStandardsPerPage('.proposed-per-page', 'ShowProposedStandardsPerPagePartial', '#proposed-standards', refreshProposedClickHandlers);
        onSelectSortableColumnHeader('.proposed-sort', 'SortProposedStandardsPartial', '#proposed-standards', refreshProposedClickHandlers);
    }

    function onClickChangePageSet(linkClass, actionMethod, containerId, refreshFunction) {
        $(linkClass).each(function (i, obj) {
            $(obj).click(function (event) {
                var pageSetIndex = $(obj).attr('data-pageSetIndex');
                changeStandardsPartial(actionMethod, { pageSetIndex: pageSetIndex }, containerId, refreshFunction);
                event.preventDefault();
            });
        });
    }

    function onClickChangePage(linkClass, actionMethod, containerId, refreshFunction) {
        $(linkClass).each(function (i, obj) {
            $(obj).click(function (event) {
                var pageIndex = $(obj).attr('data-pageIndex');
                changeStandardsPartial(actionMethod, { pageIndex: pageIndex }, containerId, refreshFunction);
                event.preventDefault();
            });
        });
    }

    function onSelectStandardsPerPage(selectClass, actionMethod, containerId, refreshFunction) {
        $(selectClass).change(function () {
            var selectedVal = $(this).find(':selected').val();
            changeStandardsPartial(actionMethod, { standardsPerPage: selectedVal }, containerId, refreshFunction);
            event.preventDefault();
        });
    }

    function onSelectSortableColumnHeader(linkClass, actionMethod, containerId, refreshFunction) {
        $(linkClass).each(function (i, obj) {
            $(obj).click(function (event) {
                var sortColumn = $(obj).attr('data-sortColumn');
                var sortDirection = $(obj).attr('data-sortDirection');
                changeStandardsPartial(actionMethod, { sortColumn: sortColumn, sortDirection: sortDirection }, containerId, refreshFunction);
                event.preventDefault();
            });
        });
    }

    function changeStandardsPartial(actionMethod, data, containerId, refreshFunction) {
        $.ajax({
            url: "/OppFinder/" + actionMethod,
            type: "get",
            data: data,
            success: function (response) {
                var jqContainer = $(containerId);
                jqContainer.html(response);
                refreshFunction();
            },
            error: function (xhr) {
            }
        });
    }

    function initSearchPartial(data, searchPartialFunction) {
        $.ajax({
            url: "/OppFinder/SearchPartial",
            type: "get",
            data: data,
            success: function (response) {
                searchPartialFunction();
            },
            error: function (xhr) {
            }
        });
    }

})(jQuery);
