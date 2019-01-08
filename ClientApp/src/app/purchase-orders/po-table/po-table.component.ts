import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseTableMK2Component } from '../../shared/base-tablemk2.component';
import { PurchaseOrder } from '../shared/purchase-order.model';
import { PurchaseOrderService } from '../shared/purchase-order.service';
import { Workgroup } from '../../dimension-datas/shared/workgroup.model';
import { BomLevel } from '../../dimension-datas/shared/bom-level.model';
import { ProjectCode } from '../../dimension-datas/shared/project-code.model';
import { ProjectCodeService } from '../../dimension-datas/shared/project-code.service';
import { WorkgroupService } from '../../dimension-datas/shared/workgroup.service';
import { BomLevelService } from '../../dimension-datas/shared/bom-level.service';
import { MatSelect } from '@angular/material';
import { Scroll } from '../../shared/scroll.model';
import { SelectionModel } from '@angular/cdk/collections';
// Rxjs
import {
  map, startWith,
  switchMap, catchError,
} from "rxjs/operators";
import { of as observableOf, merge, Observable } from 'rxjs';


@Component({
  selector: 'app-po-table',
  templateUrl: './po-table.component.html',
  styleUrls: ['./po-table.component.scss']
})
export class PoTableComponent extends BaseTableMK2Component<PurchaseOrder,PurchaseOrderService> {

  constructor(
    service: PurchaseOrderService,
    private serviceProject: ProjectCodeService,
    private serviceWorkGroup: WorkgroupService,
    private serviceBomLevel: BomLevelService,
  ) {
    super(service);
    this.columns = [
      { columnName: "", columnField: "select", cell: undefined },
      { columnName: "Po/No", columnField: "PurchaseOrderNo", cell: (row: PurchaseOrder) => row.PurchaseOrderNo },
      { columnName: "Po Date", columnField: "OrderDateString", cell: (row: PurchaseOrder) => row.OrderDateString },
      { columnName: "Job/No", columnField: "ProjectName", cell: (row: PurchaseOrder) => row.ProjectCode },
      { columnName: "Supplier", columnField: "SupplierName", cell: (row: PurchaseOrder) => row.SupplierName },
      { columnName: "WorkGroup", columnField: "WorkGroupName", cell: (row: PurchaseOrder) => row.WorkGroupName },
      { columnName: "Bom/Lv2", columnField: "WorkItemName", cell: (row: PurchaseOrder) => row.WorkItemName },
      { columnName: "Received", columnField: "ReceivedStatusString", cell: (row: PurchaseOrder) => row.ReceivedStatusString },
      { columnName: "Po/Close", columnField: "CloseStatusString", cell: (row: PurchaseOrder) => row.CloseStatusString },
    ];

    this.displayedColumns = this.columns.map(x => x.columnField);
    this.displayedColumns.push("Command");
  }
  @ViewChild("selectWorkGroup") selectWorkGroup: MatSelect;
  @ViewChild("selectBomLevel") selectBomLevel: MatSelect;
  @ViewChild("selectProjectCode") selectProjectCode: MatSelect;
  //Parameter
  workGroups: Array<Workgroup>;
  bomLevels: Array<BomLevel>;
  projectCodes: Array<ProjectCode>;

  ngOnInit(): void {
    if (!this.workGroups) {
      this.workGroups = new Array;
      this.serviceWorkGroup.getAll().subscribe((workGroupDb: Array<Workgroup>) => {
        this.workGroups = workGroupDb.slice();
      });
    }
    if (!this.bomLevels) {
      this.bomLevels = new Array;
      this.serviceBomLevel.getAll().subscribe((bomLevelDb: Array<BomLevel>) => {
        this.bomLevels = bomLevelDb.slice();
      });
    }
    if (!this.projectCodes) {
      this.projectCodes = new Array;
      this.serviceProject.getAll().subscribe((projectCodeDb: Array<ProjectCode>) => {
        this.projectCodes = projectCodeDb.slice();
      });
    }

    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
    this.searchBox.search.subscribe(() => this.paginator.pageIndex = 0);
    this.selectWorkGroup.selectionChange.subscribe(() => { this.paginator.pageIndex = 0 });
    this.selectBomLevel.selectionChange.subscribe(() => { this.paginator.pageIndex = 0 });
    this.selectProjectCode.selectionChange.subscribe(() => { this.paginator.pageIndex = 0 });

    merge(this.sort.sortChange, this.paginator.page, this.searchBox.search, this.searchBox.onlyCreate,
      this.selectWorkGroup.selectionChange, this.selectBomLevel.selectionChange,
      this.selectProjectCode.selectionChange)
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
            Where: `${(this.selectWorkGroup.value || "-")};${(this.selectBomLevel.value || "-")};${(this.selectProjectCode.value || "-")}`,
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
      ).subscribe(data => this.dataSource.data = data);

    // Selection
    this.selection = new SelectionModel<PurchaseOrder>(this.isMultiple, [], true)
    this.selection.onChange.subscribe(selected => {
      if (this.isMultiple) {
        if (selected.source) {
          // this.selectedRow = selected.source.selected[0];
          this.returnSelectesd.emit(selected.source.selected);
        }
      } else {
        if (selected.source.selected[0]) {
          this.selectedRow = selected.source.selected[0];
          this.returnSelected.emit(selected.source.selected[0]);
        }
      }
    });
  }
}
