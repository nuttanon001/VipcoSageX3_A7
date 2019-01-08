import { Component, OnInit } from '@angular/core';
import { BaseTableMK2Component } from '../../../shared/base-tablemk2.component';
import { Bank } from '../../../dimension-datas/shared/bank.model';
import { BankService } from '../../../dimension-datas/shared/bank.service';

@Component({
  selector: 'app-bank-table-dialog',
  templateUrl: './bank-table-dialog.component.html',
  styleUrls: ['./bank-table-dialog.component.scss']
})
export class BankTableDialogComponent extends BaseTableMK2Component<Bank, BankService> {

  constructor(
    service: BankService
  ) {
    super(service);
    this.columns = [
      { columnName: "", columnField: "select", cell: undefined },
      { columnName: "Bank No", columnField: "BankNumber", cell: (row: Bank) => row.BankNumber },
      { columnName: "Bank Name", columnField: "Description", cell: (row: Bank) => row.Description },
    ];

    this.displayedColumns = this.columns.map(x => x.columnField);
    this.isDialog = true;
  }
}
