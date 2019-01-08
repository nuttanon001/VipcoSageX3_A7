// Angular Core
import { Component, OnInit, ViewChild, Input, Output, EventEmitter, AfterViewInit, OnChanges } from "@angular/core";
import { MatPaginator, MatSort, MatTableDataSource, MatCheckbox, MatTable } from "@angular/material";
import { SelectionModel } from '@angular/cdk/collections';
// Rxjs
import {
  map, startWith,
  switchMap, catchError,
} from "rxjs/operators";
import { of as observableOf, merge, Observable } from 'rxjs';
// Module

export class BaseTableFontData<Model> implements OnInit, OnChanges, AfterViewInit {
  /** custom-mat-table ctor */
  constructor() { }

  // Parameter
  displayedColumns: Array<string> = ["select", "Col1", "Col2", "Col3"];
  @Input() dataRows: Array<Model>;
  @Input() readOnly: boolean = false;
  @Input() fastSelected: boolean = false;
  @Output() returnSelected: EventEmitter<{ data: Model, option: number }> = new EventEmitter<{ data: Model, option: number }>();

  // Parameter MatTable
  dataSource = new MatTableDataSource<Model>();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  selection = new SelectionModel<Model>(false, [], true);

  // Parameter Component
  resultsLength = 0;
  selectedRow: Model;

  ngOnChanges() {
    this.dataSource = new MatTableDataSource<Model>(this.dataRows);
    this.resultsLength = this.dataRows ? this.dataRows.length : 0;
   
    //this.dataSource.connect().subscribe(data => {
    //  console.log(JSON.stringify(data));
    //});
  }

  // Angular NgOnInit
  ngOnInit() {
    // this.dataSource = new MatTableDataSource<Model>(new Array);
    // If the user changes the sort order, reset back to the first page.
    this.selection.onChange.subscribe(selected => {
      if (selected.source.selected[0]) {
        this.selectedRow = selected.source.selected[0];
        if (this.fastSelected) {
          this.returnSelected.emit({ data: this.selectedRow, option: 1});
        }
      }
    });
  }
  // Angular ngAfterViewInit
  ngAfterViewInit() {
    // this.dataSource = new MatTableDataSource<Model>(new Array<Model>());
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  // OnAction Click
  onActionClick(data: Model, option: number = 0) {
    this.returnSelected.emit({ data: data, option: option });
  }
}

