require(['baseList', 'messageBox'],
    function (baseList, messageBox) {

        "use strict";

        var usersViewModel = new baseList.ListViewModel({
            columns: [
                { name: "Name", value: "Name", sortDescending: false, template: "nameCellTemplate" },
                { name: "Email", value: "Email" },
                { name: "Role", value: "UserRole", filterable: false }
            ],
            sortable: true,
            filterMode: 'search',
            pageSize: 10,
            emptyRowMessage: "No Users found",
            url: "/users/",
            successCallback: function (model) {
                messageBox.Hide();
            },
            errorCallback: function (errorResponse) {
                messageBox.ShowError("Error retrieving Users");
            }
        },
        function () {
            location.href = "/pages/users/add";
        });

        return usersViewModel;
    });