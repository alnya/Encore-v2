require(['baseList', 'messageBox'],
    function (baseList, messageBox) {

        "use strict";

        var projectsViewModel = new baseList.ListViewModel({
            columns: [
                { name: "Project Name", value: "Name", sortDescending: false, template: "nameCellTemplate" }
            ],
            sortable: true,
            filterMode: 'search',
            pageSize: 10,
            emptyRowMessage: "No Projects found",
            url: "/projects/",
            successCallback: function (model) {
                messageBox.Hide();
            },
            errorCallback: function (errorResponse) {
                messageBox.ShowError("Error retrieving Projects");
            }
        },
        function () {
            location.href = "/pages/projects/add";
        });

        return projectsViewModel;
    });