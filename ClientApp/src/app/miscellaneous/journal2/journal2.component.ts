import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { BaseScheduleComponent } from 'src/app/shared/base-schedule.component';
import { Journal2 } from '../shared/journal2.model';
import { InvoiceService } from '../shared/invoice.service';
import { FormBuilder } from '@angular/forms';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';
import { AuthService } from 'src/app/core/auth/auth.service';
import { Router } from '@angular/router';
import { Scroll } from 'src/app/shared/scroll.model';
import { ScrollData } from 'src/app/shared/scroll-data.model';

@Component({
  selector: 'app-journal2',
  templateUrl: './journal2.component.html',
  styleUrls: ['./journal2.component.scss']
})
export class Journal2Component extends BaseScheduleComponent<Journal2, InvoiceService> {
  constructor(
    service: InvoiceService,
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

    this.service.getAllWithScroll(schedule, "JournalGetScroll/")
      .subscribe((dbData: ScrollData<Journal2>) => {
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
          { field: 'DocumentNo', header: 'DocumentNo', canSort: true, width: 150 },
          { field: 'DateString', header: 'Date', canSort: true, width: 150 },
          { field: 'EntryType', header: 'EntryType', canSort: true, width: 125 },
          { field: 'Journal', header: 'Journal', canSort: true, width: 125 },
          { field: 'Site', header: 'Site', canSort: true, width: 125 },
          { field: 'Account', header: 'Account', width: 125 },
          { field: 'DebitString', header: 'Debit',  width: 125 },
          { field: 'CreditString', header: 'Credit', width: 125 },
          { field: 'Description', header: 'Description', width: 175 },
          { field: 'Project', header: 'Project', canSort: true, width: 125 },
          { field: 'Branch', header: 'Branch', canSort: true, width: 125 },
          { field: 'Bom', header: 'Bom', canSort: true, width: 125 },
          { field: 'WorkGroup', header: 'WorkGroup', canSort: true, width: 125 },
          { field: 'CostCenter', header: 'CostCenter', width: 125 },
          { field: 'FreeReference', header: 'FreeReference', width: 150 },
          { field: 'Tax', header: 'Tax', width: 150 },
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
  onShowDialog(type?: string): void { }

  // get report data
  onReport(): void {
    if (this.reportForm) {
      let scorll = this.reportForm.getRawValue() as Scroll;

      if (!scorll.Filter && !scorll.SDate && !scorll.EDate) {
        this.serviceDialogs.error("Error Message", `Please select item project or filter befor export.`, this.viewCon);
        return;
      }

      this.loading = true;
      scorll.Skip = 0;
      scorll.Take = this.totalRecords;
      this.service.getXlsx(scorll,"JournalGetReport/").subscribe(data => {
        this.loading = false;
      },() => this.loading = false,() => this.loading = false);
    }
  }

  //filterItemsOfType() {
  //  return this.columns.filter(x => x.type !== ColumnType.Group1 && x.type !== ColumnType.Group2);
  //}
}
