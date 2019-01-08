// Angular Core
import { Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { Component, OnInit, ViewContainerRef } from '@angular/core';
// Components
import { BaseScheduleComponent } from '../../shared/base-schedule.component';
// Models
import { Scroll } from '../../shared/scroll.model';
import { StockOnhand } from '../shared/stock-onhand.model';
import { ScrollData } from '../../shared/scroll-data.model';
import { Category } from '../../dimension-datas/shared/category.model';
// Services
import { AuthService } from '../../core/auth/auth.service';
import { StockOnhandService } from '../shared/stock-onhand.service';
import { DialogsService } from '../../dialogs/shared/dialogs.service';
import { ColumnType } from '../../shared/column.model';

@Component({
  selector: 'app-stock-onhand-master',
  templateUrl: './stock-onhand-master.component.html',
  styleUrls: ['./stock-onhand-master.component.scss']
})
export class StockOnhandMasterComponent extends BaseScheduleComponent<StockOnhand, StockOnhandService> {
  constructor(
    service: StockOnhandService,
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

  // Parameter
  mobHeight: any;

  ngOnInit(): void {
    this.buildForm();
  }

  // get request data
  onGetData(schedule: Scroll): void {
    this.service.getAllWithScroll(schedule)
      .subscribe((dbData: ScrollData<StockOnhand>) => {
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
          { field: 'ItemCode', header: 'Product', width: 175, type: ColumnType.PurchaseOrder },
          { field: 'ItemDescFull', header: 'Description', width: width350, type: ColumnType.PurchaseOrder },
          //{ field: 'Uom', header: 'Uom', width: 75, type: ColumnType.PurchaseOrder },
          { field: 'Category', header: 'Category', width: width100, type: ColumnType.PurchaseOrder},
          { field: 'CategoryDesc', header: 'Category Desc', width: width150, type: ColumnType.PurchaseOrder},

          { field: 'StockLocations', header: '', width: 5, type: ColumnType.PurchaseReceipt },
          { field: 'QuantityString', header: 'StockByLocation', width: 155, type: ColumnType.Hidder },
          { field: 'LocationCode', header: 'Location', width: 105, type: ColumnType.Hidder },
          { field: 'Uom', header: 'Uom', width: 80, type: ColumnType.Hidder },
          { field: 'Project', header: 'JobNo', width: 125, type: ColumnType.Hidder },
          { field: 'LotNo', header: 'LotNo/HeatNo', width: 155, type: ColumnType.Hidder },
          { field: 'HeatNo', header: 'HeatNo', width: 125, type: ColumnType.Hidder },
          { field: 'Origin', header: 'Origin', width: 125, type: ColumnType.Hidder },
          { field: 'ExpDateString', header: 'Exp.Date', width: 140, type: ColumnType.Hidder },

          { field: 'InternelStockString', header: 'TotalStock', width: 110, type: ColumnType.PurchaseOrder },
          { field: 'OnOrderString', header: 'OnOrder', width: 95, type: ColumnType.PurchaseOrder },

          // { field: 'LocationStock', header: 'Location', width: 125, },
          // { field: 'InternelStockString', header: 'StockByLocation', width: 250, },
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
      let scorll = this.reportForm.getRawValue() as Scroll;

      // debug here
      // console.log(JSON.stringify(scorll));

      if (!scorll.WhereBanks && !scorll.Filter) {
        this.serviceDialogs.error("Error Message", `Please select item category or filter befor export.`, this.viewCon);
        return;
      }

      this.loading = true;
      this.service.getXlsx(scorll).subscribe(data => {
        //console.log(data);
        this.loading = false;
      });
    }
  }

  filterItemsOfType() {
    return this.columns.filter(x => x.type !== ColumnType.Hidder);
  }
}
