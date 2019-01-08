// Angular Core
import { OnInit, ViewChild, Input, Output, EventEmitter, OnDestroy } from "@angular/core";
import { MatPaginator, MatSort, MatTableDataSource, MatCheckbox } from "@angular/material";
import { SelectionModel } from '@angular/cdk/collections';
// Rxjs
import {
  map, startWith,
  switchMap, catchError,
} from "rxjs/operators";
import { of as observableOf, merge, Observable } from 'rxjs';
// Models
import { Scroll } from "./scroll.model";
// Component
import { SearchBoxComponent } from "./search-box/search-box.component";
// Services
import { BaseRestService } from "./base-rest.service";
import { ResueaColumn } from "./resuea-column.model";
//rxjs

/** custom-mat-table component*/
export class BaseTableMK2Component<Model, Service extends BaseRestService<Model>> implements OnInit {
  /** custom-mat-table ctor */
  constructor(
    protected service: Service,
  ) { }

  // Parameter
  //columns: Array<ResueaColumn<Model>>;
  columns: any;
  displayedColumns: Array<string> = ["select", "Col1", "Col2", "Col3"];
  @Input() isOnlyCreate: boolean = false;
  @Input() isDisabled: boolean = true;
  @Input() isMultiple: boolean = false;
  @Input() isDialog: boolean = false;
  @Input() WhereId: number | undefined;
  @Input() Where: string | undefined;
  @Input() isSubAction: string = "GetScroll/";
  @Output() returnSelected: EventEmitter<Model> = new EventEmitter<Model>();
  @Output() returnSelectesd: EventEmitter<Array<Model>> = new EventEmitter<Array<Model>>();
  @Output() returnSelectedWith: EventEmitter<{ data: Model, option: number }> = new EventEmitter<{ data: Model, option: number }>();

  // Parameter MatTable
  dataSource = new MatTableDataSource<Model>();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(SearchBoxComponent) searchBox: SearchBoxComponent;
  selection: SelectionModel<Model>;
  subscribe: Observable<any>;
  // Parameter Component
  templateSelect: Array<Model>;
  resultsLength = 0;
  isLoadingResults = true;
  selectedRow: Model;

  // Angular NgOnInit
  ngOnInit() {
    this.templateSelect = new Array;
    this.searchBox.onlyCreate2 = this.isOnlyCreate;
    // If the user changes the sort order, reset back to the first page.
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
    this.searchBox.search.subscribe(() => this.paginator.pageIndex = 0);
    // Merge
    //, this.searchBox.search, this.searchBox.onlyCreate

    merge(this.sort.sortChange, this.paginator.page, this.searchBox.search, this.searchBox.onlyCreate)
      .pipe(
      startWith({}),
      switchMap(() => {
        this.isLoadingResults = true;
        let scroll: Scroll = {
          Skip: this.paginator.pageIndex * this.paginator.pageSize,
          Take: this.paginator.pageSize || 10,
          Filter: this.searchBox.search2,
          SortField: this.sort.active,
          SortOrder: this.sort.direction === "desc" ? 1 : -1,
          Where:  "",
          WhereId: this.WhereId
        };
        return this.service.getAllWithScroll(scroll, this.isSubAction);
      }),
      map(data => {
        // Flip flag to show that loading has finished.
        this.isLoadingResults = false;
        this.resultsLength = data.Scroll.TotalRow;
        return data.Data;
      }),
      catchError(() => {
        this.isLoadingResults = false;
        // Catch if the GitHub API has reached its rate limit. Return empty data.
        return observableOf([]);
      })
    ).subscribe(data => {
      this.dataSource.data = data;
      // Addtion
      if (this.templateSelect && this.templateSelect.length > 0) {
        this.dataSource.data.forEach(row => {
          if (deepIndexOf(this.templateSelect, row) !== -1) {
            this.selection.select(row)
          }
        });
      }
    });

    // Selection
    this.selection = new SelectionModel<Model>(this.isMultiple, [], true)
    this.selection.onChange.subscribe(selected => {
      // Added
      if (selected.added && selected.added.length > 0) {
        if (this.isMultiple) {
          selected.added.forEach(item => {
            if (deepIndexOf(this.templateSelect, item) === -1) {
              // console.log("Add");
              this.templateSelect.push(Object.assign({}, item));
            }
            this.selectedRow = item;
          });
          this.returnSelectesd.emit(this.templateSelect);
        } else {
          if (selected.added[0]) {
            this.selectedRow = selected.added[0];
            this.returnSelected.emit(selected.added[0]);
          }
        }
      }
      // Remove
      if (selected.removed && selected.removed.length > 0) {
        selected.removed.forEach(item => {
          if (this.templateSelect && this.templateSelect.length > 0) {
            let index = deepIndexOf(this.templateSelect, item);
            // console.log("Remove", index);
            this.templateSelect.splice(index, 1);
          }

          if (item === this.selectedRow) {
            this.selectedRow = undefined;
          }
        });
      }

      //if (this.isMultiple) {
      //  if (selected.source) {
      //    // this.selectedRow = selected.source.selected[0];
      //    this.returnSelectesd.emit(selected.source.selected);
      //  }
      //} else {
      //  if (selected.source.selected[0]) {
      //    this.selectedRow = selected.source.selected[0];
      //    this.returnSelected.emit(selected.source.selected[0]);
      //  }
      //}
    });
  }

  // Reload
  reloadData(): void {
    //this.paginator.page.emit();
    this.searchBox.search.emit("");
  }
  // OnAction Click
  onActionClick(data: Model, option: number = 0) {
    this.returnSelectedWith.emit({ data: data, option: option });
  }
}

function deepIndexOf(arr, obj) {
  return arr.findIndex(function (cur) {
    return Object.keys(obj).every(function (key) {
      return obj[key] === cur[key];
    });
  });
}
