"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
//3rdParty
var XLSX = require("xlsx");
var BaseMasterComponent = /** @class */ (function () {
    function BaseMasterComponent(service, fb, viewCon, serviceDialogs) {
        this.service = service;
        this.fb = fb;
        this.viewCon = viewCon;
        this.serviceDialogs = serviceDialogs;
        //TimeReload
        this.message = 0;
        this.count = 0;
        this.time = 300;
        this.first = 0;
        this.needReset = false;
    }
    BaseMasterComponent.prototype.ngOnInit = function () {
        this.buildForm();
    };
    // destroy
    BaseMasterComponent.prototype.ngOnDestroy = function () {
        if (this.subscription) {
            // prevent memory leak when component destroyed
            this.subscription.unsubscribe();
        }
    };
    // build form
    BaseMasterComponent.prototype.buildForm = function () {
        var _this = this;
        if (!this.scroll) {
            this.scroll = {};
        }
        this.reportForm = this.fb.group({
            Filter: [this.scroll.Filter],
            SortField: [this.scroll.SortField],
            SortOrder: [this.scroll.SortOrder],
            TotalRow: [this.scroll.TotalRow],
            SDate: [this.scroll.SDate],
            EDate: [this.scroll.EDate],
            WhereBranch: [this.scroll.WhereBranch],
            WhereProject: [this.scroll.WhereProject],
            ProjectString: [""],
            WhereWorkGroup: [this.scroll.WhereWorkGroup],
            WorkGroupString: [""],
            WhereWorkItem: [this.scroll.WhereWorkItem],
            WorkItemString: [""],
            Skip: [this.scroll.Skip],
            Take: [this.scroll.Take],
        });
        this.reportForm.valueChanges.pipe(operators_1.debounceTime(250), operators_1.distinctUntilChanged())
            .subscribe(function (data) { return _this.onValueChanged(data); });
        var ControlMoreActivities = this.reportForm.get("Filter");
        if (ControlMoreActivities) {
            ControlMoreActivities.valueChanges
                .pipe(operators_1.debounceTime(150), operators_1.distinctUntilChanged()).subscribe(function (data) {
                _this.needReset = true;
            });
        }
    };
    // on value change
    BaseMasterComponent.prototype.onValueChanged = function (data) {
        if (!this.reportForm) {
            return;
        }
        this.scroll = this.reportForm.value;
        this.loading = true;
        this.onGetData(this.scroll);
    };
    // load Data Lazy
    BaseMasterComponent.prototype.loadDataLazy = function (event) {
        // in a real application, make a remote request to load data using state metadata from event
        // event.first = First row offset
        // event.rows = Number of rows per page
        // event.sortField = Field name to sort with
        // event.sortOrder = Sort order as number, 1 for asc and -1 for dec
        // filters: FilterMetadata object having field as key and filter value, filter matchMode as value
        // imitate db connection over a network
        this.reportForm.patchValue({
            Skip: event.first,
            Take: (event.rows || 15),
            SortField: event.sortField,
            SortOrder: event.sortOrder,
        });
    };
    // reset
    BaseMasterComponent.prototype.resetFilter = function () {
        this.datasource = new Array;
        this.scroll = undefined;
        this.loading = true;
        this.buildForm();
        this.onGetData(this.scroll);
    };
    // reload data
    BaseMasterComponent.prototype.reloadData = function () {
        var _this = this;
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
        this.subscription = rxjs_1.Observable.interval(1000)
            .take(this.time).map(function (x) { return x + 1; })
            .subscribe(function (x) {
            _this.message = _this.time - x;
            _this.count = (x / _this.time) * 100;
            if (x === _this.time) {
                if (_this.reportForm.value) {
                    _this.onGetData(_this.reportForm.value);
                }
            }
        });
    };
    BaseMasterComponent.prototype.exportData = function () {
        var Table = document.getElementById('table1');
        var ws = XLSX.utils.table_to_sheet(Table);
        /* generate workbook and add the worksheet */
        var wb = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');
        /* save to file */
        XLSX.writeFile(wb, "SheetJS.xlsx");
    };
    return BaseMasterComponent;
}());
exports.BaseMasterComponent = BaseMasterComponent;
//# sourceMappingURL=base-schedule.component.js.map