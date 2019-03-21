import { Component, OnInit } from '@angular/core';
import { BaseTableMK2Component } from 'src/app/shared/base-tablemk2.component';
import { Branch } from 'src/app/dimension-datas/shared/branch.model';
import { BankService } from 'src/app/dimension-datas/shared/bank.service';

@Component({
  selector: 'app-branch-table-dialog',
  templateUrl: './branch-table-dialog.component.html',
  styleUrls: ['./branch-table-dialog.component.scss']
})
export class BranchTableDialogComponent
  extends BaseTableMK2Component<Branch, BankService> {

  constructor(
    service: BankService
  ) {
    super(service);
    this.columns = [
      { columnName: "", columnField: "select", cell: undefined },
      { columnName: "Branch No", columnField: "BranchCode", cell: (row: Branch) => row.BranchCode },
      { columnName: "Branch Name", columnField: "BranchName", cell: (row: Branch) => row.BranchName },
    ];
    this.isSubAction = "GetBranchScroll/";
    this.displayedColumns = this.columns.map(x => x.columnField);
    this.isDialog = true;
  }
}
