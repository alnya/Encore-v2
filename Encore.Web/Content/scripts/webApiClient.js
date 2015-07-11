define(['jquery'], function ($) {

    "use strict";

    var baseUrl = '/data/';

    return {
        baseUrl: baseUrl,

        ajaxGet: function(method, input, query, callback, errorCallback) {
            var url = baseUrl + method;
            if (query) {
                url = url + "?" + query;
            }

            $.ajax({
                url: url,
                type: "GET",
                data: input,
                contentType: "application/json",
                dataType: "json",
                success: function(result) {
                    callback(result, method);
                },
                error: function(result) {
                    if (errorCallback != null) {
                        errorCallback(result);
                    }
                }
            });
        },

        ajaxPost: function (method, input, params, callback, errorCallback) {

            var url = baseUrl + method;

            if (params) {
                url = url + "?" + params;
            }

            $.ajax({
                url: url,
                type: "POST",
                data: input,
                contentType: "application/json",
                dataType: "json",
                success: function(result) {
                    callback(result);
                },
                error: function(result) {
                    if (errorCallback != null) {
                        errorCallback(result);
                    }
                }
            });
        },

        ajaxPut: function(method, id, input, callback, errorCallback) {

            $.ajax({
                url: baseUrl + method + "/" + id,
                type: "PUT",
                data: input,
                contentType: "application/json",
                dataType: "json",
                success: function(result) {
                    callback(result);
                },
                error: function(result) {
                    if (errorCallback != null) {
                        errorCallback(result);
                    }
                }
            });
        },

        ajaxDelete: function(method, id, callback, errorCallback) {

            $.ajax({
                url: baseUrl + method + "/" + id,
                type: "DELETE",
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    callback(result);
                },
                error: function(result) {
                    if (errorCallback != null) {
                        errorCallback(result);
                    }
                }
            });
        },

        ajaxUploadCsv: function (method, file, callback, errorCallback) {

            $.ajax({
                url: baseUrl + method,
                beforeSend: function (request) {
                    $('.progress-spinner').addClass('active');
                    request.setRequestHeader("Content-Type", "text/csv");
                },
                type: "PUT",
                data: file,
                processData: false,
                contentType: false,
                dataType: "json",
                success: function(result) {
                    callback(result);
                },
                error: function(result) {
                    if (errorCallback != null) {
                        errorCallback(result);
                    }
                }
            });
        }
    }
});