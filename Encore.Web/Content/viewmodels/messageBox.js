define(['knockout', 'jquery'], function (ko, $) {

    var messageBoxViewModel = {
        Icon: ko.observable(''),
        Message: ko.observable(''),
        Type: ko.observable('hidden'),

        ShowSuccess: function(message) {
            this.Type("alert-success");
            this.Icon("glyphicon-ok");
            this.Message(message);
        },
        ShowError: function(message) {
            this.Type("alert-danger");
            this.Icon("glyphicon-remove");
            this.Message(message);
        },
        ShowErrors: function(message, objFromServer) {
            this.Type("alert-danger");
            this.Icon("glyphicon-remove");
            if (objFromServer != null && objFromServer.responseJSON.ValidationErrors != null) {
                message = "<p>" + message + "</p><ul>";
                ko.utils.arrayForEach(objFromServer.responseJSON.ValidationErrors, function(objError) {
                    message += ("<li>" + (objError.Row ? "[" + objError.Row + "] " : "") + objError.Property + " : " + objError.ErrorMessage + "</li>");
                });
                 message += "</ul>";
            } else if (objFromServer != null && objFromServer.responseJSON.ErrorMessage != null) {
                message = message + " : " + objFromServer.responseJSON.ErrorMessage;
            }
            this.Message(message);
        },
        Hide: function() {
            this.Type("hidden");
        }
    };

    if ($("#messageBox").length) {
        ko.applyBindings(messageBoxViewModel, $("#messageBox")[0]);
    }
    return messageBoxViewModel;
});