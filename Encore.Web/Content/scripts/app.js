requirejs.config({
    baseUrl: '/Content/',
    paths: {
        'jquery': 'scripts/thirdparty/jquery.min',
        'daterangepicker': 'scripts/thirdparty/daterangepicker',
        'bootstrap': 'scripts/thirdparty/bootstrap.min',
        'bootstrap.combobox': 'scripts/thirdparty/bootstrap.combobox',
        'knockout': 'scripts/thirdparty/knockout-3.2.0',
        'knockout.validation': 'scripts/thirdparty/knockout-validation.min',
        'moment': 'scripts/thirdparty/moment.min',      
        'gridView': 'scripts/knockout-sortable-data-table',
        'webApiClient': 'scripts/webApiClient',
        'modalDialog': 'scripts/modalDialog',
        'common': 'scripts/common',
        'validation': 'scripts/validation',
        'page': 'viewModels/page',
        'messageBox': 'viewModels/messageBox',
        'baseForm': 'viewModels/baseForm',
        'baseList': 'viewModels/baseList',
        'login': 'viewModels/login',
        'user': 'viewModels/user',
        'project': 'viewModels/project',
        'report': 'viewModels/report',
        'setup': 'viewModels/setup',
        'home': 'viewModels/home'
    },
    shim: {
        "bootstrap": {
            deps: ["jquery"]
        }
    }
});

require(['jquery', 'knockout', 'knockout.validation', 'bootstrap'], function ($, ko) {

    $.ajaxSetup({
        beforeSend: function () {
            $('.progress-spinner').addClass('active');
        },
        complete: function () {
            $('.progress-spinner').removeClass('active');
        }
    });
});