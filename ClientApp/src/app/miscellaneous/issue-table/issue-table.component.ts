import { Component, OnInit, Input } from '@angular/core';
import { Issue } from '../shared/issue.model';
import { MyPrimengColumn } from '../../shared/column.model';

@Component({
  selector: 'app-issue-table',
  templateUrl: './issue-table.component.html',
  styleUrls: ['./issue-table.component.scss']
})
export class IssueTableComponent implements OnInit {

  constructor() { }

  //Parameter
  @Input() datasource: Array<Issue>;
  columns: Array<MyPrimengColumn>;

  ngOnInit() {
    this.columns = new Array;
    this.columns = [
      { field: 'MiscLine', header: 'Line', width: 50 },
      { field: 'ItemCode', header: 'Code', width: 175 },
      { field: 'ItemNameRFT', header: 'Name', width: 300 },
      { field: 'QtyString', header: 'Qty', width: 100 },
      { field: 'Uom', header: 'Uom', width: 100 },
      { field: 'Branch', header: 'Branch', width: 100 },
      { field: 'WorkItem', header: 'Bom', width: 110 },
      { field: 'Project', header: 'Project', width: 110 },
      { field: 'WorkGroup', header: 'WorkGroup', width: 110 },
    ];
  }

}
