import { FormBuilder, FormGroup } from '@angular/forms';
import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
// 3rd Patry
import * as moment from "moment";
import { distinctUntilChanged, debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-date-pick-dialog',
  templateUrl: './date-pick-dialog.component.html',
  styleUrls: ['./date-pick-dialog.component.scss']
})
export class DatePickDialogComponent implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<DatePickDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public mode: number,
    private fb:FormBuilder
  ) { }

  //Parameter
  dialogForm: FormGroup;
  datePick: { SDate: Date, EDate : Date };
  ngOnInit() {
    this.bulidForm();
  }

  bulidForm() {
    if (!this.datePick) {
      this.datePick = {
        SDate: moment().subtract(30, "d").toDate(),
        EDate: moment().toDate(),
        };
      }

    this.dialogForm = this.fb.group({
      SDate: [this.datePick.SDate],
      EDate: [this.datePick.EDate],
      });

    this.dialogForm.valueChanges.pipe(debounceTime(250), distinctUntilChanged())
        .subscribe((data: any) => this.onValueChanged(data));
  }

  onValueChanged(data?: any): void {
    if (!this.dialogForm) { return; }
    this.datePick = this.dialogForm.value;
  }

  // No Click
  onCancelClick(): void {
    this.dialogRef.close();
  }

  // Update Click
  onSelectedClick(): void {
    if (this.datePick) {
      this.dialogRef.close(this.datePick);
    }
  }
}
