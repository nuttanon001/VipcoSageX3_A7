import { Component, OnInit } from '@angular/core';
import { BaseTableMK2Component } from 'src/app/shared/base-tablemk2.component';
import { Customer } from 'src/app/dimension-datas/shared/customer.model';
import { CustomerService } from 'src/app/dimension-datas/shared/customer.service';

@Component({
  selector: 'app-customer-table-dialog',
  templateUrl: './customer-table-dialog.component.html',
  styleUrls: ['./customer-table-dialog.component.scss']
})
export class CustomerTableDialogComponent extends BaseTableMK2Component<Customer, CustomerService> {
  constructor(
    service: CustomerService
  ) {
    super(service);
    this.columns = [
      { columnName: "", columnField: "select", cell: undefined },
      { columnName: "Bank No", columnField: "BankNumber", cell: (row: Customer) => row.CustomerNo },
      { columnName: "Bank Name", columnField: "Description", cell: (row: Customer) => row.CustomerName },
    ];

    this.displayedColumns = this.columns.map(x => x.columnField);
    this.isDialog = true;
  }
}
