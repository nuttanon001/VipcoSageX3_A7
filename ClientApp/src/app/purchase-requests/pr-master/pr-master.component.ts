// AngularCore
import { Component, OnInit, OnDestroy, ViewContainerRef } from '@angular/core';
import { FormBuilder, FormGroup, AbstractControl } from '@angular/forms';
// Services
import { PrService } from '../shared/pr.service';
// Rxjs
import { Subscription } from 'rxjs';
import { interval } from 'rxjs';
import { switchMap, map, take } from 'rxjs/operators';
// Models
import { Scroll } from '../../shared/scroll.model';
import { PrAndPo } from '../shared/pr-and-po.model';
import { ScrollData } from '../../shared/scroll-data.model';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { LazyLoadEvent } from 'primeng/primeng';
import { MyPrimengColumn, ColumnType } from '../../shared/column.model';
import { DialogsService } from '../../dialogs/shared/dialogs.service';
//3rdParty

@Component({
  selector: 'app-pr-master',
  templateUrl: './pr-master.component.html',
  styleUrls: ['./pr-master.component.scss']
})
export class PrMasterComponent implements OnInit, OnDestroy {
  constructor(
    private service: PrService,
    private fb: FormBuilder,
    private viewCon: ViewContainerRef,
    private serviceDialogs: DialogsService
  ) {
    // 100 for bar | 200 for titil and filter
    this.mobHeight = (window.screen.height - 350) + "px";
  }

  //Parameter
  mobHeight: any;
  datasource: Array<PrAndPo>;
  totalRecords: number;
  loading: boolean;
  subscription: Subscription;
  columns: Array<MyPrimengColumn>;
  columnUppers: Array<MyPrimengColumn>;
  //TimeReload
  message: number = 0;
  count: number = 0;
  time: number = 300;
  // ScrollData
  scroll: Scroll;
  reportForm: FormGroup;
  first: number = 0;
  needReset: boolean = false;
  pageRow: number = 15;

  ngOnInit() {
    this.buildForm();
  }

  // destroy
  ngOnDestroy(): void {
    if (this.subscription) {
      // prevent memory leak when component destroyed
      this.subscription.unsubscribe();
    }
  }

  // build form
  buildForm(): void {
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

    this.reportForm.valueChanges.pipe(debounceTime(250), distinctUntilChanged())
      .subscribe((data: any) => this.onValueChanged(data));

    const ControlMoreActivities: AbstractControl | undefined = this.reportForm.get("Filter");
    if (ControlMoreActivities) {
      ControlMoreActivities.valueChanges
        .pipe(
          debounceTime(150),
          distinctUntilChanged()).subscribe((data: any) => {
            this.needReset = true;
          });
    }
  }

  // on value change
  onValueChanged(data?: any): void {
    if (!this.reportForm) { return; }
    this.scroll = this.reportForm.value;
    this.loading = true;
    this.onGetData(this.scroll);
  }

