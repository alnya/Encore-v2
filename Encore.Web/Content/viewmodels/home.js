require(['knockout', 'webApiClient', 'messageBox', 'page', 'moment', 'common'],
    function (ko, webApiClient, messageBox, page, moment, common) {

        "use strict";

        var homeViewModel = new function(){
            var self = this;
            self.recentResults = ko.observableArray([]);
            self.GetData = function () {
                messageBox.Hide();
                webApiClient.ajaxGet("reports/myResults", null, null,
                    function (data) {
                        if (data) {
                            var resultRows = [];

                            ko.utils.arrayForEach(data, function (item) {
                                resultRows.push({
                                    ReportName: item.ReportName,
                                    ResultId: item.Id,
                                    RequestDate: moment(item.RunDate).format(common.CLIENT_DATETIME_FORMAT)
                                })
                            });

                            self.recentResults(resultRows);
                        }
                    },
                    function (errorResponse) {
                        messageBox.ShowErrors("Error loading results:", errorResponse);
                    });
            }
        };

        homeViewModel.GetData();
        ko.applyBindings(homeViewModel, $("#homePage")[0]);

        return homeViewModel;
    });

