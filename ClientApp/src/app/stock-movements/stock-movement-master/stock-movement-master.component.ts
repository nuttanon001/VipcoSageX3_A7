import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { ColumnType } from '../../shared/column.model';
import { BaseScheduleComponent } from '../../shared/base-schedule.component';
import { StockMovement } from '../shared/stock-movement.model';
import { StockMovementsService } from '../shared/stock-movements.service';
import { FormBuilder } from '@angular/forms';
import { DialogsService } from '../../dialogs/shared/dialogs.service';
import { AuthService } from '../../core/auth/auth.service';
import { Router } from '@angular/router';
import { Scroll } from '../../shared/scroll.model';
import { ScrollData } from '../../shared/scroll-data.model';
import { Category } from '../../dimension-datas/shared/category.model';

@Component({
  selector: 'app-stock-movement-master',
  templateUrl: './stock-movement-master.component.html',
  styleUrls: ['./stock-movement-master.component.scss']
})
export class StockMovementMasterComponent extends BaseScheduleComponent<StockMovement, StockMovementsService> {
  constructor(
    service: StockMovementsService,
    fb: FormBuilder,
    viewCon: ViewContainerRef,
    serviceDialogs: DialogsService,
    private serviceAuth: AuthService,
    private router: Router,
  ) {
    super(service, fb, viewCon, serviceDialogs);
    // 100 for bar | 200 for titil and filter
    this.mobHeight = (window.screen.height - 340) + "px";
  }

  //Parameter
  mobHeight: any;

  ngOnInit(): void {
    this.buildForm();
  }

  // get request data
  onGetData(schedule: Scroll): void {
    this.service.getAllWithScroll(schedule)
      .subscribe((dbData: ScrollData<StockMovement>) => {
        if (!dbData) {
          this.totalRecords = 0;
          this.columns = new Array;
          this.datasource = new Array;
          this.reloadData();
          this.loading = false;
          return;
        }

        if (dbData.Scroll) {
          this.totalRecords = dbData.Scroll.TotalRow || 0;
        } else {
          this.totalRecords = 0;
        }

        // new Column Array
        let width100: number = 100;
        let width150: number = 150;
        let width250: number = 250;
        let width350: number = 350;

        this.columns = new Array;
        this.columns = [
          { field: 'ItemCode', header: 'Product', width: 185, type: ColumnType.PurchaseOrder },
          { field: 'ItemDescFull', header: 'Description', width: width350, type: ColumnType.PurchaseOrder },
          { field: 'Category', header: 'Category', width: width100, type: ColumnType.PurchaseOrder },
          { field: 'CategoryDesc', header: 'Category Desc', width: 200, type: ColumnType.PurchaseOrder },
          { field: 'Uom', header: 'Uom', width: 75, type: ColumnType.PurchaseOrder },

          { field: 'StockMovement2s', header: '', width: 10, type: ColumnType.PurchaseReceipt },
          { field: 'DocNo', header: 'DocNo', width: width150, type: ColumnType.Hidder },
          { field: 'MovementType', header: 'Type', width: 125, type: ColumnType.Hidder },
          { field: 'MovementDateString', header: 'Date', width: 110, type: ColumnType.Hidder },
          { field: 'QuantityInString', header: 'MoveIn', width: 85, type: ColumnType.Hidder },
          { field: 'QuantityOutString', header: 'MoveOut', width: 110, type: ColumnType.Hidder },
          { field: 'Location', header: 'Location', width: 150, type: ColumnType.Hidder },
          { field: 'Bom', header: 'Bom', width: 100, type: ColumnType.Hidder },
          { field: 'Project', header: 'JobNo', width: 100, type: ColumnType.Hidder },
          { field: 'WorkGroup', header: 'Group', width: 100, type: ColumnType.Hidder },
        ];

        if (dbData.Data) {
          this.datasource = dbData.Data.slice();
        } else {
          this.datasource = new Array;
        }

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

  // Open Dialog
  onShowDialog(type?: string): void {
    if (type) {
      if (type === "Category") {
        this.serviceDialogs.dialogSelectCategory(this.viewCon, 1)
          .subscribe((cates: Array<Category>) => {
            let nameCategory: string = "";
            if (cates) {
              nameCategory = (cates[0].CategoryName.length > 15 ? cates[0].CategoryName.slice(0, 15) + "..." : cates[0].CategoryName) +
                (cates.length > 1 ? `+ ${cates.length - 1} others` : "");
              //--------------------//
            }
            this.needReset = true;
            this.reportForm.patchValue({
              WhereBanks: cates ? cates.map((item) => item.CategoryCode) : undefined,
              BankString: cates ? nameCategory : undefined,
            });
          });
      }
    }
  }

  // get report data
  onReport(): void {
    if (this.reportForm) {
      this.loading = true;
      const scorll = this.reportForm.getRawValue() as Scroll;
      this.service.getXlsx(scorll).subscribe(data => {
        // console.log(data);
        this.loading = false;
      });
    }
  }

  filterItemsOfType() {
    return this.columns.filter(x => x.type !== ColumnType.Hidder);
  }
}
