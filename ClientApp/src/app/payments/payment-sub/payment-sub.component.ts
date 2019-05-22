// Angular Core
import { Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { Component, OnInit, ViewContainerRef } from '@angular/core';
// Components
import { BaseScheduleComponent } from 'src/app/shared/base-schedule.component';
// Models
import { Scroll } from 'src/app/shared/scroll.model';
import { PaymentSub } from '../shared/payment-sub.model';
import { ScrollData } from 'src/app/shared/scroll-data.model';
import { Branch } from 'src/app/dimension-datas/shared/branch.model';
import { Partner } from 'src/app/dimension-datas/shared/partner.model';
import { ProjectCode } from 'src/app/dimension-datas/shared/project-code.model';
// Services
import { AuthService } from 'src/app/core/auth/auth.service';
import { PaymentService } from '../shared/payment.service';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';

@Component({
  selector: 'app-payment-sub',
  templateUrl: './payment-sub.component.html',
  styleUrls: ['./payment-sub.component.scss']
})
export class PaymentSubComponent
  extends BaseScheduleComponent<PaymentSub, PaymentService> {
  constructor(
    service: PaymentService,
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
    if (!this.currentUser || this.currentUser.LevelUser < 1) {
      if (!this.currentUser || !this.currentUser.SubLevel || this.currentUser.SubLevel !== 3) {
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

    this.service.getAllWithScroll(schedule, "PaymentSubGetScroll/")
      .subscribe((dbData: ScrollData<PaymentSub>) => {
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
          { field: 'PartnerNo', header: 'PartnerNo', width: 175, canSort: true },
          { field: 'PartnerName', header: 'PartnerName', width: 250, canSort: true },
          { field: 'PaymentNo', header: 'PaymentNo', width: 175, canSort: true },
          { field: 'Comment', header: 'Comment', width: 175, canSort: true },
          { field: 'PaymentDateString', header: 'Date', width: 150, canSort: true },
          { field: 'Reference', header: 'Reference', width: 175, canSort: true },
          { field: 'Project', header: 'Project', width: 150, canSort: true },
          { field: 'AmountProgressString', header: 'Retenion', width: 150 },
          { field: 'AmountDownString', header: 'Down', width: 150 },
          { field: 'AmountConsumeString', header: 'Consume', width: 150 },
          { field: 'AmountRetenionString', header: 'Retenion', width: 150 },
          { field: 'AmountVatString', header: 'Vat', width: 150 },
          { field: 'AmountTaxString', header: 'Tax', width: 150 },
          { field: 'AmountDeductString', header: 'Deduct', width: 150 },
        ];

        if (dbData.Data) {
          this.datasource = dbData.Data.sort((left, right) => {
            if (left.PartnerNo > right.PartnerNo) return 1;
            if (left.PartnerNo < right.PartnerNo) return -1;
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
      if (type === "Partner") {
        this.serviceDialogs.dialogSelectPartner(this.viewCon, 1)
          .subscribe((partners: Array<Partner>) => {
            let namePartners: string = "";
            if (partners) {
              namePartners = (partners[0].PartnerName.length > 21 ? partners[0].PartnerName.slice(0, 21) + "..." : partners[0].PartnerName) +
                (partners.length > 1 ? `+ ${partners.length - 1} others` : "");
              //--------------------//
            }
            this.needReset = true;
            this.reportForm.patchValue({
              WhereBanks: partners ? partners.map((item) => item.PartnerNo) : undefined,
              BankString: partners ? namePartners : undefined,
            });
          });
      } else if (type === "Project") {
        this.serviceDialogs.dialogSelectProject(this.viewCon, 1)
          .subscribe((projects: Array<ProjectCode>) => {
            let nameProject: string = "";
            if (projects) {
              nameProject = (projects[0].ProjectName.length > 21 ? projects[0].ProjectName.slice(0, 21) + "..." : projects[0].ProjectName) +
                (projects.length > 1 ? `+ ${projects.length - 1} others` : "");
              //--------------------//
            }
            this.needReset = true;
            this.reportForm.patchValue({
              WhereProjects: projects ? projects.map((item) => item.ProjectCode) : undefined,
              ProjectString: projects ? nameProject : undefined,
            });
          });
      }
    }
  }

  // get report data
  onReport(): void {
    if (this.reportForm) {
      let scorll = this.reportForm.getRawValue() as Scroll;

      if (!scorll.WhereProjects && !scorll.Filter && !scorll.WhereBanks && !scorll.SDate) {
        this.serviceDialogs.error("Error Message", `Please select some filter befor export.`, this.viewCon);
        return;
      }

      this.loading = true;
      scorll.Skip = 0;
      scorll.Take = this.totalRecords;
      this.service.getXlsx(scorll, "PaymentSubGetReport/").subscribe(data => {
        // console.log(data);
        this.loading = false;
      }, () => this.loading = false, () => this.loading = false);
    }
  }

  // On sort
  onSort() {
    this.updateRowGroupMetaData();
  }

  // On Update row group
  updateRowGroupMetaData() {
    console.log("updateRowGroupMetaData");
    this.rowGroupMetadata = {};
    if (this.datasource) {
      for (let i = 0; i < this.datasource.length; i++) {
        let rowData = this.datasource[i];
        let PartnerNo = rowData.PartnerNo;
        if (i == 0) {
          this.rowGroupMetadata[PartnerNo] = { index: 0, size: 1 };
        }
        else {
          let previousRowData = this.datasource[i - 1];
          let previousRowGroup = previousRowData.PartnerNo;
          if (PartnerNo === previousRowGroup)
            this.rowGroupMetadata[PartnerNo].size++;
          else
            this.rowGroupMetadata[PartnerNo] = { index: i, size: 1 };
        }
      }
    }
  }

}
