﻿$(document).ready(function () {
    ticketCenter.filters.setupFilterForm();
    ticketCenter.makeClicky();
});

var ticketCenter = (function (tc) {
    tc.makeClicky = function () {
        $(".clickable").clickable();
    }

    tc.completeChangeList = function () {
        // Animate
        $('#ticketList').fadeIn(100);
        tc.makeClicky();
        tc.filters.setupFilterForm();
    }

    return tc;

})(ticketCenter || {});


ticketCenter.paging = (function () {
    var beginChangePage = function (args) {

        $('#ticketList').fadeOut(100);
    };

    return { beginChangePage: beginChangePage }
})();

//sub-module for filterbar features
ticketCenter.filters = (function () {
    var beginChangeFilter = function (args) {

        $('#ticketList').fadeOut(100);
    };

    var setupFilterForm = function () {
        $('.filterBar select').addClass('form-control');
        $('.pagination').addClass('pagination-sm');
        $("#filterSubmitButton").hide();
        $('select.postback').change(function () {
            $('#filterSubmitButton').trigger('click');
        });
    };

    return {
        beginChangeFilter: beginChangeFilter,
        setupFilterForm: setupFilterForm
    }
})();


//sub-module for sort features
ticketCenter.sorts = (function () {
    var shiftstatus = false;


    var setShiftStatus = function (e) {
        if (e) {
            shiftstatus = e.shiftKey;
        }
    };

    var beginChangeSort = function (event, args) {

        if (shiftstatus) {
            args.url = args.url + "&isMultiSort=true";
        }
        $('#ticketList').fadeOut(100);
    };


    return {
        setShiftStatus: setShiftStatus,
        beginChangeSort: beginChangeSort,
    }
})();