  // get request data
  onGetData(schedule: Scroll): void {
    this.service.getAllWithScroll(schedule)
      .subscribe((dbData: ScrollData<PrAndPo>) => {
        if (!dbData || !dbData.Data) {
          this.totalRecords = 0;
          this.columns = new Array;
          this.datasource = new Array;
          this.reloadData();
          this.loading = false;
          return;
        }

        this.totalRecords = dbData.Scroll.TotalRow || 0;
        // new Column Array
        let width100: number = 100;
        let width150: number = 150;
        let width250: number = 250;
        let width350: number = 350;

        this.columns = new Array;
        this.columns = [
          { field: 'PrNumber', header: 'PrNo.', width: width150, type: ColumnType.PurchaseRequest },
          { field: 'Project', header: 'JobNo.', width: width150, type: ColumnType.PurchaseRequest },
          { field: 'PRDateString', header: 'PrDate', width: width150, type: ColumnType.PurchaseRequest },
          { field: 'RequestDateString', header: 'Request', width: width150, type: ColumnType.PurchaseRequest },
          { field: 'ItemCode', header: 'Item-Code', width: width250, type: ColumnType.PurchaseRequest },
          { field: 'ItemName', header: 'Item-Name', width: width350, type: ColumnType.PurchaseRequest },
          { field: 'PurUom', header: 'Uom', width: width100, type: ColumnType.PurchaseRequest },
          { field: 'Branch', header: 'Branch', width: width150, type: ColumnType.PurchaseRequest },
          { field: 'WorkItemName', header: 'BomLv', width: width250, type: ColumnType.PurchaseRequest },
          { field: 'WorkGroupName', header: 'WorkGroup', width: width250, type: ColumnType.PurchaseRequest },
          { field: 'QuantityPur', header: 'Qty.', width: width100, type: ColumnType.PurchaseRequest },
          { field: 'PrWeightString', header: 'Weight', width: width100, type: ColumnType.PurchaseRequest },
          
          { field: 'PrCloseStatus', header: 'PrClose', width: 110, type: ColumnType.PurchaseRequest },
          { field: 'CreateBy', header: 'Create', width: width100, type: ColumnType.PurchaseRequest },

          { field: 'PoNumber', header: 'PoNo', width: width150, type: ColumnType.PurchaseOrder },
          { field: 'PoProject', header: 'JobNo', width: width150, type: ColumnType.PurchaseOrder },
          { field: 'PoDateString', header: 'PoDate', width: width100, type: ColumnType.PurchaseOrder },
          { field: 'DueDateString', header: 'DueDate', width: width100, type: ColumnType.PurchaseOrder },
          { field: 'PoQuantityPur', header: 'Qty', width: width100, type: ColumnType.PurchaseOrder },
          { field: 'PoQuantityWeight', header: 'Weight', width: width150, type: ColumnType.PurchaseOrder },
          { field: 'PoPurUom', header: 'Uom', width: width100, type: ColumnType.PurchaseOrder },
          { field: 'PoBranch', header: 'Branch', width: width150, type: ColumnType.PurchaseOrder },
          { field: 'PoWorkItemName', header: 'BomLv', width: width250, type: ColumnType.PurchaseOrder },
          { field: 'PoWorkGroupName', header: 'WorkGroup', width: width250, type: ColumnType.PurchaseOrder },
          { field: 'PoStatus', header: 'TypePo', width: width250, type: ColumnType.PurchaseOrder },
          { field: 'CloseStatus', header: 'PoStatus', width: width100, type: ColumnType.PurchaseOrder },

          { field: 'PurchaseReceipts', header: '', width: 10, type: ColumnType.PurchaseReceipt },
          { field: 'RcNumber', header: 'RecNo', width: width150, type: ColumnType.Hidder },
          { field: 'RcStatus', header: 'Status', width: 100, type: ColumnType.Hidder },
          { field: 'HeatNumber', header: 'HeatNo', width: width150, type: ColumnType.Hidder },
          { field: 'RcProject', header: 'JobNo', width: width150, type: ColumnType.Hidder },
          { field: 'RcDateString', header: 'Date', width: width100, type: ColumnType.Hidder },
          { field: 'RcQuantityPur', header: 'Qty', width: width100, type: ColumnType.Hidder },
          { field: 'RcQuantityWeight', header: 'Weight.', width: width100, type: ColumnType.Hidder },
          { field: 'RcPurUom', header: 'Uom', width: width100, type: ColumnType.Hidder },
          { field: 'RcBranch', header: 'Branch', width: width100, type: ColumnType.Hidder },
          { field: 'RcWorkItemName', header: 'BomLv', width: width150, type: ColumnType.Hidder },
          { field: 'RcWorkGroupName', header: 'WorkGroup', width: width150, type: ColumnType.Hidder },
          //{ field: 'PurchaseReceipts2', header: '', width: 10, type: ColumnType.PurchaseReceipt },
        ];

        let PrCol: MyPrimengColumn = { header: "PurchaseRequest", colspan: 0, width: 0 };
        let PoCol: MyPrimengColumn = { header: "PurchaseOrder", colspan: 0, width: 0 };
        let RcCol: MyPrimengColumn = { header: "PurchaseReceipt", colspan: 0, width: 0 };
        this.columns.forEach(item => {
          if (item.type === ColumnType.PurchaseRequest) {
            PrCol.colspan++;
            PrCol.width += item.width;
          } else if (item.type === ColumnType.PurchaseOrder) {
            PoCol.colspan++;
            PoCol.width += item.width;
          } else if (item.type === ColumnType.PurchaseReceipt || item.type === ColumnType.Hidder) {
            RcCol.colspan++;
            RcCol.width += item.width;
          }
        });
        this.columnUppers = new Array;
        this.columnUppers.push(PrCol);
        this.columnUppers.push(PoCol);
        this.columnUppers.push(RcCol);

        this.datasource = dbData.Data.slice();
        if (this.needReset) {
          this.first = 0;
          this.needReset = false;
        }

        this.reloadData();
      }, error => {
        this.totalRecords = 0;
        this.columns = new Array;
        this.datasource = new Array;
        this.reloadData();
      }, () => this.loading = false);
  }

