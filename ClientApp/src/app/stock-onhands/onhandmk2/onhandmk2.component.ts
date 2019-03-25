import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { BaseScheduleComponent } from 'src/app/shared/base-schedule.component';
import { Onhandmk2 } from '../shared/onhandmk2.model';
import { StockOnhandService } from '../shared/stock-onhand.service';
import { FormBuilder } from '@angular/forms';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';
import { AuthService } from 'src/app/core/auth/auth.service';
import { Router } from '@angular/router';
import { Scroll } from 'src/app/shared/scroll.model';
import { ScrollData } from 'src/app/shared/scroll-data.model';
import { Category } from 'src/app/dimension-datas/shared/category.model';

@Component({
  selector: 'app-onhandmk2',
  templateUrl: './onhandmk2.component.html',
  styleUrls: ['./onhandmk2.component.scss']
})
export class Onhandmk2Component extends BaseScheduleComponent<Onhandmk2, StockOnhandService> {
  constructor(
    service: StockOnhandService,
    fb: FormBuilder,
    viewCon: ViewContainerRef,
    serviceDialogs: DialogsService,
    private serviceAuth: AuthService,
    private router: Router,
  ) {
    super(service, fb, viewCon, serviceDialogs);

    this.serviceAuth.currentUser.subscribe(x => {
      // console.log(JSON.stringify(x));
      this.currentUser = x;
    });
  }

  // Parameter
  failLogin: boolean = false;
  rowGroupMetadata: any;

  ngOnInit(): void {
    this.buildForm();
    if (!this.currentUser || this.currentUser.LevelUser < 2) {
      if (!this.currentUser || !this.currentUser.SubLevel || this.currentUser.SubLevel < 1) {
        this.serviceDialogs.error("Waining Message", "Access is restricted. please contact administrator !!!", this.viewCon).
          subscribe(() => this.router.navigate(["login"]));
      } else {
        this.failLogin = true;
      }
    } else {
      this.failLogin = true;
    }

    // this.failLogin = true;
  }
  // get request data
  onGetData(schedule: Scroll): void {
    if (!this.failLogin) {
      return;
    }

    this.service.getAllWithScroll(schedule, "OnHandMk2GetScroll/")
      .subscribe((dbData: ScrollData<Onhandmk2>) => {
        if (!dbData && !dbData.Data) {
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

        this.columns = new Array;
        this.columns = [
          //{ field: 'LocationCode', header: 'Location', width: 130, canSort: true },
          { field: 'ItemNo', header: 'ItemCode', width: 175, canSort: true },
          { field: 'TextName', header: 'TextName', width: 350, canSort: true },
          { field: 'ItemCate', header: 'Category', width: 125, canSort: true },
          { field: 'LotCode', header: 'Lot', width: 125, canSort: true },
          { field: 'QuantityString', header: 'Quantity', width: 150, canSort: true },
          { field: 'Uom', header: 'Uom', width: 120, canSort: true },
          { field: 'UnitPriceString', header: 'UnitPrice', width: 100 },
          { field: 'AmountString', header: 'Amonut', width: 120 },
          { field: 'OrderDateString', header: 'OrderDate', width: 120 },
        ];

        if (dbData.Data) {
          this.datasource = dbData.Data.sort((left, right) => {
            if (left.LocationCode > right.LocationCode) return 1;
            if (left.LocationCode < right.LocationCode) return -1;
            return 0;
          }).slice();

          // this.datasource = dbData.Data;
        } else {
          this.datasource = new Array;
        }

        if (this.needReset) {
          this.first = 0;
          this.needReset = false;
        }

        this.updateRowGroupMetaData();
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
          .subscribe((category: Array<Category>) => {
            let nameCategory: string = "";
            if (category) {
              nameCategory = (category[0].CategoryName.length > 15 ? category[0].CategoryName.slice(0, 15) + "..." : category[0].CategoryName) +
                (category.length > 1 ? `+ ${category.length - 1} others` : "");
              //--------------------//
            }
            this.needReset = true;
            this.reportForm.patchValue({
              WhereBanks: category ? category.map((item) => item.CategoryCode) : undefined,
              BankString: category ? nameCategory : undefined,
            });
          });
      }
    }
  }

  // get report data
  onReport(): void {
    if (this.reportForm) {
      let scorll = this.reportForm.getRawValue() as Scroll;

      if (!scorll.WhereProject && !scorll.Filter && !scorll.WhereBanks && !scorll.SDate
           && !scorll.WhereRange11 && !scorll.WhereRange12 && !scorll.WhereRange21 && !scorll.WhereRange22) {
        this.serviceDialogs.error("Error Message", `Please select item project or filter befor export.`, this.viewCon);
        return;
      }

      this.loading = true;
      scorll.Skip = 0;
      scorll.Take = this.totalRecords;
      this.service.getXlsx(scorll, "OnHandMk2GetReport/").subscribe(data => {
        // console.log(data);
        this.loading = false;
      }, () => this.loading = false, () => this.loading = false);
    }
  }

  onSort() {
    this.updateRowGroupMetaData();
  }

  updateRowGroupMetaData() {
    this.rowGroupMetadata = {};
    if (this.datasource) {
      for (let i = 0; i < this.datasource.length; i++) {
        let rowData = this.datasource[i];
        let LocationCode = rowData.LocationCode;
        if (i == 0) {
          this.rowGroupMetadata[LocationCode] = { index: 0, size: 1 };
        }
        else {
          let previousRowData = this.datasource[i - 1];
          let previousRowGroup = previousRowData.LocationCode;
          if (LocationCode === previousRowGroup)
            this.rowGroupMetadata[LocationCode].size++;
          else
            this.rowGroupMetadata[LocationCode] = { index: i, size: 1 };
        }
      }
    }
  }
}
