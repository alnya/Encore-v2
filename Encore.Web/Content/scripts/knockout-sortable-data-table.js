/**
 * Knockout bootstrap pageable and sortable data table
 * Based on: https://github.com/labory/knockout-bootstrap-sortable-data-table
 */
define(['knockout', 'common', 'webApiClient', 'moment'], function (ko, common, webApiClient, moment) {

    function TableColumn(column) {
        var self = this;

        self.name = column.name || "";
        self.DefaultOptionsCaption = "Any " + column.name;
        self.OptionsCaption = ko.observable(column.OptionsCaption || "Any " + column.name);
        self.MultiSelect = column.MultiSelect || false;
        self.value = column.value;
        self.sortValue = column.sortValue || column.value;
        self.template = column.template;
        self.sortable = column.sortable;
        self.filterable = column.filterable;
        self.dataType = column.dataType || "String";
        self.filterValues = ko.observableArray(column.filterValues || []);
        self.width = column.width;
        self.sortDescending = ko.observable(column.sortDescending);
        self.visible = ko.observable((typeof column.visible === 'undefined') ? true : column.visible);
        self.filterValue = ko.observable(column.filterValue);
        self.filterKey = column.filterKey || column.value;
        self.labelText = column.labelText || "";
        self.LookupUrl = column.LookupUrl || "";
    };

    var gridViewModel = function(config) {
        var self = this;

        self.bindFilterToQuery = (typeof config.bindFilterToQuery === 'undefined') ? true : config.bindFilterToQuery;
        self.autoLoad = (typeof config.autoLoad === 'undefined') ? true: config.autoLoad;
        self.sortable = config.sortable || false;
        self.filterMode = config.filterMode || 'none';
        self.items = ko.observableArray(config.items || []);
        self.emptyRowMessage = config.emptyRowMessage;
        self.emptyRowText = ko.observable("");
        self.columns = [];
        self.nonTableFilterableFields = [];
        self.allFilterableFields = [];
        self.sortColumn = ko.observable(null);
        self.url = config.url;
        self.successCallback = config.successCallback;
        self.errorCallback = config.errorCallback;
        self.totalPages = ko.observable();
        self.pageIndex = ko.observable(0);
        self.pageSize = ko.observable(config.pageSize || 10);
        self.pageRadius = ko.observable(config.pageRadius || 2);
        self.isFirstPage = ko.pureComputed(function() { return self.pageIndex() === 0; });
        self.isLastPage = ko.pureComputed(function () { return self.pageIndex() === self.totalPages() - 1; });
        self.downloadEnabled = config.downloadEnabled || false;
        self.downloadUrl = ko.observable();

        var filterTimer;

        if (self.bindFilterToQuery) {
            self.queryParams = config.queryParams || common.parseQueryString(location.search.substring(1));
        } else {
            self.queryParams = config.queryParams || {};
        }

        if (self.queryParams.Page) {
            self.pageIndex(self.queryParams.Page - 1);
        } else{
            self.queryParams.Page = self.pageIndex() + 1;
        }

        if (self.queryParams.Pagesize) {
            self.pageSize(self.queryParams.Pagesize);
        } else {
            self.queryParams.Pagesize = self.pageSize();
        }

        if (self.queryParams.ReturnFields) {
            var returnFields = self.queryParams.ReturnFields.toString().split(',');
        };

        var setConfigFilterFromQuery = function(column) {
            if (self.queryParams[column.value] || self.queryParams[column.value + ":from"]) {
                if (column.dataType == "DateRange") {
                    var dateFrom = moment(self.queryParams[column.value + ":from"]), dateTo = moment(self.queryParams[column.value + ":to"]);
                    if (dateFrom.isValid() && dateTo.isValid()) {
                        column.filterValue = dateFrom.format('DD MMM YYYY') + " - " + dateTo.format('DD MMM YYYY');
                    }
                } else if (column.dataType == "Date") {
                    var date = moment(self.queryParams[column.value]);
                    if (date.isValid()) {
                        column.filterValue = date.format('DD MMM YYYY');
                    }
                } else {
                    column.filterValue = self.queryParams[column.value];
                }
            }
        }

        ko.utils.arrayForEach(config.columns, function(column) {

            column.sortable = (typeof column.sortable === 'undefined') ? true : column.sortable;
            column.sortable = column.sortable && self.sortable;
            column.filterable = (typeof column.filterable === 'undefined') ? true : column.filterable;

            var sortBy = column.sortValue || column.value;

            if (self.queryParams.SortBy) {
                if (self.queryParams.SortBy == sortBy) {
                    column.sortDescending = (self.queryParams.SortDescending === 'true');
                }
            } else if (typeof column.sortDescending !== 'undefined') {
                self.queryParams.SortBy = sortBy;
                self.queryParams.SortDescending = column.sortDescending;
            };

            setConfigFilterFromQuery(column);

            if (returnFields) {
                column.visible = returnFields.indexOf(column.value) > -1;
            }

            var tableColumn = new TableColumn(column);
            self.columns.push(tableColumn);

            if (tableColumn.sortDescending() != null) {
                self.sortColumn(tableColumn);
            }

            if (tableColumn.filterable) {
                tableColumn.filterValue.subscribe(function() {
                    if (self.filterMode == 'filter') {
                        clearTimeout(filterTimer);
                        filterTimer = setTimeout(function() {
                            self.reload();
                        }, 1000);
                    }
                });
            };
        });

        self.visibleColumns = ko.computed(function () {
            return ko.utils.arrayFilter(self.columns, function (column) {
                return column.visible();
            });
        }, self);

        if (config.nonTableFilterableFields) {
            ko.utils.arrayForEach(config.nonTableFilterableFields, function(field) {

                setConfigFilterFromQuery(field);

                var filterField = new TableColumn(field);
                filterField.filterable = true;

                self.nonTableFilterableFields.push(filterField);
            });
        }

        ko.utils.arrayForEach(self.columns, function(field) {
            if (field.filterable) {
                self.allFilterableFields.push(field);
            };
        });

        self.allFilterableFields = self.allFilterableFields.concat(self.nonTableFilterableFields);

        // TODO - standard comparison on type
        self.comparator = config.comparator || function(a, b) {
            return a && b && a.id && b.id ? a.id === b.id : a === b;
        };
        
        self.pages = ko.pureComputed(function() {
            var pages = [];
            var page, elem, last;
            for (page = 1; page <= self.totalPages(); page++) {
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

        self.prevPage = function() {
            if (self.pageIndex() > 0) {
                self.pageIndex(self.pageIndex() - 1);
            }
        };

        self.nextPage = function() {
            if (self.pageIndex() < self.totalPages() - 1) {
                self.pageIndex(self.pageIndex() + 1);
            }
        };

        self.moveToPage = function(index) {
            self.pageIndex(index - 1);
        };
        
        var buildQueryString = function() {

            var paramString = '';

            for (var property in self.queryParams) {
                if (self.queryParams.hasOwnProperty(property)) {
                    if (paramString != '')
                        paramString += '&';
                    paramString += property + '=' + self.queryParams[property];
                }
            }

            return paramString;
        }

        var setQueryString = function (query) {

            if (self.bindFilterToQuery && query != "") {
                history.replaceState(window.history.state, document.title, window.location.href.split('?')[0] + "?" + query);
            }
            self.downloadUrl(webApiClient.baseUrl +self.url + '/extract.csv?' +query);
        }

        self.visibleColumns.subscribe(function() { 

            if (self.visibleColumns().length < self.columns.length) {
                var returnFieldNames = [];
                ko.utils.arrayForEach(self.visibleColumns(), function (column) {
                    returnFieldNames.push(column.value);
                });
                self.queryParams.ReturnFields = returnFieldNames.join(',');
            } else {
                delete self.queryParams.ReturnFields;
            }

            setQueryString(buildQueryString());
        });

        var rebuildQueryParams = function () {

            var queryReturnFields = self.queryParams.ReturnFields;
            self.queryParams = {};

            if (queryReturnFields) {
                self.queryParams.ReturnFields = queryReturnFields;
            };

            self.queryParams.Page = self.pageIndex() + 1;
            self.queryParams.Pagesize = self.pageSize();

            var sortColumn = self.sortColumn();

            if ((sortColumn != null && sortColumn.sortDescending() != null)) {

                self.queryParams.SortBy = sortColumn.sortValue;
                self.queryParams.SortDescending = sortColumn.sortDescending();
            }

            ko.utils.arrayForEach(self.filterTerms(), function(term) {
                if (term.filterValue !== "") {
                    if (term.dataType == "DateRange") {
                        var dates = term.filterValue.split(' - ');
                        if (dates.length == 2) {
                            var dateFrom = moment(dates[0]), dateTo = moment(dates[1]);
                            if (dateFrom.isValid() && dateTo.isValid()) {
                                self.queryParams[term.fieldName + ':from'] = dateFrom.format('YYYY-MM-DD');
                                self.queryParams[term.fieldName + ':to'] = dateTo.format('YYYY-MM-DD');
                            } else {
                                self.queryParams[term.fieldName] = term.filterValue;
                            }
                        }
                    } else if (term.dataType == "Date") {
                        var dateValue = moment(term.filterValue);
                        if (dateValue.isValid()) {
                            self.queryParams[term.fieldName] = dateValue.format('YYYY-MM-DD');
                        } else {
                            self.queryParams[term.fieldName] = term.filterValue;
                        }
                    } else if (term.filterValue.constructor === Array && term.filterValue.length > 0) {
                        self.queryParams[term.fieldName + ':in'] = term.filterValue;
                    } else {
                        self.queryParams[term.fieldName] = term.filterValue;
                    }
                }
            });
        }
        
        self.reload = function() {
            rebuildQueryParams();
            loadData();
        };

        var loadData = function () {
            self.emptyRowText("Loading Data");

            var params = buildQueryString();
            setQueryString(params);

            webApiClient.ajaxGet(self.url, null, params,
                function (data) {
                    if (data) {
                        self.items(data.Results);
                        self.emptyRowText(self.emptyRowMessage);
                        self.totalPages(data.Pages);
                        self.pageIndex(Math.min(self.pageIndex(), self.totalPages() - 1));
                        self.filterApplied(self.filterTerms().length > 0);

                        if (self.successCallback != null) {
                            self.successCallback(data);
                        }
                    }
                },
                function (errorResponse) {
                    if (self.errorCallback != null) {
                        self.errorCallback(errorResponse);
                    }
                });
        };

        var sortTimer;
        self.sort = function(column) {
            clearTimeout(sortTimer);
            column.sortDescending(column.sortDescending() || false);

            if (self.sortColumn() === column) {
                column.sortDescending(!column.sortDescending());
            }

            self.sortColumn(column);

            sortTimer = setTimeout(function() {
                self.reload();
            }, 250);
        };

        self.filterApplied = ko.observable(false);

        self.filterTerms = function () {
            var terms = [];
            ko.utils.arrayForEach(self.allFilterableFields, function (column) {

                if (column.filterable && column.filterValue() !== "" && (typeof column.filterValue() !== 'undefined')) {
                    terms.push({
                        dataType: column.dataType,
                        fieldName: column.filterKey,
                        filterValue: column.filterValue()
                    });
                }
            });

            return terms;
        };

        self.clearFilter = function() {
            ko.utils.arrayForEach(self.allFilterableFields, function (column) {
                column.filterValue(undefined);
            });

            if (self.autoLoad) {
                self.reload();
            } else {
                self.items([]);
                self.emptyRowText("");
                self.totalPages(0);
                self.filterApplied(false);
            }
        };

        self.pageIndex.subscribe(function() {
             self.reload();
        });

        self.pageSize.subscribe(function() {
             self.reload();
        });

        self.formatDate = function (value, parent) {
            return moment(typeof value == 'function' ? value(parent) : parent[value]).isValid() ?
                moment(typeof value == 'function' ? value(parent) : parent[value], common.SERVER_DATETIME_FORMAT).format(common.CLIENT_DATETIME_FORMAT) : '';
        };

        if (self.autoLoad) {
            loadData();
        }
    }

    var templateEngine = new ko.nativeTemplateEngine();

    ko.bindingHandlers.dataTable = {
        init: function(element, valueAccessor) {
            return { 'controlsDescendantBindings': true };
        },
        update: function(element, valueAccessor, allBindingsAccessor) {
            var viewModel = valueAccessor();
            ko.renderTemplate("ko-table-template", viewModel, { templateEngine: templateEngine }, element, "replaceNode");
        }
    };

    return { GridViewModel: gridViewModel };
});
