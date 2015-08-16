define(['knockout', 'webApiClient', 'messageBox', 'page', 'moment', 'common'],
    function (ko, webApiClient, messageBox, page, moment, common) {

        "use strict";

        var projectViewModel = ko.validatedObservable({

            EntityName: "Project",
            Url: "/projects/",
            CanDeleteEntity: true,
            CanEditEntity: true,
            Name: ko.observable().extend({ maxLength: 100, required: true }),
            Description: ko.observable().extend({ maxLength: 400, required: false }),
            ApiUrl: ko.observable().extend({ maxLength: 400, required: false }),
            FieldPrefix: ko.observable().extend({ maxLength: 20, required: false }),

            SiteDataCount: ko.observable(),
            FieldDataCount: ko.observable(),
            SummaryDataCount: ko.observable(),
            DataLastUpdated: ko.observable(),

            SetModel: function (objFromServer) {
                var self = this;
                if (!objFromServer) return;

                self.Name(objFromServer.Name);
                self.Description(objFromServer.Description);
                self.ApiUrl(objFromServer.ApiUrl);
                self.FieldPrefix(objFromServer.FieldPrefix);

                self.SiteDataCount(objFromServer.SiteDataCount);
                self.FieldDataCount(objFromServer.FieldDataCount);
                self.SummaryDataCount(objFromServer.SummaryDataCount);

                if (objFromServer.DataLastUpdated) {
                    self.DataLastUpdated(moment(objFromServer.DataLastUpdated).format(common.CLIENT_DATE_FORMAT));
                }
                else {
                    self.DataLastUpdated("Never");
                }
            },

            GetEntityModel: function () {
                var self = this;

                return {
                    Name: self.Name(),
                    Description: self.Description(),
                    ApiUrl: self.ApiUrl(),
                    FieldPrefix: self.FieldPrefix()
                };
            },

            TestUrl: function () {
                var self = this;

                messageBox.Hide();

                var entityModel = self.GetEntityModel();

                webApiClient.ajaxPut(self.Url, "testUrl", ko.toJSON(entityModel), function (model) {
                    if (model === true) {
                        messageBox.ShowSuccess("URL Contacted Successfully");
                    }
                    else {
                        messageBox.ShowErrors("Unable to contact URL");
                    }
                },
                function (errorResponse) {
                    messageBox.ShowErrors("Error testing URL:", errorResponse);
                });
            },

            SyncDetails: function () {
                var self = this;

                var entityId = page.RecordId;

                if (entityId != 'add') {

                    messageBox.Hide();

                    webApiClient.ajaxPut(self.Url, entityId + "/syncDetails", null, function (model) {
                        messageBox.ShowSuccess("Sync Details Requested Successfully");
                    },
                    function (errorResponse) {
                        messageBox.ShowErrors("Error requesting details:", errorResponse);
                    });
                }
            }
        });

        return projectViewModel;
    });

