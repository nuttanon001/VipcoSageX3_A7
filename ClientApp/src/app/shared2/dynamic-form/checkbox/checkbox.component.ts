import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FieldConfig } from "../field-config.model";
// Component
@Component({
  selector: "app-checkbox",
  template: `
  <div class="app-checkbox" [formGroup]="group" [ngClass]="{'show-vertival': field.vertival }">
    <mat-checkbox [formControlName]="field.name">
      {{field.label}}
    </mat-checkbox>
  </div>
`,
  styles: [`
  .app-checkbox
    {
      display: flex;
      flex-wrap: wrap;
      justify-content: left;
      flex-flow: row;
      margin: 5px;
    }
  .show-vertival
    {
      flex-flow: column;
    }
`]
})
export class CheckboxComponent implements OnInit {
  field: FieldConfig;
  group: FormGroup;
  constructor() { }
  ngOnInit() { }
}
