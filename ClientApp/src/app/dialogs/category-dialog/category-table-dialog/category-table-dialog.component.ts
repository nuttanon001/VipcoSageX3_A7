import { Component, OnInit } from '@angular/core';
import { Category } from '../../../dimension-datas/shared/category.model';
import { CategoryService } from '../../../dimension-datas/shared/category.service';
import { BaseTableMK2Component } from '../../../shared/base-tablemk2.component';

@Component({
  selector: 'app-category-table-dialog',
  templateUrl: './category-table-dialog.component.html',
  styleUrls: ['./category-table-dialog.component.scss']
})
export class CategoryTableDialogComponent extends BaseTableMK2Component<Category, CategoryService> {

  constructor(
    service: CategoryService
  ) {
    super(service);
    this.columns = [
      { columnName: "", columnField: "select", cell: undefined },
      { columnName: "Category Code", columnField: "CategoryCode", cell: (row: Category) => row.CategoryCode },
      { columnName: "Category Name", columnField: "CategoryName", cell: (row: Category) => row.CategoryName },
    ];

    this.displayedColumns = this.columns.map(x => x.columnField);
    this.isDialog = true;
  }
}
