require(['baseList', 'messageBox', 'webApiClient'],
    function (baseList, messageBox, webApiClient) {

        "use strict";

        var reportsViewModel = new baseList.ListViewModel({
            columns: [
                { name: "Report Name", value: "Name", sortDescending: false, template: "nameCellTemplate" },
                { name: "Request Status", value: "LastRequestStatus", dataType: "List", filterValues: ['Pending', 'InProgress', 'Failed', 'Complete'] },
                { name: "Last Requested", value: "LastRequested", filterable: false, dataType: "Date" },
                { name: "Action", template: "requestButtonCellTemplate", filterable: false, sortable: false }
            ],
            sortable: true,
            filterMode: 'search',
            pageSize: 10,
            emptyRowMessage: "No Reports found",
            url: "/reports/",
            successCallback: function (model) {
                messageBox.Hide();
            },
            errorCallback: function (errorResponse) {
                messageBox.ShowError("Error retrieving Reports");
            }
        },
        function () {
            location.href = "/pages/reports/add";
        });

        reportsViewModel.tableViewModel.RequestReport = function (id) {
            webApiClient.ajaxPut("/reports/", id + "/request", null, function () {
                messageBox.ShowSuccess("Report Requested Successfully");
            },
            function (errorResponse) {
                messageBox.ShowErrors("Report Request Failed:", errorResponse);
            });
        };

        return reportsViewModel;
    });