import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  OnDestroy,
  Output
} from "@angular/core";
import {
  FormGroup,
  FormBuilder,
  Validators,
  FormControl
} from "@angular/forms";
import { FieldConfig, Validator } from "../field-config.model";
import { debounceTime, distinctUntilChanged } from "rxjs/operators";


@Component({
  exportAs: "dynamicForm",
  selector: "dynamic-form",
  template: `
    <form [formGroup]="form" (submit)="onSubmit($event)">
      <ng-container *ngFor="let field of fields;">
        <ng-container *ngIf="!field.hidden"  dynamicField [field]="field" [group]="form">
        </ng-container>
      </ng-container>
    </form>
  `,
  styles: []
})
export class DynamicFormComponent implements OnInit {
  @Input() fields: FieldConfig[] = [];
  @Output() submit: EventEmitter<any> = new EventEmitter<any>();
  form: FormGroup;
  get value() {
    return this.form.value;
  }
  constructor(
    private fb: FormBuilder,
  ) { }

  // OnInit
  ngOnInit() {
    this.form = this.createControl();
    // console.log(JSON.stringify(this.form));

    // Show all request
    this.validateAllFormFields(this.form);

    this.form.valueChanges.pipe(debounceTime(250), distinctUntilChanged())
      .subscribe(data => {
        if (!this.form) { return; }
        if (this.form.valid) {
          this.submit.emit(this.form.getRawValue());
        } else {
          this.validateAllFormFields(this.form);
        }
      });
  }
  // on Submit
  onSubmit(event: Event) {
    event.preventDefault();
    event.stopPropagation();
    if (this.form.valid) {
      this.submit.emit(this.form.getRawValue());
    } else {
      this.validateAllFormFields(this.form);
    }
  }
  // createControl
  createControl() {
    const group = this.fb.group({});
    this.fields.forEach(field => {
      if (field.type === "button") return;
      const control = this.fb.control(
        { value: field.value, disabled: field.disabled},
        this.bindValidations(field.validations || [])
      );
      group.addControl(field.name, control);
    });
    return group;
  }
  // bindValidations
  bindValidations(validations: any) {
    if (validations.length > 0) {
      const validList = [];
      validations.forEach(valid => {
        validList.push(valid.validator);
      });
      return Validators.compose(validList);
    }
    return null;
  }
  // validateAllFormFields
  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control.markAsTouched({ onlySelf: true });
    });
  }
}
