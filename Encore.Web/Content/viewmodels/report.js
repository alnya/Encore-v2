define(['knockout', 'webApiClient', 'messageBox', 'page', 'moment', 'daterangepicker', 'common'],
    function (ko, webApiClient, messageBox, page, moment, daterangepicker, common) {

        "use strict";
              
        var Section = function (name, selected) {
            this.name = name;
            this.isSelected = ko.computed(function () {
                return this === selected();
            }, this);
        };

        var Leaf = function (id, name) {
            this.name = ko.observable(name);
            this.id = ko.observable(id);
            this.children = ko.observableArray([]);
        };

        var reportViewModel = new function () {
            var self = this;

            self.siteTypes = ko.observableArray([]);
            self.fieldTypes = ko.observableArray([]);
            self.projects = ko.observableArray([]);
            self.fieldData = ko.observableArray([]);
            self.siteData = ko.observableArray([]);
            self.summaryData = ko.observable({});

            self.selectedFields = ko.observableArray([]);
            self.selectedSites = ko.observableArray([]);
            self.selectedDateRange = ko.observable('');

            self.Name = ko.observable('').extend({ maxLength: 100, required: true });
            self.Mode = ko.observable('VIEW');
            self.IsLoaded = ko.observable(false);

            self.SiteSearch = ko.observable('');
            self.FieldSearch = ko.observable('');

            self.rowCount = ko.computed(function () {
                return self.selectedFields().length > 0 && self.selectedSites().length ? self.summaryData().rows : 0;
            }, this);
            self.dateFrom = ko.computed(function () {
                return self.selectedFields().length > 0 && self.selectedSites().length && self.summaryData().rows > 0 ? moment(self.summaryData().dateFrom).format(common.CLIENT_DATE_FORMAT) : '';
            }, this);
            self.dateTo = ko.computed(function () {
                return self.selectedFields().length > 0 && self.selectedSites().length && self.summaryData().rows > 0 ? moment(self.summaryData().dateTo).format(common.CLIENT_DATE_FORMAT) : '';
            }, this);

            self.selectedSiteFilter = ko.observable();
            self.selectedFieldFilter = ko.observable();

            self.siteFilters = ko.observableArray([
                new Section('All', self.selectedSiteFilter),
                new Section('Project', self.selectedSiteFilter),
                new Section('Type', self.selectedSiteFilter),
                new Section('Search', self.selectedSiteFilter),
                new Section('Fields', self.selectedSiteFilter)
            ]);
            self.fieldFilters = ko.observableArray([
                new Section('All', self.selectedFieldFilter),
                new Section('Project', self.selectedFieldFilter),
                new Section('Type', self.selectedFieldFilter),
                new Section('Search', self.selectedFieldFilter),
                new Section('Sites', self.selectedFieldFilter)
            ]);

            self.siteSearchVisible = ko.computed(function () {
                return self.selectedSiteFilter() == self.siteFilters()[3];
            }, this);

            self.fieldSearchVisible = ko.computed(function () {
                return self.selectedFieldFilter() == self.fieldFilters()[3];
            }, this);

            self.selectedSiteFilter(self.siteFilters()[0]);
            self.selectedFieldFilter(self.fieldFilters()[0]);

            self.ShowSiteInfo = function () {
                self.infoSite(this.id());
                self.infoField('');
                $('#infoModal').modal('show');
            },

            self.ShowFieldInfo = function () {
                self.infoSite('');
                self.infoField(this.id());
                $('#infoModal').modal('show');
            },

            self.PreFilterArrayByNotSelected = function (selected, array) {
                return ko.utils.arrayFilter(array, function (item) {
                    var found = false;
                    ko.utils.arrayForEach(selected, function (s) {
                        if (s.id == item.id)
                            found = true;
                    });
                    return !found;
                })
            };

            self.Reset = function () {
                self.selectedFields([]);
                self.selectedSites([]);
                self.SetDefaultDateRange();
                self.selectedFieldFilter(self.fieldFilters()[0]);
                self.selectedSiteFilter(self.siteFilters()[0]);
            };

            self.FilterArrayByType = function (type, array, selected) {
                array = self.PreFilterArrayByNotSelected(selected, array);
                return ko.utils.arrayFilter(array, function (item) {
                    return item.type == type;
                })
            };

            self.FilterArrayByProject = function (project, array, selected) {
                array = self.PreFilterArrayByNotSelected(selected, array);
                return ko.utils.arrayFilter(array, function (item) {
                    var found = false;
                    ko.utils.arrayForEach(item.projects, function (p) {
                        if (p == project)
                            found = true;
                    });
                    return found;
                });
            };

            self.FilterArrayBySearch = function (filter, array, selected) {
                filter = filter.toLowerCase();
                array = self.PreFilterArrayByNotSelected(selected, array);
                return ko.utils.arrayFilter(array, function (item) {
                    return (
                        item.name.toLowerCase().indexOf(filter) > -1 ||
                        item.type.toLowerCase().indexOf(filter) > -1 ||
                        (ko.utils.arrayFirst(item.altNames, function (altName) {
                            return altName.toLowerCase().indexOf(filter) > -1;
                        }) != null));
                })
            };

            self.FilterArrayBySites = function (array, selected) {
                array = self.PreFilterArrayByNotSelected(selected, array);
                return ko.utils.arrayFilter(array, function (item) {
                    var found = false;
                    ko.utils.arrayForEach(item.siteIds, function (siteId) {
                        ko.utils.arrayForEach(self.selectedSites(), function (ss) {
                            if (ss.id == siteId)
                                found = true;
                        });
                    });
                    return found;
                });
            };

            self.FilterArrayByFields = function (array, selected) {
                array = self.PreFilterArrayByNotSelected(selected, array);
                return ko.utils.arrayFilter(array, function (item) {
                    var found = false;
                    ko.utils.arrayForEach(item.fieldIds, function (fieldId) {
                        ko.utils.arrayForEach(self.selectedFields(), function (ff) {
                            if (ff.id == fieldId)
                                found = true;
                        });
                    });
                    return found;
                });
            };

            self.addToSelectedFields = function () {
                var id = this.id();
                var field = ko.utils.arrayFirst(self.fieldData(), function (field) {
                    return field.id == id;
                });

                if (field != null){
                    self.selectedFields.push(field);
                    self.selectedSiteFilter(self.siteFilters()[4]);
                }
            };

            self.removeFromSelectedFields = function () {
                var id = this.id;
                var field = ko.utils.arrayFirst(self.fieldData(), function (field) {
                    return field.id == id;
                });
                self.selectedFields.remove(field);
            };

            self.addToSelectedSites = function () {
                var id = this.id();
                var site = ko.utils.arrayFirst(self.siteData(), function (site) {
                    return site.id == id;
                });

                if (site != null) {
                    self.selectedSites.push(site);
                    self.selectedFieldFilter(self.fieldFilters()[4]);
                }
            };

            self.removeFromSelectedSites = function () {
                var id = this.id;
                var field = ko.utils.arrayFirst(self.siteData(), function (field) {
                    return field.id == id;
                });
                self.selectedSites.remove(field);
            };

            self.infoSite = ko.observable();

            self.infoField = ko.observable();

            self.infoNodes = ko.computed(function () {
                var result = [];
                if (self.infoSite() != '') {
                    ko.utils.arrayForEach(self.fieldTypes(), function (type) {
                        var pleaf = new Leaf(null, type);
                        var array = ko.utils.arrayFilter(self.fieldData(), function (item) {
                            return item.type == type &&
                                ko.utils.arrayFirst(item.siteIds, function (siteId) {
                                    return siteId == self.infoSite();
                                }) != null;
                        });
                        ko.utils.arrayForEach(array, function (site) {
                            pleaf.children().push(new Leaf(site.id, site.name))
                        });
                        result.push(pleaf);
                    });
                }
                if (self.infoField() != '') {
                    ko.utils.arrayForEach(self.siteTypes(), function (type) {
                        var pleaf = new Leaf(null, type);
                        var array = ko.utils.arrayFilter(self.siteData(), function (item) {
                            return item.type == type &&
                                ko.utils.arrayFirst(item.fieldIds, function (fieldId) {
                                    return fieldId == self.infoField();
                                }) != null;
                        });
                        ko.utils.arrayForEach(array, function (site) {
                            pleaf.children().push(new Leaf(site.id, site.name))
                        });
                        result.push(pleaf);
                    });
                }
                return result;
            });

            self.sites = ko.computed(function () {
                if (self.selectedSiteFilter() == self.siteFilters()[0]) {
                    var result = new Leaf(null, 'All Sites');
                    var array = self.PreFilterArrayByNotSelected(self.selectedSites(), self.siteData());
                    ko.utils.arrayForEach(array, function (site) {
                        result.children().push(new Leaf(site.id, site.name))
                    });

                    return [result];
                }
                if (self.selectedSiteFilter() == self.siteFilters()[1]) {
                    var result = [];
                    ko.utils.arrayForEach(self.projects(), function (project) {
                        var pleaf = new Leaf(null, project);
                        var array = self.FilterArrayByProject(project, self.siteData(), self.selectedSites());
                        ko.utils.arrayForEach(array, function (site) {
                            pleaf.children().push(new Leaf(site.id, site.name))
                        })
                        result.push(pleaf);
                    });

                    return result;
                }
                if (self.selectedSiteFilter() == self.siteFilters()[2]) {
                    var result = [];
                    ko.utils.arrayForEach(self.siteTypes(), function (type) {
                        var pleaf = new Leaf(null, type);
                        var array = self.FilterArrayByType(type, self.siteData(), self.selectedSites());
                        ko.utils.arrayForEach(array, function (site) {
                            pleaf.children().push(new Leaf(site.id, site.name))
                        })
                        result.push(pleaf);
                    });

                    return result;
                }
                if (self.selectedSiteFilter() == self.siteFilters()[3]) {
                    var result = new Leaf(null, 'Search for ' + self.SiteSearch());
                    var array = self.FilterArrayBySearch(self.SiteSearch(), self.siteData(), self.selectedSites());
                    ko.utils.arrayForEach(array, function (site) {
                        result.children().push(new Leaf(site.id, site.name));
                    });

                    return [result];
                }
                if (self.selectedSiteFilter() == self.siteFilters()[4]) {
                    var result = new Leaf(null, 'Sites using selected fields');
                    var array = self.FilterArrayByFields(self.siteData(), self.selectedSites());
                    ko.utils.arrayForEach(array, function (site) {
                        result.children().push(new Leaf(site.id, site.name));
                    });

                    return [result];
                }
            });

            self.fields = ko.computed(function () {
                if (self.selectedFieldFilter() == self.fieldFilters()[0]) {
                    var result = new Leaf(null, 'All Fields');
                    var array = self.PreFilterArrayByNotSelected(self.selectedFields(), self.fieldData());

                    ko.utils.arrayForEach(array, function (field) {
                        result.children().push(new Leaf(field.id, field.name))
                    });

                    return [result];
                }
                if (self.selectedFieldFilter() == self.fieldFilters()[1]) {
                    var result = [];
                    ko.utils.arrayForEach(self.projects(), function (project) {
                        var pleaf = new Leaf(null, project);
                        var array = self.FilterArrayByProject(project, self.fieldData(), self.selectedFields());
                        ko.utils.arrayForEach(array, function (field) {
                            pleaf.children().push(new Leaf(field.id, field.name))
                        })
                        result.push(pleaf);
                    });

                    return result;
                }
                if (self.selectedFieldFilter() == self.fieldFilters()[2]) {
                    var result = [];
                    ko.utils.arrayForEach(self.fieldTypes(), function (type) {
                        var pleaf = new Leaf(null, type);
                        var array = self.FilterArrayByType(type, self.fieldData(), self.selectedFields());
                        ko.utils.arrayForEach(array, function (field) {
                            pleaf.children().push(new Leaf(field.id, field.name))
                        })
                        result.push(pleaf);
                    });
                    return result;
                }
                if (self.selectedFieldFilter() == self.fieldFilters()[3]) {
                    var result = new Leaf(null, 'Search for ' + self.FieldSearch());
                    var array = self.FilterArrayBySearch(self.FieldSearch(), self.fieldData(), self.selectedFields());
                    ko.utils.arrayForEach(array, function (site) {
                        result.children().push(new Leaf(site.id, site.name))
                    });

                    return [result];

                }
                if (self.selectedFieldFilter() == self.fieldFilters()[4]) {
                    var result = new Leaf(null, 'Fields using selected sites');
                    var array = self.FilterArrayBySites(self.fieldData(), self.selectedFields());
                    ko.utils.arrayForEach(array, function (site) {
                        result.children().push(new Leaf(site.id, site.name));
                    });

                    return [result];
                }
            });
            
            self.GetSummaryData = function () {
                
                if (self.selectedFields().length > 0 && self.selectedSites().length > 0) {

                    var selectedFieldIds = [];

                    ko.utils.arrayForEach(self.selectedFields(), function (field) {
                        selectedFieldIds.push(encodeURIComponent(field.id));
                    });

                    var selectedSiteIds = [];

                    ko.utils.arrayForEach(self.selectedSites(), function (site) {
                        selectedSiteIds.push(encodeURIComponent(site.id));
                    });

                    var splitDate = self.selectedDateRange().split(' - ');

                    var queryString =
                        "SelectedSiteIds=" + selectedSiteIds.join(',') +
                        "&SelectedFieldIds=" + selectedFieldIds.join(',') +
                        "&FromDate=" + splitDate[0] +
                        "&ToDate=" + splitDate[1];

                    webApiClient.ajaxGet("/reports/builder/summary", null, queryString, function (model) {
                        if (model) {
                            self.summaryData(model);
                        }
                    },
                    function (errorResponse) {
                        messageBox.ShowErrors("Error loading:", errorResponse);
                    });
                }
                else {
                    self.summaryData({});
                }
            }

            var summaryTimer;

            self.SubscribeSummaryData = function () {
                clearTimeout(summaryTimer);
                summaryTimer = setTimeout(function () {
                    self.GetSummaryData();
                }, 500);
            }

            self.selectedFields.subscribe(self.SubscribeSummaryData);
            self.selectedSites.subscribe(self.SubscribeSummaryData);
            self.selectedDateRange.subscribe(self.SubscribeSummaryData);

            self.SaveReport = function () {

                var self = this;

                messageBox.Hide();

                var nameValid = ko.validation.group(self.Name, { deep: false });
                if (nameValid.showAllMessages() > 0) {
                    messageBox.ShowError("Please correct the following errors");
                    return;
                }

                var splitDate = self.selectedDateRange().split(' - ');

                var entityModel =
                {
                    Name: self.Name(),
                    DateFrom: splitDate[0],
                    DateTo: splitDate[1],
                    FieldIds : [],
                    SiteIds : []
                }

                ko.utils.arrayForEach(self.selectedFields(), function (field) {
                    entityModel.FieldIds.push(field.id);
                });

                ko.utils.arrayForEach(self.selectedSites(), function (site) {
                    entityModel.SiteIds.push(site.id);
                });

                var reportId = page.RecordId;

                if (reportId != 'add' && reportId != null) {
                    webApiClient.ajaxPut("/reports/", reportId, ko.toJSON(entityModel), function (model) {
                        messageBox.ShowSuccess("Report Requested Successfully");
                    },
                    function (errorResponse) {
                        messageBox.ShowErrors("Error updating:", errorResponse);
                    });
                } else {
                    webApiClient.ajaxPost("/reports/", ko.toJSON(entityModel), null, function (model) {
                        messageBox.ShowSuccess("Report Requested Successfully");
                        self.SetEditMode();
                        if (model.Id) {
                            location.href = document.URL.replace("add", model.Id + "?success=true");
                        }
                    },
                    function (errorResponse) {
                        messageBox.ShowErrors("Error adding:", errorResponse);
                    });
                }
            }

            self.DeleteReport = function () {
                var reportId = page.RecordId;
                if (reportId != null) {
                    webApiClient.ajaxDelete("/reports/", reportId,
                        function (model) {
                            messageBox.ShowSuccess("Deleted Successfully");
                            self.SetDeletedMode();
                        },
                        function (errorResponse) {
                            messageBox.ShowErrors("Error deleting:", errorResponse);
                        });

                } else {
                    messageBox.ShowError("Error deleting.");
                }
            }

            self.SetSelectionDataModel = function (model) {
                var self = this;
                self.siteTypes(model.siteTypes);
                self.fieldTypes(model.fieldTypes);
                self.projects(model.projects);
                self.fieldData(model.fieldData);
                self.siteData(model.siteData);
            }

            self.IsInMode = function (mode) {
                return self.Mode() == mode;
            };

            self.CanDeleteEntity = function () {
                return self.IsInMode('EDIT');
            };

            self.SetAddMode = function () {
                self.Mode("ADD");
            };

            self.SetEditMode = function () {
                self.Mode("EDIT");
            };

            self.SetDeletedMode = function () {
                self.Mode("DELETED");
                self.IsLoaded(false);
            };

            self.ShowDeleteDialog = function () {
                var r = confirm("Are you sure you want to delete this report?");
                if (r) {
                    self.DeleteReport();
                }
            };
            
            self.GetReportEntity = function () {
                var reportId = page.RecordId;

                if (reportId != 'add' && reportId != null) {
                    webApiClient.ajaxGet("/reports/" + reportId, null, null, function (model) {
                        if (model) {
                            self.SetReportSelection(model);
                            self.SetEditMode();
                            self.IsLoaded(true);
                        }
                    },
                    function (errorResponse) {
                        messageBox.ShowErrors("Error loading:", errorResponse);
                    });
                } else {
                    self.SetAddMode();
                    self.IsLoaded(true);
                }
            }

            self.SetReportSelection = function (model) {
                ko.utils.arrayForEach(model.FieldIds, function (fieldId) {
                    var field = ko.utils.arrayFirst(self.fieldData(), function (field) {
                        return field.id == fieldId;
                    });

                    if (field != null) {
                        self.selectedFields.push(field);
                    }
                });

                ko.utils.arrayForEach(model.SiteIds, function (siteId) {
                    var site = ko.utils.arrayFirst(self.siteData(), function (site) {
                        return site.id == siteId;
                    });

                    if (site != null) {
                        self.selectedSites.push(site);
                    }
                });

                self.selectedDateRange(moment(model.DateFrom).format(common.CLIENT_DATE_FORMAT) + " - " + moment(model.DateTo).format(common.CLIENT_DATE_FORMAT));

                self.Name(model.Name)

                self.selectedFieldFilter(self.fieldFilters()[4]);
                self.selectedSiteFilter(self.siteFilters()[4]);
            }

            self.GetSelectionData = function () {
                var self = this;
                webApiClient.ajaxGet("/reports/builder", null, null, function (model) {
                    if (model) {
                        self.SetSelectionDataModel(model);
                        self.GetReportEntity();
                    }
                },
                function (errorResponse) {
                    messageBox.ShowErrors("Error loading:", errorResponse);
                });
            };

            self.SetDefaultDateRange = function () {
                self.selectedDateRange("2006/01/01 - " + moment().format(common.CLIENT_DATE_FORMAT));
            }

            self.Initialise = function () {
                var self = this;

                self.GetSelectionData();
  
                ko.applyBindings(self, $("#reportBuilder")[0]);

                $('input[id="ReportDate"]').daterangepicker({
                    format: common.CLIENT_DATE_FORMAT,
                },
                function (start, end) {
                    $('input[id="ReportDate"]').val(start.format(common.CLIENT_DATE_FORMAT) + ' - ' + end.format(common.CLIENT_DATE_FORMAT)).change();
                });

                self.SetDefaultDateRange();

                var qa = common.parseQueryString(location.search.substring(1));
                if (qa["success"]) {
                    messageBox.ShowSuccess("Report Requested Successfully");
                }
            };
        }

        return reportViewModel;
    });