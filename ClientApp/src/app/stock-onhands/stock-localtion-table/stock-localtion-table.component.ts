import { Component, OnInit, Input } from '@angular/core';
import { StockLocation } from '../shared/stock-location.model';
import { MyPrimengColumn } from '../../shared/column.model';

@Component({
  selector: 'app-stock-localtion-table',
  templateUrl: './stock-localtion-table.component.html',
  styleUrls: ['./stock-localtion-table.component.scss']
})
export class StockLocaltionTableComponent implements OnInit {

  constructor() { }

  //Parameter
  @Input() datasource: Array<StockLocation>;
  columns: Array<MyPrimengColumn>;

  ngOnInit() {
    this.columns = new Array;
    this.columns = [
      { field: 'QuantityString', header: 'StockByLocation', width: 140 },
      { field: 'LocationCode', header: 'Location', width: 100 },
      { field: 'Uom', header: 'Uom', width: 75 },
      { field: 'Project', header: 'JobNo', width: 120 },
      { field: 'LotNo', header: 'LotNo/HeatNo', width: 150 },
      { field: 'HeatNo', header: 'HeatNo', width: 120 },
      { field: 'Origin', header: 'Origin', width: 120 },
      { field: 'ExpDateString', header: 'Exp.Date', width: 120 },

    ];
  }

}
