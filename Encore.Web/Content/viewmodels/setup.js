define(['knockout', 'webApiClient', 'messageBox', 'page', 'baseList'],
    function (ko, webApiClient, messageBox, page, baseList) {

        "use strict";

        var setupViewModel = {

            tableViewModel: new baseList.ListViewModel({
                columns: [
                    { name: "Name", value: "Name", sortDescending: false },
                    { name: "Type", value: "Type" },
                ],
                sortable: true,
                filterMode: 'search',
                pageSize: 10,
                emptyRowMessage: "No Sites found",
                url: "/setup/sites/",
                successCallback: function (model) {
                    messageBox.Hide();
                },
                errorCallback: function (errorResponse) {
                    messageBox.ShowError("Error retrieving Sites");
                }
            }, null),

            UploadSites: function (file) {
                var self = this;
                messageBox.Hide();
                webApiClient.ajaxUploadCsv("/setup/sites", file,
                function(model) {
                    messageBox.ShowSuccess(file.name + " uploaded sucessfully");
                },
                function(errorResponse) {
                    messageBox.ShowErrors("Upload Failed. ", errorResponse);
                });
            },

            SyncSummaries: function () {
                var self = this;
                messageBox.Hide();

                webApiClient.ajaxPut("/setup/", "/syncProjectSummaries", null, function (model) {
                    messageBox.ShowSuccess("Sync Details Requested Successfully");
                },
                function (errorResponse) {
                    messageBox.ShowErrors("Error requesting details:", errorResponse);
                });
            },
            
            SyncFields: function () {
                var self = this;
                messageBox.Hide();

                webApiClient.ajaxPut("/setup/", "/syncFields", null, function (model) {
                    messageBox.ShowSuccess("Sync Fields Completed Successfully");
                },
                function (errorResponse) {
                    messageBox.ShowErrors("Error requesting fields:", errorResponse);
                });
            },

            Initialise: function () {
                var self = this;

                ko.applyBindings(self, $("#setupActions")[0]);
            }
        };

        return setupViewModel;
    });

