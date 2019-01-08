import { Component, OnInit, Input } from '@angular/core';
import { StockMovement2 } from '../shared/stock-movement2.model';
import { MyPrimengColumn } from '../../shared/column.model';

@Component({
  selector: 'app-stock-movement2-table',
  templateUrl: './stock-movement2-table.component.html',
  styleUrls: ['./stock-movement2-table.component.scss']
})
export class StockMovement2TableComponent implements OnInit {

  constructor() { }

  //Parameter
  @Input() datasource: Array<StockMovement2>;
  columns: Array<MyPrimengColumn>;

  ngOnInit() {
    this.columns = new Array;
    this.columns = [
      { field: 'DocNo', header: 'DocNo', width: 150 },
      { field: 'MovementType', header: 'Status', width: 125 },
      { field: 'MovementDateString', header: 'Date', width: 110 },
      { field: 'QuantityInString', header: 'MoveIn', width: 85 },
      { field: 'QuantityOutString', header: 'MoveOut', width: 110 },
      { field: 'Location', header: 'Location', width: 150 },
      { field: 'Bom', header: 'Bom', width: 100 },
      { field: 'Project', header: 'JobNo', width: 100 },
      { field: 'WorkGroup', header: 'Group', width: 100 },
    ];
  }

}
