define(['page', 'knockout', 'common', 'webApiClient', 'messageBox', 'jquery'],
    function (page, ko, common, webApiClient, messageBox, $) {

        "use strict";

        var baseForm = new function () {

            var self = this;

            self.Initialise = function (entityViewModel) {

                self.setupEntityViewModel(entityViewModel);

                self.GetEntity();

                ko.applyBindings(self, $("#entityForm")[0]);

                var qa = common.parseQueryString(location.search.substring(1));
                if (qa["success"]) {
                    messageBox.ShowSuccess("Saved successfully");
                }
            };

            self.setupEntityViewModel = function (vm) {
                vm.Created = ko.observable();
                vm.CreatedBy = ko.observable();
                vm.LastModified = ko.observable();
                vm.LastModifiedBy = ko.observable();
                self.EntityViewModel = vm;
            }

            self.formFields = function () {

                var formFields = [];

                for (var p = 0; p < self.EntityViewModel.Panels.length; p++) {
                    var panel = self.EntityViewModel.Panels[p];

                    for (var c = 0; c < panel.Columns.length; c++) {
                        var column = panel.Columns[c];

                        for (var f = 0; f < column.Fields.length; f++) {
                            formFields.push(column.Fields[f]);
                        }
                    }
                }

                return formFields;
            }

            self.IsLoaded = ko.observable(false);
            self.EntityViewModel = ko.observable();
            self.Mode = ko.observable('VIEW');
            self.ModeText = ko.observable("");
            self.OriginalModel = "";

            self.ShowCancelDialog = function () {

                if (self.IsDirty()) {
                    var r = confirm("Are you sure you want to cancel this edit?");
                    if (!r) return;
                }

                self.GetEntity();
            };

            self.ShowDeleteDialog = function () {
                var r = confirm("Are you sure you want to delete this record?");
                if (r) {
                    self.DeleteEntity();
                }
            };

            self.IsInMode = function (mode) { return self.Mode() == mode; };

            self.SetAddMode = function () {
                self.Mode("ADD");
                self.ModeText("Add a new " + self.EntityViewModel.EntityName);
                self.OriginalModel = self.EntityViewModel.GetEntityModel();
            };

            self.SetEditMode = function () {
                self.Mode("EDIT");
                self.ModeText("Edit a " + self.EntityViewModel.EntityName);
                self.OriginalModel = self.EntityViewModel.GetEntityModel();
            };

            self.SetReadOnlyMode = function () {
                self.Mode("VIEW");
                self.ModeText("View a " + self.EntityViewModel.EntityName);
                self.OriginalModel = self.EntityViewModel.GetEntityModel();
            };

            self.SetDeletedMode = function () {
                self.Mode("DELETED");
                self.ModeText("Deleted " + self.EntityViewModel.EntityName);
            };

            self.CanDeleteEntity = function () {

                if (self.IsInMode('VIEW') && self.EntityViewModel.CanDeleteEntity) {
                    return self.EntityViewModel.CanDeleteEntity;
                }

                return false;
            };

            self.CanSelectEntity = function (entityName) {
                return page["HasGet" + entityName + "Permission"];
            };

            self.CanEditEntity = function () {

                if (self.IsInMode('VIEW')) {
                    if (self.EntityViewModel.CanEditEntity != null) {
                        return self.EntityViewModel.CanEditEntity;
                    }

                    return true;
                }

                return false;
            };

            self.DisplayForm = function () {

                if (!self.IsInMode('DELETED') && self.IsLoaded()) {
                    return true;
                }

                return false;
            };

            self.GetFormFieldTemplate = function (type) {
                switch (type) {
                    case "Checkbox":
                        return "formFieldCheckBox";
                    case "Time":
                        return "formFieldTime";
                    case "String":
                        return "formFieldString";
                    case "Number":
                        return "formFieldNumber";
                    case "Text":
                        return "formFieldText";
                    case "List":
                        return "formFieldList";
                    case "Date":
                        return "formFieldDate";
                    default:
                        return "formFieldString";
                }
            };

            self.getProperty = function (prop, options) {
                if (self.EntityViewModel.hasOwnProperty(prop) && !ko.isComputed(self.EntityViewModel[prop])) {
                    var value = self.EntityViewModel[prop];
                    if (options != null && ko.isObservable(options) && value() != null) {
                        var selectedItem = ko.utils.arrayFilter(options(), function (option) {
                            return option.Id == value();
                        });
                        if (selectedItem != null && selectedItem.length == 1) { return selectedItem[0].Name; }
                    };
                    return value;
                }

                return null;
            };

            self.controlIsVisible = function (permissions) {
                var permission = $.inArray(self.Mode(), permissions) !== -1;
                if (!permission && self.Mode() == "EDIT") {
                    // special case that in edit we also want to see view fields
                    permission = $.inArray("VIEW", permissions) !== -1;
                }
                return permission;
            };

            self.controlIsReadOnly = function (permissions) {

                if (self.Mode() == "VIEW") {
                    return $.inArray(self.Mode(), permissions) !== -1;
                }
                if (self.Mode() == "EDIT" || self.Mode() == "ADD") {
                    var permission = $.inArray(self.Mode(), permissions) !== -1;
                    if (permission) {
                        return false;
                    }
                }
                return true;
            };

            self.IsDirty = function () {

                var origModel = self.OriginalModel;
                var currentModel = self.EntityViewModel.GetEntityModel();
                var changesMade = JSON.stringify(ko.toJS(origModel), self.stringifyReplacer) !== JSON.stringify(ko.toJS(currentModel), self.stringifyReplacer);
                return changesMade;
            };

            self.stringifyReplacer = function (key, value) {
                if (value === undefined) {
                    return null;
                }
                return value;
            };

            window.onbeforeunload = function (e) {
                if (self.IsDirty()) {
                    return "You have made changes to the '" + self.EntityViewModel.EntityName + "'.";
                }

                return undefined;
            };

            self.SetModel = function (model) {
                var vm = self.EntityViewModel;
                if (model.Name) {
                    $('h1.page-header').html(model.Name);
                }
                vm.SetModel(model);
            },

            self.Submit = function () {
                messageBox.Hide();

                if (!self.IsDirty()) {
                    messageBox.ShowSuccess("No changes made");
                    self.SetReadOnlyMode();
                    return;
                }

                if (!self.EntityViewModel.isValid()) {
                    messageBox.ShowError("Please correct the following errors");
                    self.EntityViewModel.errors.showAllMessages();
                } else {

                    var entityId = page.RecordId;
                    var entityModel = self.EntityViewModel.GetEntityModel();

                    if (entityId != 'add') {

                        webApiClient.ajaxPut(self.EntityViewModel.Url, entityId, ko.toJSON(entityModel), function (model) {
                            if (model) {
                                messageBox.ShowSuccess("Edited Successfully");
                                self.SetModel(model);
                                self.SetReadOnlyMode();
                            }
                        },
                            function (errorResponse) {
                                messageBox.ShowErrors("Error updating:", errorResponse);
                            });
                    } else {

                        webApiClient.ajaxPost(self.EntityViewModel.Url, ko.toJSON(entityModel), null, function (model) {
                            if (model) {
                                self.SetModel(model);
                                self.SetReadOnlyMode();
                                if (model.Id) {
                                    location.href = document.URL.replace("add", model.Id + "?success=true");
                                }
                            }
                        },
                        function (errorResponse) {
                            messageBox.ShowErrors("Error adding:", errorResponse);
                        });
                    }
                }
            };

            self.GetEntity = function () {

                var entityId = page.RecordId;
                messageBox.Hide();
                if (entityId != 'add' && entityId != null) {

                    webApiClient.ajaxGet(self.EntityViewModel.Url + entityId, null, null, function (model) {
                        if (model) {

                            self.SetModel(model);
                            self.SetReadOnlyMode();
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

            self.DeleteEntity = function () {

                var entityId = page.RecordId;

                if (entityId != null) {

                    webApiClient.ajaxDelete(self.EntityViewModel.Url, entityId,
                        function (model) {
                            messageBox.ShowSuccess("Deleted Successfully");
                            self.SetDeletedMode();

                            if (self.EntityViewModel.OnDelete) {
                                self.EntityViewModel.OnDelete();
                            }
                        },
                        function (errorResponse) {
                            messageBox.ShowErrors("Error deleting:", errorResponse);
                        });

                } else {
                    messageBox.ShowError("Error deleting.");
                }
            }
        };

        ko.bindingHandlers.datePicker = {
            init: function (element, valueAccessor) {
                common.setupDatePicker($(element).parent());
                ko.utils.registerEventHandler(element, "change", function () {
                    var observable = valueAccessor();
                    observable($(element).val());
                });
            },
            update: function (element, valueAccessor) {
                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(value);
                common.setupDatePicker($(element).parent(), value);
            }
        };

        return baseForm;
    });