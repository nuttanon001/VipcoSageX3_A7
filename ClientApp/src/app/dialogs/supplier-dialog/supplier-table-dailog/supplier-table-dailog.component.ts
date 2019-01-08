import { Component, OnInit } from '@angular/core';
import { BaseTableMK2Component } from '../../../shared/base-tablemk2.component';
import { Supplier } from '../../../dimension-datas/shared/supplier.model';
import { SupplierService } from '../../../dimension-datas/shared/supplier.service';

@Component({
  selector: 'app-supplier-table-dailog',
  templateUrl: './supplier-table-dailog.component.html',
  styleUrls: ['./supplier-table-dailog.component.scss']
})
export class SupplierTableDailogComponent extends BaseTableMK2Component<Supplier, SupplierService> {

  constructor(
    service: SupplierService
  ) {
    super(service);
    this.columns = [
      { columnName: "", columnField: "select", cell: undefined },
      { columnName: "Supplier Code", columnField: "SupplierNo", cell: (row: Supplier) => row.SupplierNo },
      { columnName: "Supplier Name", columnField: "SupplierName", cell: (row: Supplier) => row.SupplierName },
    ];

    this.displayedColumns = this.columns.map(x => x.columnField);
    this.isDialog = true;
  }
}
