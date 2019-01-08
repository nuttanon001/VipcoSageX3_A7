import { Component, OnInit, Input } from '@angular/core';
import { Journal } from '../shared/journal.model';
import { MyPrimengColumn } from '../../shared/column.model';

@Component({
  selector: 'app-journal-table',
  templateUrl: './journal-table.component.html',
  styleUrls: ['./journal-table.component.scss']
})
export class JournalTableComponent implements OnInit {

  constructor() { }

  //Parameter
  @Input() datasource: Array<Journal>;
  columns: Array<MyPrimengColumn>;

  ngOnInit() {
    this.columns = new Array;
    this.columns = [
      { field: 'AccLine', header: 'Line', width: 75 },
      { field: 'AccountCode', header: 'Code', width: 125 },
      { field: 'AmountCurrencyString', header: 'Currency', width: 150 },
      { field: 'AccountNumber', header: 'AccNumber', width: 125 },
      { field: 'Description', header: 'Desc.', width: 125 },
      { field: 'Branch', header: 'Branch', width: 100 },
      { field: 'WorkItem', header: 'Bom', width: 100 },
      { field: 'Project', header: 'Project', width: 125 },
      { field: 'WorkGroup', header: 'WorkGroup', width: 125 },
    ];
  }

}
