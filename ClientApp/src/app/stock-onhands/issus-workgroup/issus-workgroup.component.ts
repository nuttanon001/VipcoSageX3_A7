import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { BaseScheduleComponent } from 'src/app/shared/base-schedule.component';
import { IssusWorkgroup } from '../shared/issus-workgroup.model';
import { StockOnhandService } from '../shared/stock-onhand.service';
import { FormBuilder } from '@angular/forms';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';
import { AuthService } from 'src/app/core/auth/auth.service';
import { Router } from '@angular/router';
import { ScrollData } from 'src/app/shared/scroll-data.model';
import { Scroll } from 'src/app/shared/scroll.model';
import { Workgroup } from 'src/app/dimension-datas/shared/workgroup.model';
import { ProjectCode } from 'src/app/dimension-datas/shared/project-code.model';
import { Branch } from 'src/app/dimension-datas/shared/branch.model';

@Component({
  selector: 'app-issus-workgroup',
  templateUrl: './issus-workgroup.component.html',
  styleUrls: ['./issus-workgroup.component.scss']
})

export class IssusWorkgroupComponent extends BaseScheduleComponent<IssusWorkgroup, StockOnhandService> {
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
    this.sizeForm = 290;
    this.mobHeight = (window.innerHeight - this.sizeForm) + "px";

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

    this.service.getAllWithScroll(schedule, "IssusWorkGroupGetScroll/")
      .subscribe((dbData: ScrollData<IssusWorkgroup>) => {
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
          { field: 'ItemNo', header: 'ItemCode', width: 175, canSort: true },
          { field: 'TextName', header: 'TextName', width: 350, canSort: true },
          { field: 'Branch', header: 'Branch', width: 150, canSort: true },
          { field: 'Project', header: 'Project', width: 150, canSort: true },
          { field: 'WorkGroup', header: 'WorkGroup', width: 150, canSort: true },
          { field: 'Uom', header: 'Uom', width: 120, canSort: true },
          { field: 'QuantityString', header: 'Quantity', width: 150, canSort: true },
          { field: 'UnitPriceString', header: 'UnitPrice', width: 120 },
          { field: 'TotalCostString', header: 'TotalCost', width: 150 },
        ];

        if (dbData.Data) {
          this.datasource = dbData.Data.sort((left, right) => {
            if (left.WorkGroup > right.WorkGroup) return 1;
            if (left.WorkGroup < right.WorkGroup) return -1;
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
      if (type === "WorkGroup") {
        this.serviceDialogs.dialogSelectGroup(this.viewCon, 1)
          .subscribe((workGroups: Array<Workgroup>) => {
            let nameWorkGroups: string = "";
            if (workGroups) {
              nameWorkGroups = (workGroups[0].WorkGroupName.length > 21 ? workGroups[0].WorkGroupName.slice(0, 21) + "..." : workGroups[0].WorkGroupName) +
                (workGroups.length > 1 ? `+ ${workGroups.length - 1} others` : "");
              //--------------------//
            }
            this.needReset = true;
            this.reportForm.patchValue({
              WhereBanks: workGroups ? workGroups.map((item) => item.WorkGroupCode) : undefined,
              BankString: workGroups ? nameWorkGroups : undefined,
            });
          });
      } else if (type === "Branch") {
        this.serviceDialogs.dialogSelectBranch(this.viewCon, 1)
          .subscribe((branchs: Array<Branch>) => {
            let nameBranch: string = "";
            if (branchs) {
              nameBranch = (branchs[0].BranchName.length > 21 ? branchs[0].BranchName.slice(0, 21) + "..." : branchs[0].BranchName) +
                (branchs.length > 1 ? `+ ${branchs.length - 1} others` : "");
              //--------------------//
            }
            this.needReset = true;
            this.reportForm.patchValue({
              WhereBranchs: branchs ? branchs.map((item) => item.BranchCode) : undefined,
              BranchString: branchs ? nameBranch : undefined,
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

      if (!scorll.WhereProjects && !scorll.Filter && !scorll.WhereBanks && !scorll.SDate && !scorll.EDate
        && !scorll.WhereRange11 && !scorll.WhereRange12 && !scorll.WhereRange21 && !scorll.WhereRange22
        && !scorll.WhereBranchs) {
        this.serviceDialogs.error("Error Message", `Please select item project or filter befor export.`, this.viewCon);
        return;
      }

      this.loading = true;
      scorll.Skip = 0;
      scorll.Take = this.totalRecords;
      this.service.getXlsx(scorll, "IssusWorkGroupGetReport/").subscribe(data => {
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
        let WorkGroup = rowData.WorkGroup;
        if (i == 0) {
          this.rowGroupMetadata[WorkGroup] = { index: 0, size: 1 };
        }
        else {
          let previousRowData = this.datasource[i - 1];
          let previousRowGroup = previousRowData.WorkGroup;
          if (WorkGroup === previousRowGroup)
            this.rowGroupMetadata[WorkGroup].size++;
          else
            this.rowGroupMetadata[WorkGroup] = { index: i, size: 1 };
        }
      }
    }
  }

}
