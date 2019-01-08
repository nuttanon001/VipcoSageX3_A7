import { Component, OnInit, Input } from '@angular/core';
import { PurchaseReceipt } from '../shared/purchase-receipt.model';
import { MyPrimengColumn } from '../../shared/column.model';

@Component({
  selector: 'app-pr-table',
  templateUrl: './pr-table.component.html',
  styleUrls: ['./pr-table.component.scss']
})
export class PrTableComponent implements OnInit {

  constructor() { }

  //Parameter
  @Input() datasource: Array<PurchaseReceipt>;
  columns: Array<MyPrimengColumn>;

  ngOnInit() {
    this.columns = new Array;
    let width100: number = 100;
    let width150: number = 150;

    this.columns = [
      { field: 'RcNumber', header: 'RecNo.', width: width150 },
      { field: 'RcStatus', header: 'Status', width: 100 },
      { field: 'HeatNumber', header: 'HeatNo', width: width150 },
      { field: 'RcProject', header: 'JobNo.', width: width150 },
      { field: 'RcDateString', header: 'Date', width: width100 },
      { field: 'RcQuantityPur', header: 'Qty.', width: width100 },
      { field: 'RcQuantityWeight', header: 'Weight.', width: width100 },
      { field: 'RcPurUom', header: 'Uom', width: width100},
      { field: 'RcBranch', header: 'Branch', width: width100},
      { field: 'RcWorkItemName', header: 'BomLv', width: width150 },
      { field: 'RcWorkGroupName', header: 'WorkGroup', width: width150 },
    ];
  }

}
