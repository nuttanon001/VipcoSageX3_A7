import { Component, OnInit } from '@angular/core';
import { BaseTableComponent } from 'src/app/shared2/baseclases/base-table.component';
import { PurchaseExtend } from '../shared/purchase-extend.model';
import { PurchaseExtendService } from '../shared/purchase-extend.service';
import { AuthService } from 'src/app/core/auth/auth.service';
import * as moment from "moment";

@Component({
  selector: 'app-purchase-extend-table',
  templateUrl: './purchase-extend-table.component.html',
  styleUrls: ['./purchase-extend-table.component.scss']
})
export class PurchaseExtendTableComponent extends BaseTableComponent<PurchaseExtend, PurchaseExtendService> {

  constructor(
    service: PurchaseExtendService,
    serviceAuth: AuthService
  ) {
    super(service, serviceAuth);

    this.columns = [
      { columnName: "PRNumber", columnField: "PRNumber", cell: (row: PurchaseExtend) => row.PRNumber },
      { columnName: "ReceivedDate", columnField: "PrReceivedDate", cell: (row: PurchaseExtend) => moment(row.PrReceivedDate).format("DD-MM-YYYY") + row.PrReceivedTime },
      { columnName: "Remark", columnField: "Remark", cell: (row: PurchaseExtend) => row.Remark },
    ];
    this.displayedColumns = this.columns.map(x => x.columnField);
    //this.displayedColumns.splice(0, 0, "select");
    this.displayedColumns.splice(0, 0, "Command");
    this.isDisabled = true;
  }
}
