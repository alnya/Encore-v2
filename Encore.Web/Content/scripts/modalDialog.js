define(['knockout', 'jquery'],
    function (ko, $) {

        "use strict";

        var modalDialogViewModel = new function () {

            var self = this;

            self.ModalClass = ko.observable("modal fade");
            self.ModalDialogClass = ko.observable("modal-dialog");
            self.ModalDialogTitle = ko.observable("");
            self.ModalDialogBody = ko.observable("");
            self.ModalDialogTemplate = ko.observable("");
            self.ModalDialogTemplateData = ko.observable("");
            self.ModalDialogOkModalDismiss = ko.observable(false);
            self.ModalDialogOkBtnIsVisible = ko.observable(true);
            self.ModalDialogOkBtnText = ko.observable("Ok");
            self.ModalDialogOkBtnAction = "";
            self.ModalDialogCancelBtnText = ko.observable("Cancel");
            self.ModalMessage = ko.observable("");
            self.ModalMessageIcon = ko.observable('glyphicon-ok');
            self.ModalMessageType = ko.observable('hidden');

            self.ModalShowSuccess = function(message) {
                self.ModalMessage(message);
                self.ModalMessageIcon('glyphicon-ok');
                self.ModalMessageType('alert-success');
            };

            self.ModalShowError = function(message) {
                self.ModalMessage(message);
                self.ModalMessageIcon('glyphicon-remove');
                self.ModalMessageType('alert-danger');
            };

            self.ShowModalDialogCloseText = function(title, bodyText) {
                self.ShowModalDialog(true, title, bodyText, "", "", false, false, "", "", "Close");
            }

            self.ShowModalDialogCloseTemplate = function(title, template, templateData) {
                self.ShowModalDialog(false, title, "", template, templateData, false, false, "", "", "Close");
            }

            self.ShowModalDialogOkCancelTemplate = function (title, template, templateData, okBtnAction) {
                self.ShowModalDialog(false, title, "", template, templateData, false, true, okBtnAction, "Ok", "Cancel");
            }

            self.ShowModalDialogSaveCancel = function(title, template, templateData, okBtnAction) {
                self.ShowModalDialog(false, title, "", template, templateData, false, true, okBtnAction, "Save", "Cancel");
            }

            self.ShowModalDialogOkCancelSmall = function(title, bodyText, okBtnAction) {
                self.ShowModalDialog(true, title, bodyText, "", "", true, true, okBtnAction, "Ok", "Cancel");
            }

            self.ShowModalDialogOkCancelMedium = function (title, bodyText, okBtnAction) {
                self.ShowModalDialog(false, title, bodyText, "", "", true, true, okBtnAction, "Ok", "Cancel");
            }

            self.ShowModalDialog = function(isSmall, title, bodyText, template, templateData, okModalDismiss, okBtnIsVisible, okBtnAction, okBtnText, cancelBtnText) {
                $('#aeModalDialog').modal('toggle');

                if (isSmall) {
                    self.ModalClass("modal fade bs-example-modal-sm");
                    self.ModalDialogClass("modal-dialog modal-sm");
                } else {
                    self.ModalClass("modal fade");
                    self.ModalDialogClass("modal-dialog");
                }

                self.ModalDialogTitle(title);
                self.ModalDialogBody(bodyText);
                self.ModalDialogOkModalDismiss(okModalDismiss);
                self.ModalDialogOkBtnIsVisible(okBtnIsVisible);
                self.ModalDialogOkBtnAction = okBtnAction;
                self.ModalDialogOkBtnText(okBtnText);
                self.ModalDialogCancelBtnText(cancelBtnText);
                self.ModalDialogTemplateData(templateData);
                self.ModalDialogTemplate(template);

                self.ModalMessageType('hidden');
            };

            // Viewmodels subscribe to "DoAction" which is updated by "this.DialogOk"
            self.DoAction = ko.observable("");

            self.DialogOk = function () {
                self.DoAction(self.ModalDialogOkBtnAction + moment().format());
            }

            self.CloseModalDialog = function() {
                $('#modalDialog').modal('hide');
            };
        }

        ko.applyBindings(modalDialogViewModel, $("#modalDialogView")[0]);

        return modalDialogViewModel;
    });