define(['knockout'], function (ko) {

    ko.validation.rules['passwordConfirm'] = {
        validator: function (confirm, password) {

            return confirm === password;
        },
        message: 'Must be the same as password.'
    };

    ko.validation.registerExtenders();
});