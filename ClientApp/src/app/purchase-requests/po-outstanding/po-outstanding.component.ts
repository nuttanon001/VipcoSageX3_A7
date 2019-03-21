import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { BaseScheduleComponent } from 'src/app/shared/base-schedule.component';
import { PoOutstanding } from '../shared/po-outstanding.model';
import { PrService } from '../shared/pr.service';
import { FormBuilder } from '@angular/forms';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';
import { AuthService } from 'src/app/core/auth/auth.service';
import { Router } from '@angular/router';
import { Scroll } from 'src/app/shared/scroll.model';
import { ScrollData } from 'src/app/shared/scroll-data.model';
import { ProjectCode } from 'src/app/dimension-datas/shared/project-code.model';
import { Supplier } from 'src/app/dimension-datas/shared/supplier.model';

@Component({
  selector: 'app-po-outstanding',
  templateUrl: './po-outstanding.component.html',
  styleUrls: ['./po-outstanding.component.scss']
})
export class PoOutstandingComponent extends BaseScheduleComponent<PoOutstanding, PrService> {
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
      if (!this.currentUser.SubLevel || this.currentUser.SubLevel < 1) {
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

    this.service.getAllWithScroll(schedule, "PoOutStandingGetScroll/")
      .subscribe((dbData: ScrollData<PoOutstanding>) => {
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
          { field: 'PoNumber', header: 'Po.No.', width: 130, canSort: true },
          { field: 'Project', header: 'Project', width: 125, canSort: true },
          { field: 'PoDateString', header: 'Po.Date', width: 125, canSort: true },
          { field: 'DueDateString', header: 'DueDate', width: 125, canSort: true },
          { field: 'ItemNo', header: 'ItemNo', width: 170, canSort: true },
          { field: 'TextName', header: 'ItemName', width: 350, canSort: true },
          { field: 'Uom', header: 'Uom', width: 100, canSort: true },
          { field: 'Branch', header: 'Branch', width: 100, canSort: true },
          { field: 'WorkItem', header: 'WorkItem', width: 200, canSort: true },
          { field: 'WorkGroup', header: 'WorkGroup', width: 200, canSort: true },
          { field: 'QuantityString', header: 'Quantity', width: 125, canSort: true },
          { field: 'WeigthString', header: 'Weigth', width: 125 },
          { field: 'AmountString', header: 'Amount', width: 125 },
          { field: 'StatusCloseString', header: 'Close', width: 125 },
          { field: 'StatusOrderString', header: 'Order', width: 125 },
          { field: 'SupName', header: 'Supplier', width: 250, canSort: true },
          { field: 'SysDateString', header: 'SysDate', width: 125 },
          { field: 'DIFFString', header: 'Diff', width: 125 },
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

      if (!scorll.WhereProject && !scorll.Filter && !scorll.WhereBanks && !scorll.SDate && !scorll.EDate) {
        this.serviceDialogs.error("Error Message", `Please select item project or filter befor export.`, this.viewCon);
        return;
      }

      this.loading = true;
      scorll.Skip = 0;
      scorll.Take = this.totalRecords;
      this.service.getXlsx(scorll, "PoOutStandingGetReport/").subscribe(data => {
        // console.log(data);
        this.loading = false;
      }, () => this.loading = false, () => this.loading = false);
    }
  }
}
