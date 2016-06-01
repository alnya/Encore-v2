define(['knockout', 'webApiClient', 'messageBox'],
    function (ko, webApiClient, messageBox) {

        "use strict";

        ko.bindingHandlers.valueWithInit = {
            init: function (element, valueAccessor, allBindingsAccessor, context) {
                var observable = valueAccessor();
                var value = element.value;
                observable(value);
                ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor, context);
            },
            update: ko.bindingHandlers.value.update
        };

        var loginPageViewModel = ko.validatedObservable({
            Name: ko.observable('').extend({ required: true }),
            Password: ko.observable('').extend({ required: true }),
            Submit: function () {
                var self = this;
                messageBox.Hide();
                if (!this.isValid()) {
                    messageBox.ShowError("Please correct the following:");
                    this.errors.showAllMessages();
                } else {
                    webApiClient.ajaxPost("/login", ko.toJSON(this), null, function (response) {
                        window.location.replace("/pages/");
                    }, function (errorResponse) { messageBox.ShowError("Login Failed: Incorrect details provided"); });
                }
            }
        });

        return loginPageViewModel;
    });