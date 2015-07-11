define(['knockout', 'validation',  'webApiClient', 'messageBox'],
    function (ko, validation, webApiClient, messageBox) {

        "use strict";

        var userViewModel = ko.validatedObservable({

            EntityName: "User",
            Url: "/account/",
            CanDeleteEntity: false,
            Name: ko.observable().extend({ maxLength: 40, required: true }),
            Email: ko.observable().extend({ maxLength: 100, required: false }),
            Password: ko.observable().extend({ maxLength: 128, required: true }),
            PasswordConfirm: ko.observable(),

            SetModel: function (objFromServer) {
                var self = this;
                if (!objFromServer) return;

                self.Name(objFromServer.Name);
                self.Password(objFromServer.Password);
                self.PasswordConfirm(objFromServer.Password);
                self.Email(objFromServer.Email);
            },

            GetEntityModel: function () {
                var self = this;

                return {
                    Name: self.Name(),
                    Password: self.Password(),
                    Email: self.Email()
                };
            }
        });

        userViewModel().PasswordConfirm.extend({ maxLength: 128, required: true, passwordConfirm: userViewModel().Password });

    return userViewModel;
    });

