import { Component, OnInit } from '@angular/core';
import { BaseTableMK2Component } from 'src/app/shared/base-tablemk2.component';
import { Partner } from 'src/app/dimension-datas/shared/partner.model';
import { CustomerService } from 'src/app/dimension-datas/shared/customer.service';

@Component({
  selector: 'app-partner-table-dialog',
  templateUrl: './partner-table-dialog.component.html',
  styleUrls: ['./partner-table-dialog.component.scss']
})
export class PartnerTableDialogComponent extends BaseTableMK2Component<Partner, CustomerService> {
  constructor(
    service: CustomerService
  ) {
    super(service);
    this.columns = [
      { columnName: "", columnField: "select", cell: undefined },
      { columnName: "Partner No", columnField: "PartnerNo", cell: (row: Partner) => row.PartnerNo },
      { columnName: "Partner Name", columnField: "PartnerName", cell: (row: Partner) => row.PartnerName },
    ];
    this.isSubAction = "GetPartnerScroll/";
    this.displayedColumns = this.columns.map(x => x.columnField);
    this.isDialog = true;
  }
}
