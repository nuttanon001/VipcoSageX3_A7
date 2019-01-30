import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FieldConfig } from "../field-config.model";

@Component({
  selector: "app-select",
  template: `
  <mat-form-field [formGroup]="group" class="app-select">
    <mat-select [placeholder]="field.label" [formControlName]="field.name">
      <mat-option *ngFor="let item of field.options" [value]="item.value" style="margin-left:5px;">
        {{item.label}}
      </mat-option>
    </mat-select>
  </mat-form-field>
`,
  styles: [`
 .app-select {
    width: 45%;
    margin: 5px;

    mat-form-field {
      width: 90%;
      min-height:50px;
      margin:5px;
    }
  }

  @media(max-width: 600px)
  {
    .app-select {
      width:100%;
      margin: 5px;

      mat-form-field {
        width: 90%;
        min-height:50px;
        margin:5px;
      }
    }
  }

`]
})
export class SelectComponent implements OnInit {
  field: FieldConfig;
  group: FormGroup;
  constructor() { }
  ngOnInit() { }
}
