import { Component, OnInit, AfterViewInit } from '@angular/core';
import { BaseTableDetailComponent } from 'src/app/shared2/baseclases/base-table-detail.component';
import { PurchaseLineExtend } from '../shared/purchase-line-extend.model';

@Component({
  selector: 'app-purchase-line-extend-sub-table',
  templateUrl: './purchase-line-extend-sub-table.component.html',
  styleUrls: ['./purchase-line-extend-sub-table.component.scss']
})
export class PurchaseLineExtendSubTableComponent extends BaseTableDetailComponent<PurchaseLineExtend> {

  constructor() {
    super();
    this.columns = [
      { columnName: "PrLine.", columnField: "PrLine", cell: (row: PurchaseLineExtend) => row.PrLine },
      { columnName: "ItemCode", columnField: "ItemCode", cell: (row: PurchaseLineExtend) => row.ItemCode },
      { columnName: "ItemName", columnField: "ItemName", cell: (row: PurchaseLineExtend) => row.ItemName },
      { columnName: "Quantity", columnField: "Quantity", cell: (row: PurchaseLineExtend) => row.Quantity },
      { columnName: "Remark", columnField: "Remark", cell: (row: PurchaseLineExtend) => row.Remark, canEdit: true },
    ];
    this.displayedColumns = this.columns.map(x => x.columnField);
    this.displayedColumns.splice(0, 0, "Command");
  }

  //Parameter

  // on blur
  onBlurText(inputvalue?: any, rowData?: PurchaseLineExtend): void {
    if (rowData) {
      this.returnSelectedWith.emit({
        data: rowData,
        option: 2
      });
    }
  }

  // on blur check
  onBlurCheck(checkValue?: any, rowData?: PurchaseLineExtend): void {
    if (rowData) {
      this.returnSelectedWith.emit({
        data: rowData,
        option: 2
      });
    }
  }
}