  // load Data Lazy
  loadDataLazy(event: LazyLoadEvent): void {
    // in a real application, make a remote request to load data using state metadata from event
    // event.first = First row offset
    // event.rows = Number of rows per page
    // event.sortField = Field name to sort with
    // event.sortOrder = Sort order as number, 1 for asc and -1 for dec
    // filters: FilterMetadata object having field as key and filter value, filter matchMode as value

    this.pageRow = (event.rows || 15);
    // imitate db connection over a network
    this.reportForm.patchValue({
      Skip: event.first,
      Take: (event.rows || 15),
      SortField: event.sortField,
      SortOrder: event.sortOrder,
    });
  }

  // reset
  resetFilter(): void {
    this.datasource = new Array;
    this.scroll = undefined;
    this.loading = true;
    this.buildForm();
    this.onGetData(this.scroll);
  }

  // reload data
  reloadData(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }

    const result = interval(1000).pipe(
      take(this.time),
      map(res => {

        // console.log(res);

        this.message = this.time - res;
        this.count = (res / this.time) * 100;
        if (res === this.time) {
          if (this.reportForm.value) {
            this.onGetData(this.reportForm.value);
          }
        }
      })
    );

    this.subscription = result.subscribe();
  }

  filterItemsOfType() {
    return this.columns.filter(x => x.type !== ColumnType.Hidder);
  }

  // Open Dialog
  onShowDialog(type?: string): void {
    if (type) {
      if (type === "WorkGroup") {
        this.serviceDialogs.dialogSelectGroup(this.viewCon)
          .subscribe(group => {
            this.needReset = true;
            this.reportForm.patchValue({
              WhereWorkGroup: group ? group.WorkGroupCode : undefined,
              WorkGroupString: group ? group.WorkGroupName : undefined,
            });
          });
      } else if (type === "Project") {
        this.serviceDialogs.dialogSelectProject(this.viewCon)
          .subscribe(job => {
            this.needReset = true;
            this.reportForm.patchValue({
              WhereProject: job ? job.ProjectCode : undefined,
              ProjectString: job ? job.ProjectName : undefined,
            });
          });
      } else if (type === "WorkItem") {
        this.serviceDialogs.dialogSelectBomLevel(this.viewCon)
          .subscribe(bom => {
            this.needReset = true;
            this.reportForm.patchValue({
              WhereWorkItem: bom ? bom.BomLevelCode : undefined,
              WorkItemString: bom ? bom.BomLevelName : undefined,
            });
          });
      }
    }
  }

  exportData(): void {
    //const Table = document.getElementById('table1');
    //const ws: XLSX.WorkSheet = XLSX.utils.table_to_sheet(Table);
    ///* generate workbook and add the worksheet */
    //const wb: XLSX.WorkBook = XLSX.utils.book_new();
    //XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');
    ///* save to file */
    //XLSX.writeFile(wb, "SheetJS.xlsx");
  }

  // get report data
  onReport(): void {
    if (this.reportForm) {
      // if (this.totalRecords > 999) {
      //   this.serviceDialogs.error("Error Message", `Total records is ${this.totalRecords} over 1,000 !!!`, this.viewCon);
      //   return;
      // }
    
      let scorll = this.reportForm.getRawValue() as Scroll;
      if (!scorll.WhereProject && !scorll.Filter) {
        this.serviceDialogs.error("Error Message", `Please select jobno or filter befor export.`, this.viewCon);
        return;
      }

      this.loading = true;
      scorll.Skip = 0;
      scorll.Take = this.totalRecords;
      this.service.getXlsx(scorll).subscribe(data => {
        // console.log(data);
        this.loading = false;
      });
    }
  }
}
