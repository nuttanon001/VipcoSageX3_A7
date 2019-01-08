import { Component, OnInit } from '@angular/core';
import { BaseTableMK2Component } from '../../../shared/base-tablemk2.component';
import { BomLevel } from '../../../dimension-datas/shared/bom-level.model';
import { BomLevelService } from '../../../dimension-datas/shared/bom-level.service';

@Component({
  selector: 'app-bom-table-dialog',
  templateUrl: './bom-table-dialog.component.html',
  styleUrls: ['./bom-table-dialog.component.scss']
})
export class BomTableDialogComponent extends BaseTableMK2Component<BomLevel, BomLevelService> {

  constructor(
    service: BomLevelService
  ) {
    super(service);
    this.columns = [
      { columnName: "", columnField: "select", cell: undefined },
      { columnName: "Bom Code", columnField: "BomLevelCode", cell: (row: BomLevel) => row.BomLevelCode },
      { columnName: "Bom Name", columnField: "BomLevelName", cell: (row: BomLevel) => row.BomLevelName },
    ];

    this.displayedColumns = this.columns.map(x => x.columnField);
    this.isDialog = true;
  }
}
