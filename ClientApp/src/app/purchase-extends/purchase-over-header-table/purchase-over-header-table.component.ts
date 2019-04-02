import { Component, OnInit } from '@angular/core';
import { BaseTableComponent } from 'src/app/shared2/baseclases/base-table.component';
import { PurchaseOverHeader } from '../shared/purchase-over-header.model';
import { PurchaseOverHeaderService } from '../shared/purchase-over-header.service';
import { AuthService } from 'src/app/core/auth/auth.service';
import * as moment from "moment";


@Component({
  selector: 'app-purchase-over-header-table',
  templateUrl: './purchase-over-header-table.component.html',
  styleUrls: ['./purchase-over-header-table.component.scss']
})
export class PurchaseOverHeaderTableComponent extends BaseTableComponent<PurchaseOverHeader, PurchaseOverHeaderService> {

  constructor(
    service: PurchaseOverHeaderService,
    serviceAuth: AuthService
  ) {
    super(service, serviceAuth);

    this.columns = [
      { columnName: "ReceivedDate", columnField: "PrReceivedDate", cell: (row: PurchaseOverHeader) => moment(row.PrReceivedDate).format("DD-MM-YYYY") + row.PrReceivedTime },
      { columnName: "Remark", columnField: "Remark", cell: (row: PurchaseOverHeader) => row.Remark },
    ];
    this.displayedColumns = this.columns.map(x => x.columnField);
    //this.displayedColumns.splice(0, 0, "select");
    this.displayedColumns.splice(0, 0, "Command");
    this.isDisabled = true;
  }
}
