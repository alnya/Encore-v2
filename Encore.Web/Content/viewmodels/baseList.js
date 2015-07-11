define(['knockout', 'jquery', 'gridView', 'common'],
    function (ko, $, gridView, common) {

    "use strict";

    var baseList = {
        ListViewModel: function (gridConfig, onCreateNew) {
            this.tableViewModel = new gridView.GridViewModel(gridConfig);

            this.CanAddEntity = function () {
                return true;
            }

            this.CreateNew = onCreateNew;

            ko.applyBindings(this, $("#listView")[0]);
        }
    };

    common.setupControls();

    return baseList;
});