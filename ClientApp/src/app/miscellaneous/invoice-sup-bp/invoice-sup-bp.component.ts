import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { InvoiceSupBp } from '../shared/invoice-sup-bp.model';
import { InvoiceService } from '../shared/invoice.service';
import { FormBuilder } from '@angular/forms';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';
import { AuthService } from 'src/app/core/auth/auth.service';
import { Router } from '@angular/router';
import { BaseScheduleComponent } from 'src/app/shared/base-schedule.component';
import { Scroll } from 'src/app/shared/scroll.model';
import { ScrollData } from 'src/app/shared/scroll-data.model';
import { ColumnType } from 'src/app/shared/column.model';
import { ProjectCode } from 'src/app/dimension-datas/shared/project-code.model';

@Component({
  selector: 'app-invoice-sup-bp',
  templateUrl: './invoice-sup-bp.component.html',
  styleUrls: ['./invoice-sup-bp.component.scss']
})
export class InvoiceSupBpComponent extends BaseScheduleComponent<InvoiceSupBp, InvoiceService> {
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

    this.service.getAllWithScroll(schedule)
      .subscribe((dbData: ScrollData<InvoiceSupBp>) => {
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
          { field: 'AccountDateString', header: 'AccountDate', canSort: true, width: 150 },
          { field: 'InvType', header: 'InvType', canSort: true, width: 100 },
          { field: 'Site', header: 'Site', canSort: true, width: 125 },
          { field: 'SupplierName', header: 'Supplier', canSort: true, width: 200 },
          { field: 'LineAccountCode', header: 'AccountCode', canSort: true, width: 125 },
          { field: 'AmountTaxString', header: 'AmountTax', canSort: true,width: 125 },
          { field: 'Tax', header: 'Tax', canSort: true, width: 125 },
          { field: 'TaxAmountString', header: 'TaxAmount', width: 125 },
          { field: 'Comment', header: 'Comment', width: 200 },
          { field: 'Project1', header: 'Project1',canSort: true, width: 125 },
          { field: 'Project2', header: 'Project2', width: 125 },
          { field: 'Branch', header: 'Branch', width: 125 },
          { field: 'Bom', header: 'Bom', width: 125 },
          { field: 'WorkGroup', header: 'WorkGroup', width: 125 },
          { field: 'CostCenter', header: 'CostCenter', width: 125 },
          { field: 'Issued', header: 'Issued', width: 125 },
          { field: 'Title', header: 'Title', width: 150 },
          { field: 'TaxinvNo', header: 'TaxinvNo', width: 150 },
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
      } else if (type === "Supplier") {
        this.serviceDialogs.dialogSelectSupplier(this.viewCon)
          .subscribe(sub => {
            this.needReset = true;
            this.reportForm.patchValue({
              WhereSupplier: sub ? sub.SupplierNo : undefined,
              SupplierString: sub ? sub.SupplierName : undefined,
            });
          });
      } 
    }
  }

  // get report data
  onReport(): void {
    if (this.reportForm) {
      let scorll = this.reportForm.getRawValue() as Scroll;

      if (!scorll.WhereProject && !scorll.WhereSupplier && !scorll.Filter && !scorll.SDate && !scorll.EDate) {
        this.serviceDialogs.error("Error Message", `Please select item project or filter befor export.`, this.viewCon);
        return;
      }

      this.loading = true;
      scorll.Skip = 0;
      scorll.Take = this.totalRecords;
      this.service.getXlsx(scorll).subscribe(data => {
        // console.log(data);
        this.loading = false;
      }, () => this.loading = false, () => this.loading = false);
    }
  }

  //filterItemsOfType() {
  //  return this.columns.filter(x => x.type !== ColumnType.Group1 && x.type !== ColumnType.Group2);
  //}
}
