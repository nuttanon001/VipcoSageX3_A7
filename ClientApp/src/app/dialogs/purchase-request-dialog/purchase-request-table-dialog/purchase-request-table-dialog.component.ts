import { Component, OnInit } from '@angular/core';
import { BaseTableComponent } from 'src/app/shared2/baseclases/base-table.component';
import { PurchaseRequestPure } from 'src/app/purchase-extends/shared/purchase-request-pure.model';
import { PurchaseExtendService } from 'src/app/purchase-extends/shared/purchase-extend.service';
import { AuthService } from 'src/app/core/auth/auth.service';
import * as moment from "moment";

@Component({
  selector: 'app-purchase-request-table-dialog',
  templateUrl: './purchase-request-table-dialog.component.html',
  styleUrls: ['./purchase-request-table-dialog.component.scss']
})
export class PurchaseRequestTableDialogComponent
  extends BaseTableComponent<PurchaseRequestPure, PurchaseExtendService>{

  constructor(service: PurchaseExtendService, servierAuth: AuthService) {
    super(service, servierAuth);

    this.columns = [
      { columnName: "No.", columnField: "PrNumber", cell: (row: PurchaseRequestPure) => row.PrNumber },
      { columnName: "Type", columnField: "PrType", cell: (row: PurchaseRequestPure) => row.PrType },
      { columnName: "StatusClose", columnField: "StatusClose", cell: (row: PurchaseRequestPure) => row.StatusClose },
      { columnName: "StatusOrder", columnField: "StatusOrder", cell: (row: PurchaseRequestPure) => row.StatusOrder },
      { columnName: "Date", columnField: "PrDate", cell: (row: PurchaseRequestPure) => moment(row.PrDate).format("DD-MM-YYYY")},
    ];
    this.displayedColumns = this.columns.map(x => x.columnField);
    this.displayedColumns.splice(0, 0, "select");
    //this.displayedColumns.splice(0, 0, "Command");
    this.isDisabled = true;
    this.isKeyIndex = "PrSageHeaderId";
    this.isSubAction = "PurchaseExtendScroll/";
  }
}
