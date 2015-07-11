define(['jquery', 'knockout', 'moment', 'daterangepicker'], function ($, ko, moment) {

    return {

        SERVER_DATETIME_FORMAT: "YYYY-MM-DDTHH:mm:ss.SSSZZZ",
        CLIENT_DATETIME_FORMAT: "YYYY/MM/DD HH:mm",
        CLIENT_DATE_FORMAT: "YYYY/MM/DD",

        parseQueryString: function (queryString) {
            var params = {}, queries, temp, i, l;

            queries = queryString.split("&");

            for (i = 0, l = queries.length; i < l; i++) {
                temp = queries[i].split('=');
                if (temp.length == 2) {
                    params[temp[0]] = temp[1];
                }
            };

            return params;
        },       

        setupControls:function()
        {
            this.setupDatePicker('.datePicker');
            this.setupDateRangePicker('.dateRange');
        },

        setupDateRangePicker: function (element, value) {
            $(element).daterangepicker({
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract('days', 1), moment().subtract('days', 1)],
                    'Last 7 Days': [moment().subtract('days', 6), moment()],
                    'Last 30 Days': [moment().subtract('days', 29), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month')]
                },
                startDate: moment(),
                endDate: moment()
            },
            function (start, end) {
                $(element).find('input').val(start.format('D MMM YYYY') + ' - ' + end.format('D MMM YYYY')).change();
            });
            if (value) {
                var dates = value.split(' - ');
                if (dates.length == 2) {
                    var dateFrom = moment(dates[0]), dateTo = moment(dates[1]);
                    if (dateFrom.isValid() && dateTo.isValid()) {
                        $(element).data('daterangepicker').setStartDate(dateFrom);
                        $(element).data('daterangepicker').setEndDate(dateTo);
                    }
                }
            }
        },

        setupDatePicker: function(element, value) {
            $(element).daterangepicker({ singleDatePicker: true },
                   function (start, end) { $(element).find('input').val(start.format('D MMM YYYY')).change(); });
            if (value) {
                var date = moment(value);
                if (date.isValid()) {
                    $(element).data('daterangepicker').setStartDate(date);
                    $(element).data('daterangepicker').setEndDate(date);
                }
            }
        }
    }
});
