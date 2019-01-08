import { Component, Inject } from '@angular/core';
import { Category } from '../../dimension-datas/shared/category.model';
import { CategoryService } from '../../dimension-datas/shared/category.service';
import { BaseDialogComponent } from '../../shared/base-dialog.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-category-dialog',
  templateUrl: './category-dialog.component.html',
  styleUrls: ['./category-dialog.component.scss'],
  providers: [CategoryService]
})
export class CategoryDialogComponent extends BaseDialogComponent<Category, CategoryService> {
  /** employee-dialog ctor */
  constructor(
    public service: CategoryService,
    public dialogRef: MatDialogRef<CategoryDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public mode: number
  ) {
    super(
      service,
      dialogRef
    );
  }
  // on init
  onInit(): void {
    this.fastSelectd = this.mode === 0 ? true : false;
  }
}
