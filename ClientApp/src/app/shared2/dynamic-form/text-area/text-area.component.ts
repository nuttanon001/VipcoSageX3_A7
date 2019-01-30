import { Component, OnInit } from '@angular/core';
import { FieldConfig } from '../field-config.model';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-text-area',
  template: `
 <mat-form-field [formGroup]="group" class="app-text-area">
    <textarea matInput [formControlName]="field.name" [placeholder]="field.label">
    </textarea>
    <ng-container *ngFor="let validation of field.validations;" ngProjectAs="mat-error">
      <mat-error *ngIf="group.get(field.name).hasError(validation.name)">
        {{validation.message}}
      </mat-error>
    </ng-container>
  </mat-form-field>
  `,
  styles: [`
 .app-text-area {
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
    .app-text-area
    {
      margin: 5px;
      width:100%;

      mat-form-field {
        width: 90%;
        min-height:50px;
        margin:5px;
      }
    }
  }
`]
})
export class TextAreaComponent implements OnInit {
  field: FieldConfig;
  group: FormGroup;

  constructor() { }
  ngOnInit() { }
}
