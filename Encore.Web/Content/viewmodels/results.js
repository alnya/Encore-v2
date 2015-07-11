require(['knockout', 'webApiClient', 'messageBox', 'page'],
    function (ko, webApiClient, messageBox, page) {

        "use strict";

        var resultsViewModel = new function () {

            self = this;

            self.items = ko.observableArray([]);
            self.emptyRowMessage = "No Report Results Found"
            self.emptyRowText = ko.observable("");

            self.reportName = ko.observable("");
            self.columns = ko.observableArray([])
            self.totalPages = ko.observable(0);
            self.pageIndex = ko.observable(0);
            self.pageSize = ko.observable(10);
            self.pageRadius = ko.observable(2);
            self.isFirstPage = ko.pureComputed(function () { return self.pageIndex() === 0; });
            self.isLastPage = ko.pureComputed(function () { return self.pageIndex() === self.totalPages() - 1; });

            self.downloadUrl = ko.pureComputed(function () {
                return webApiClient.baseUrl + "reports/results/" + page.RecordId + "/download/" + self.reportName().replace(/\s/g, "-") + ".csv"
            });

            self.pages = ko.pureComputed(function () {
                var pages = [];
                var page, elem, last;
                for (page = 1; page <= self.totalPages() ; page++) {
                    var activePage = self.pageIndex() + 1;
                    var totalPage = self.totalPages();
                    var radius = self.pageRadius();
                    if (page == 1 || page == totalPage) {
                        elem = page;
                    } else if (activePage < 2 * radius + 1) {
                        elem = (page <= 2 * radius + 1) ? page : "ellipsis";
                    } else if (activePage > totalPage - 2 * radius) {
                        elem = (totalPage - 2 * radius <= page) ? page : "ellipsis";
                    } else {
                        elem = (Math.abs(activePage - page) <= radius ? page : "ellipsis");
                    }
                    if (elem != "ellipsis" || last != "ellipsis") {
                        pages.push(elem);
                    }
                    last = elem;
                }
                return pages;
            });

            self.prevPage = function () {
                if (self.pageIndex() > 0) {
                    self.pageIndex(self.pageIndex() - 1);
                }
            };

            self.nextPage = function () {
                if (self.pageIndex() < self.totalPages() - 1) {
                    self.pageIndex(self.pageIndex() + 1);
                }
            };

            self.moveToPage = function (index) {
                self.pageIndex(index - 1);
            };

            self.pageIndex.subscribe(function () {
                self.loadData();
            });

            self.pageSize.subscribe(function () {
                self.loadData();
            });

            self.loadData = function () {
                messageBox.Hide();
                self.emptyRowText("Loading Result Data");

                var params = "PageSize=" + self.pageSize() + '&Page=' + self.pageIndex() + 1;

                webApiClient.ajaxGet("reports/results/" + page.RecordId, null, params,
                    function (data) {
                        if (data) {
                            self.reportName(data.ReportName);
                            self.columns(data.Columns);
                            self.items(data.Rows);
                            self.emptyRowText(self.emptyRowMessage);
                            self.totalPages(data.Pages);
                            self.pageIndex(Math.min(self.pageIndex(), self.totalPages() - 1));
                        }
                    },
                    function (errorResponse) {
                        messageBox.ShowErrors("Error loading results:", errorResponse);
                    });
            };

            self.Initialize = function(){
                self.loadData();
            }
        }
            
        resultsViewModel.Initialize();
        ko.applyBindings(resultsViewModel, $("#resultsTable")[0]);

        return resultsViewModel;
    });