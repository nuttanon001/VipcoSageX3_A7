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
import { extend } from 'webdriver-js-extender';
import { BaseScheduleComponent } from 'src/app/shared/base-schedule.component';
import { Workgroup } from 'src/app/dimension-datas/shared/workgroup.model';
import { ProjectCode } from 'src/app/dimension-datas/shared/project-code.model';
import { BomLevel } from 'src/app/dimension-datas/shared/bom-level.model';
//3rdParty

@Component({
  selector: 'app-pr-master',
  templateUrl: './pr-master.component.html',
  styleUrls: ['./pr-master.component.scss']
})
export class PrMasterComponent extends BaseScheduleComponent<PrAndPo,PrService> {
  constructor(
    service: PrService,
    fb: FormBuilder,
    viewCon: ViewContainerRef,
    serviceDialogs: DialogsService
  ) {
    
    super(service, fb, viewCon, serviceDialogs);
    // console.log(this.mobHeight);
  }

  //Parameter

  ngOnInit(): void {
    this.sizeForm = 250;
    this.mobHeight = (window.innerHeight - this.sizeForm) + "px";

    super.ngOnInit();
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
          { field: 'PrNumber', header: 'PrNo.', canSort:true, width: width150, type: ColumnType.PurchaseRequest },
          { field: 'Project', header: 'JobNo.', canSort: true, width: width150, type: ColumnType.PurchaseRequest },
          { field: 'PRDateString', header: 'PrDate', canSort: true, width: width150, type: ColumnType.PurchaseRequest },
          { field: 'RequestDateString', header: 'Request', canSort: true, width: width150, type: ColumnType.PurchaseRequest },
          // new requirement
          { field: 'ReceivedDate', header: 'Purchase Received', width: 175, type: ColumnType.PurchaseRequest },
          { field: 'PurchaseComment', header: 'Purchase Comment', width: 175, type: ColumnType.PurchaseRequest },

          { field: 'ItemCode', header: 'Item-Code', width: width250, type: ColumnType.PurchaseRequest },
          { field: 'ItemName', header: 'Item-Name', canSort: true, width: width350, type: ColumnType.PurchaseRequest },
          { field: 'PurUom', header: 'Uom', width: width100, type: ColumnType.PurchaseRequest },
          { field: 'Branch', header: 'Branch', canSort: true, width: width150, type: ColumnType.PurchaseRequest },
          { field: 'WorkItemName', header: 'BomLv', canSort: true, width: width250, type: ColumnType.PurchaseRequest },
          { field: 'WorkGroupName', header: 'WorkGroup', canSort: true, width: width250, type: ColumnType.PurchaseRequest },
          { field: 'PROther', header: 'Other', width: width150, type: ColumnType.PurchaseRequest },
          { field: 'QuantityPur', header: 'Qty.', width: width100, type: ColumnType.PurchaseRequest },
          { field: 'PrWeightString', header: 'Weight', width: width100, type: ColumnType.PurchaseRequest },
          
          { field: 'PrCloseStatus', header: 'PrClose', width: 110, type: ColumnType.PurchaseRequest },
          { field: 'CreateBy', header: 'Create', canSort: true, width: width100, type: ColumnType.PurchaseRequest },

          { field: 'PoNumber', header: 'PoNo', canSort: true, width: width150, type: ColumnType.PurchaseOrder },
          { field: 'PoProject', header: 'JobNo', width: width150, type: ColumnType.PurchaseOrder },
          { field: 'PoDateString', header: 'PoDate', canSort: true, width: width100, type: ColumnType.PurchaseOrder },
          { field: 'DueDateString', header: 'DueDate', canSort: true, width: width100, type: ColumnType.PurchaseOrder },
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


  filterItemsOfType() {
    return this.columns.filter(x => x.type !== ColumnType.Hidder);
  }

  // Open Dialog
  onShowDialog(type?: string): void {
    if (type) {
      if (type === "WorkGroup") {
        this.serviceDialogs.dialogSelectGroup(this.viewCon)
          .subscribe((group:Workgroup) => {
            this.needReset = true;
            this.reportForm.patchValue({
              WhereWorkGroup: group ? group.WorkGroupCode : undefined,
              WorkGroupString: group ? group.WorkGroupName : undefined,
            });
          });
      } else if (type === "Project") {
        this.serviceDialogs.dialogSelectProject(this.viewCon)
          .subscribe((job:ProjectCode) => {
            this.needReset = true;
            this.reportForm.patchValue({
              WhereProject: job ? job.ProjectCode : undefined,
              ProjectString: job ? job.ProjectName : undefined,
            });
          });
      } else if (type === "WorkItem") {
        this.serviceDialogs.dialogSelectBomLevel(this.viewCon)
          .subscribe((bom:BomLevel) => {
            this.needReset = true;
            this.reportForm.patchValue({
              WhereWorkItem: bom ? bom.BomLevelCode : undefined,
              WorkItemString: bom ? bom.BomLevelName : undefined,
            });
          });
      }
    }
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
      },() => this.loading = false,() => this.loading = false);
    }
  }
}
