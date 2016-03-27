define(['knockout', 'validation',  'webApiClient', 'messageBox', 'modalDialog'],
    function (ko, validation, webApiClient, messageBoxm, modalDialog) {

        "use strict";

        var userViewModel = ko.validatedObservable({

            EntityName: "User",
            Url: "/users/",
            CanDeleteEntity: true,
            Name: ko.observable().extend({ maxLength: 40, required: true }),
            Email: ko.observable().extend({ maxLength: 100, required: false }),
            UserRole: ko.observable().extend({ required: true }),
            Password: ko.observable().extend({ maxLength: 128, required: true }),
            PasswordConfirm: ko.observable(),
            ProjectTokens: ko.observableArray([]),
            Projects: ko.observable([]),

            SetModel: function (objFromServer) {
                var self = this;
                if (!objFromServer) return;

                self.Name(objFromServer.Name);
                self.Password(objFromServer.Password);
                self.PasswordConfirm(objFromServer.Password);
                self.Email(objFromServer.Email);
                self.UserRole(objFromServer.UserRole);

                if (objFromServer.ProjectTokens != null) {
                    self.ProjectTokens([]);
                    ko.utils.arrayForEach(objFromServer.ProjectTokens, function (obj) {
                        var objToken = self.ProjectToken();
                        objToken.CopyFrom(obj);
                        self.ProjectTokens.push(objToken);
                    });
                }
            },

            GetEntityModel: function () {
                var self = this;

                return {
                    Name: self.Name(),
                    Password: self.Password(),
                    Email: self.Email(),
                    UserRole: self.UserRole(),
                    ProjectTokens: self.GetProjectTokens()
                };
            },

            ProjectToken: function () {
                var self = this;

                return {
                    Parent: self,
                    ProjectId: ko.observable().extend({ required: true }),
                    UserName: ko.observable().extend({ required: true, maxLength: 40 }),
                    Token: ko.observable().extend({ required: true, maxLength: 128 }),
                    TokenConfirm: ko.observable().extend({ required: true, maxLength: 128 }),

                    CopyFrom: function (obj) {
                        var self = this;
                        var copyValue = function (value) {
                            return typeof value == 'function' ? value() : value;
                        }
                        self.ProjectId(copyValue(obj.ProjectId));
                        self.UserName(copyValue(obj.UserName));
                        self.Token(copyValue(obj.Token));
                        self.TokenConfirm(copyValue(obj.Token));
                    }
                }
            },
            
            GetProjectTokens: function () {
                var self = this;
                var projectTokens = [];
                ko.utils.arrayForEach(self.ProjectTokens(), function (model) {
                    projectTokens.push({
                        ProjectId: model.ProjectId,
                        UserName: model.UserName,
                        Token: model.Token
                    });
                });
                return projectTokens;
            },
            
            RemoveToken: function (model) {
                this.Parent.ProjectTokens.remove(model);
            },

            AddToken: function () {
                var newToken = this.ProjectToken();
                newToken.TokenConfirm.extend({ passwordConfirm: newToken.Token });

                modalDialog.ShowModalDialogOkCancelTemplate("Add Project Authorization", "projectTokenTemplate", newToken, 'AddProjectAuthorization');
            },

            EditToken: function (model) {
                var editToken = model.Parent.ProjectToken();
                editToken.TokenConfirm.extend({ passwordConfirm: editToken.Token });

                editToken.CopyFrom(model);
                editToken.Original = model;

                modalDialog.ShowModalDialogOkCancelTemplate("Edit Project Authorization", "projectTokenTemplate", editToken, 'EditProjectAuthorization');
            }
        });

        if (modalDialog != null) {
            modalDialog.DoAction.subscribe(function (action) {
                var model = modalDialog.ModalDialogTemplateData();
                var validationModel = ko.validatedObservable(model);

                if (action.indexOf("EditProjectAuthorization") === 0) {
                    if (!validationModel.isValid()) {
                        modalDialog.ModalShowError("Please correct the following errors");
                    } else {
                        model.Original.CopyFrom(model);
                        modalDialog.CloseModalDialog();
                    }
                }
                else if (action.indexOf("AddProjectAuthorization") === 0) {
                    if (!validationModel.isValid()) {
                        validationModel.errors.showAllMessages();
                        modalDialog.ModalShowError("Please correct the following errors");
                    } else {
                        userViewModel().ProjectTokens.push(model);
                        modalDialog.CloseModalDialog();
                    }
                }
            });
        };

        userViewModel().PasswordConfirm.extend({ maxLength: 128, required: true, passwordConfirm: userViewModel().Password });

        webApiClient.ajaxGet("/projects/", null, null, function (projects) {
            if (projects) {
                userViewModel().Projects(projects.Results);
            }
        });

    return userViewModel;
    });

