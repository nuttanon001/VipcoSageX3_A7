import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { BaseScheduleComponent } from '../../shared/base-schedule.component';
import { MiscAccount } from '../shared/misc-account.model';
import { MiscService } from '../shared/misc.service';
import { FormBuilder } from '@angular/forms';
import { DialogsService } from '../../dialogs/shared/dialogs.service';
import { AuthService } from '../../core/auth/auth.service';
import { Router } from '@angular/router';
import { Scroll } from '../../shared/scroll.model';
import { ScrollData } from '../../shared/scroll-data.model';
import { ColumnType } from '../../shared/column.model';
import { ProjectCode } from '../../dimension-datas/shared/project-code.model';

@Component({
  selector: 'app-misc-master',
  templateUrl: './misc-master.component.html',
  styleUrls: ['./misc-master.component.scss']
})
export class MiscMasterComponent extends BaseScheduleComponent<MiscAccount, MiscService> {
  constructor(
    service: MiscService,
    fb: FormBuilder,
    viewCon: ViewContainerRef,
    serviceDialogs: DialogsService,
    private serviceAuth: AuthService,
    private router: Router,
  ) {
    super(service, fb, viewCon, serviceDialogs);
    // 100 for bar | 200 for titil and filter
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

    this.service.getAllWithScroll(schedule)
      .subscribe((dbData: ScrollData<MiscAccount>) => {
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
          { field: 'MiscNumber', header: 'IssueNo', width: 150, type: ColumnType.PurchaseRequest },
          { field: 'MiscDateString', header: 'IssueDate', width: 150, type: ColumnType.PurchaseRequest },
          { field: 'Project', header: 'Project', width: 125, type: ColumnType.PurchaseRequest },
          { field: 'Description', header: 'Description', width: 200, type: ColumnType.PurchaseRequest },

          { field: 'Issues', header: '', width: 10, type: ColumnType.PurchaseReceipt },
          { field: 'MiscLine', header: 'Line', width: 50, type: ColumnType.Group1 },
          { field: 'ItemCode', header: 'Code', width: 175, type: ColumnType.Group1 },
          { field: 'ItemNameRFT', header: 'Name', width: 300, type: ColumnType.Group1 },
          { field: 'QtyString', header: 'Qty', width: 100, type: ColumnType.Group1 },
          { field: 'Uom', header: 'Uom', width: 100, type: ColumnType.Group1 },
          { field: 'Branch', header: 'Branch', width: 100, type: ColumnType.Group1 },
          { field: 'WorkItem', header: 'Bom', width: 110, type: ColumnType.Group1 },
          { field: 'Project', header: 'Project', width: 110, type: ColumnType.Group1 },
          { field: 'WorkGroup', header: 'WorkGroup', width: 115, type: ColumnType.Group1 },

          { field: 'AccNumber', header: 'JournalNo', width: 150, type: ColumnType.PurchaseOrder },
          { field: 'AccDateString', header: 'JournalDate', width: 175, type: ColumnType.PurchaseOrder },
          { field: 'DocDateString', header: 'DocDate', width: 175, type: ColumnType.PurchaseOrder },
          { field: 'AccType', header: 'AccType', width: 150, type: ColumnType.PurchaseOrder },
          { field: 'AccIssue', header: 'AccRef', width: 150, type: ColumnType.PurchaseOrder },

          { field: 'Journals', header: '', width: 5, type: ColumnType.Hidder },
          { field: 'AccLine', header: 'Line', width: 75, type: ColumnType.Group2 },
          { field: 'AccountCode', header: 'Code', width: 125, type: ColumnType.Group2 },
          { field: 'AmountCurrencyString', header: 'Currency', width: 150, type: ColumnType.Group2 },
          { field: 'AccountNumber', header: 'AccNumber', width: 125, type: ColumnType.Group2 },
          { field: 'Description', header: 'Desc.', width: 125, type: ColumnType.Group2 },
          { field: 'Branch', header: 'Branch', width: 100, type: ColumnType.Group2 },
          { field: 'WorkItem', header: 'Bom', width: 100, type: ColumnType.Group2 },
          { field: 'Project', header: 'Project', width: 125, type: ColumnType.Group2 },
          { field: 'WorkGroup', header: 'WorkGroup', width: 125, type: ColumnType.Group2 },

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
      if (type === "Project") {
        this.serviceDialogs.dialogSelectProject(this.viewCon)
          .subscribe((pjt: ProjectCode) => {
            this.needReset = true;
            this.reportForm.patchValue({
              WhereProject: pjt ? pjt.ProjectCode : undefined,
              ProjectString: pjt ? `${pjt.ProjectCode} | ${pjt.ProjectName}` : undefined,
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

      if (!scorll.WhereProject && !scorll.Filter && !scorll.SDate && !scorll.EDate 
          && !scorll.SDate2 && !scorll.EDate2) {
        this.serviceDialogs.error("Error Message", `Please select item project or filter befor export.`, this.viewCon);
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

  filterItemsOfType() {
    return this.columns.filter(x => x.type !== ColumnType.Group1 && x.type !== ColumnType.Group2);
  }
}
