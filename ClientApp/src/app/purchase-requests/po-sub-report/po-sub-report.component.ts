import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { BaseScheduleComponent } from 'src/app/shared/base-schedule.component';
import { PoSubReport } from '../shared/po-sub-report';
import { PrService } from '../shared/pr.service';
import { FormBuilder } from '@angular/forms';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';
import { AuthService } from 'src/app/core/auth/auth.service';
import { Router } from '@angular/router';
import { ScrollData } from 'src/app/shared/scroll-data.model';
import { Scroll } from 'src/app/shared/scroll.model';
import { ProjectCode } from 'src/app/dimension-datas/shared/project-code.model';
import { Supplier } from 'src/app/dimension-datas/shared/supplier.model';

@Component({
  selector: 'app-po-sub-report',
  templateUrl: './po-sub-report.component.html',
  styleUrls: ['./po-sub-report.component.scss']
})
export class PoSubReportComponent extends BaseScheduleComponent<PoSubReport, PrService> {
  constructor(
    service: PrService,
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

  ngOnInit(): void {
    this.buildForm();
    if (!this.currentUser || this.currentUser.LevelUser < 2) {
      if (!this.currentUser || !this.currentUser.SubLevel || this.currentUser.SubLevel < 2) {
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

    this.service.getAllWithScroll(schedule, "SubReportGetScroll/")
      .subscribe((dbData: ScrollData<PoSubReport>) => {
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
          // { field: 'InvoiceStatusString', header: 'Type', width: 150 },
          { field: 'PoDateString', header: 'Date', width: 125, canSort: true },
          { field: 'PoNumber', header: 'Po.No.', width: 125, canSort: true },
          { field: 'ItemNo', header: 'ItemNo', width: 170, canSort: true },
          { field: 'TextName', header: 'ItemName', width: 350, canSort: true },
          { field: 'ProjectLine', header: 'Project', width: 150, canSort: true },
          { field: 'SupName', header: 'Supplier', width: 250, canSort: true },
          { field: 'Uom', header: 'Uom', width: 100, canSort: true },
          { field: 'QuantityString', header: 'Quantity', width: 125, canSort: true },
          { field: 'UnitPriceString', header: 'UnitPrice', width: 125 },
          { field: 'AmountString', header: 'Amount', width: 125, canSort: true },
          { field: 'WeigthPerQuantityString', header: 'Kg/Qty', width: 125 },
          { field: 'WeigthString', header: 'Weigth', width: 125, canSort: true },
          { field: 'AmountPerKgString', header: 'Price/Kg', width: 125 },
        ];

        if (dbData.Data) {
          this.datasource = dbData.Data;
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
      if (type === "Project") {
        this.serviceDialogs.dialogSelectProject(this.viewCon)
          .subscribe((pjt: ProjectCode) => {
            this.needReset = true;
            this.reportForm.patchValue({
              WhereProject: pjt ? pjt.ProjectCode : undefined,
              ProjectString: pjt ? `${pjt.ProjectCode} | ${pjt.ProjectName}` : undefined,
            });
          });
      } else if (type === "Supplier") {
        this.serviceDialogs.dialogSelectSupplier(this.viewCon, 1)
          .subscribe((supplier: Array<Supplier>) => {
            let nameSupplier: string = "";
            if (supplier) {
              nameSupplier = (supplier[0].SupplierName.length > 15 ? supplier[0].SupplierName.slice(0, 15) + "..." : supplier[0].SupplierName) +
                (supplier.length > 1 ? `+ ${supplier.length - 1} others` : "");
              //--------------------//
            }
            this.needReset = true;
            this.reportForm.patchValue({
              WhereBanks: supplier ? supplier.map((item) => item.SupplierNo) : undefined,
              BankString: supplier ? nameSupplier : undefined,
            });
          });
      }
    }
  }

  // get report data
  onReport(): void {
    if (this.reportForm) {
      let scorll = this.reportForm.getRawValue() as Scroll;

      if (!scorll.WhereProject && !scorll.WhereBanks && !scorll.Filter && !scorll.SDate && !scorll.EDate) {
        this.serviceDialogs.error("Error Message", `Please select item project or filter befor export.`, this.viewCon);
        return;
      }

      this.loading = true;
      scorll.Skip = 0;
      scorll.Take = this.totalRecords;
      this.service.getXlsx(scorll, "SubReportGetReport/").subscribe(data => {
        // console.log(data);
        this.loading = false;
      }, () => this.loading = false, () => this.loading = false);
    }
  }
}
