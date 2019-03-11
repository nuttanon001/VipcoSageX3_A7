import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { InvoiceOutDue } from '../shared/invoice-out-due.model';
import { FormBuilder } from '@angular/forms';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';
import { AuthService } from 'src/app/core/auth/auth.service';
import { Router } from '@angular/router';
import { User } from 'src/app/users/shared/user.model';
import { InvoiceOutDueService } from '../shared/invoice-out-due.service';
import { Scroll } from 'src/app/shared/scroll.model';
import { ScrollData } from 'src/app/shared/scroll-data.model';
import { ProjectCode } from 'src/app/dimension-datas/shared/project-code.model';
import { Customer } from 'src/app/dimension-datas/shared/customer.model';
import { BaseScheduleComponent } from 'src/app/shared/base-schedule.component';

@Component({
  selector: 'app-invoice-out-due',
  templateUrl: './invoice-out-due.component.html',
  styleUrls: ['./invoice-out-due.component.scss']
})
export class InvoiceOutDueComponent extends BaseScheduleComponent<InvoiceOutDue, InvoiceOutDueService> {
  constructor(
    service: InvoiceOutDueService,
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
  currentUser: User;
  failLogin: boolean = false;
  rowGroupMetadata: any;

  ngOnInit(): void {
    this.buildForm();
    if (!this.currentUser || this.currentUser.LevelUser < 2) {
      this.serviceDialogs.error("Waining Message", "Access is restricted. please contact administrator !!!", this.viewCon).
        subscribe(() => this.router.navigate(["login"]));
    } else {
      this.failLogin = true;
    }
  }

  // get request data
  onGetData(schedule: Scroll): void {
    if (!this.failLogin) {
      return;
    }

    this.service.getAllWithScroll(schedule,"InvoiceOutStandingGetScroll/")
      .subscribe((dbData: ScrollData<InvoiceOutDue>) => {
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
          { field: 'InvoiceNo', header: 'InvoiceNo', width: 150, canSort: true},
          { field: 'Project', header: 'JobNo', width: 125, canSort: true},
          { field: 'CustomerName', header: 'Customer', width: 350, canSort: true},
          { field: 'THB_TAXString', header: 'THB+Tax', width: 100 },
          { field: 'USD_TAXString', header: 'USD+Tax', width: 100 },
          { field: 'EUR_TAXString', header: 'EUR+Tax', width: 100 },
          { field: 'THBString', header: 'THB', width: 100 },
          { field: 'USDString', header: 'USD', width: 100 },
          { field: 'EURString', header: 'EUR', width: 100 },
          { field: 'DocDateString', header: 'IssuedDate', width: 130, canSort: true },
          { field: 'DueDateString', header: 'DueDate', width: 130, canSort: true },
          { field: 'NowDateString', header: 'SystemDate', width: 130 },
          { field: 'DIFFString', header: 'Duration(d)', width: 140 },
        ];

        if (dbData.Data) {
          this.datasource = dbData.Data.sort((left, right) => {
            if (left.InvoiceStatus > right.InvoiceStatus) return 1;
            if (left.InvoiceStatus < right.InvoiceStatus) return -1;
            return 0;
          }).slice();

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
      if (type === "Project") {
        this.serviceDialogs.dialogSelectProject(this.viewCon)
          .subscribe((pjt: ProjectCode) => {
            this.needReset = true;
            this.reportForm.patchValue({
              WhereProject: pjt ? pjt.ProjectCode : undefined,
              ProjectString: pjt ? `${pjt.ProjectCode} | ${pjt.ProjectName}` : undefined,
            });
          });
      } else if (type === "Customer") {
        this.serviceDialogs.dialogSelectCustomer(this.viewCon, 1)
          .subscribe((customer: Array<Customer>) => {
            let nameCustomers: string = "";
            if (customer) {
              nameCustomers = (customer[0].CustomerName.length > 15 ? customer[0].CustomerName.slice(0, 15) + "..." : customer[0].CustomerName) +
                (customer.length > 1 ? `+ ${customer.length - 1} others` : "");
              //--------------------//
            }
            this.needReset = true;
            this.reportForm.patchValue({
              WhereBanks: customer ? customer.map((item) => item.CustomerNo) : undefined,
              BankString: customer ? nameCustomers : undefined,
            });
          });
      }
    }
  }

  // get report data
  onReport(): void {
    if (this.reportForm) {
      let scorll = this.reportForm.getRawValue() as Scroll;

      //if (!scorll.WhereProject && !scorll.WhereBanks && !scorll.Filter) {
      //  this.serviceDialogs.error("Error Message", `Please select item project or filter befor export.`, this.viewCon);
      //  return;
      //}

      this.loading = true;
      scorll.Skip = 0;
      scorll.Take = this.totalRecords;
      this.service.getXlsx(scorll).subscribe(data => {
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
        let InvoiceStatus = rowData.InvoiceStatus;
        if (i == 0) {
          this.rowGroupMetadata[InvoiceStatus] = { index: 0, size: 1 };
        }
        else {
          let previousRowData = this.datasource[i - 1];
          let previousRowGroup = previousRowData.InvoiceStatus;
          if (InvoiceStatus === previousRowGroup)
            this.rowGroupMetadata[InvoiceStatus].size++;
          else
            this.rowGroupMetadata[InvoiceStatus] = { index: i, size: 1 };
        }
      }
    }
  }
}
