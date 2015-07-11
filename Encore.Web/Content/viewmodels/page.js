define(['knockout', 'knockout.validation'], function (ko) {

    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateElement: true,
        grouping: { deep: true, observable: true, live: true }
    });

    return function () {

    };
});