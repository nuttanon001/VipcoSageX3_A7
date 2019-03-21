import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { BaseInfoComponent } from 'src/app/shared2/baseclases/base-info-component';
import { AllowedEmployee } from '../shared/allowed-employee.model';
import { AllowedEmployeeService } from '../shared/allowed-employee.service';
import { AllowedEmployeeCommunicateService } from '../shared/allowed-employee-communicate.service';
import { ShareService } from 'src/app/shared2/share.service';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';
import { typeField, inputType, ValidatorField } from 'src/app/shared2/dynamic-form/field-config.model';
import { Validators } from '@angular/forms';
import { Employee } from 'src/app/employees/shared/employee.model';

@Component({
  selector: 'app-allowed-employee-info',
  templateUrl: './allowed-employee-info.component.html',
  styleUrls: ['./allowed-employee-info.component.scss']
})
export class AllowedEmployeeInfoComponent
  extends BaseInfoComponent<AllowedEmployee, AllowedEmployeeService, AllowedEmployeeCommunicateService> {

  constructor(
    service: AllowedEmployeeService,
    serviceCom: AllowedEmployeeCommunicateService,
    private serviceShared: ShareService,
    private serviceDialogs: DialogsService,
    private viewCon: ViewContainerRef,
  ) { super(service, serviceCom) }

  // Parameters

  // Methods
  onGetDataByKey(InfoValue: AllowedEmployee): void {
    if (InfoValue && InfoValue.EmpCode) {
      // if set copy
      this.isCopying = InfoValue.Copying;

      this.service.getOneKeyNumber(InfoValue)
        .subscribe(dbData => {
          this.InfoValue = dbData;
          this.isValid = true;
        }, error => console.error(error), () => {
          if (this.isCopying) {
            this.InfoValue.EmpCode = "";
            this.InfoValue.Creator = undefined;
            this.InfoValue.CreateDate = undefined;
            this.InfoValue.Modifyer = undefined;
            this.InfoValue.ModifyDate = undefined;
          }
          this.buildForm();
        });
    }
    else {
      this.InfoValue = {
        EmpCode: ""
      };
      this.buildForm();
    }
  }

  // Build Form
  buildForm(): void {

    this.regConfig = [
      // BasemodelRequireWorkpermit //
      {
        type: typeField.inputclick,
        label: "EmpCode",
        inputType: inputType.text,
        name: "EmpCode",
        disabled: this.denySave,
        value: this.InfoValue.EmpCode,
        readonly: true,
        validations: [
          {
            name: ValidatorField.required,
            validator: Validators.required,
            message: "This field is required"
          },
        ]
      },
      {
        type: typeField.inputclick,
        label: "NameThai",
        inputType: inputType.text,
        name: "NameThai",
        disabled: this.denySave,
        value: this.InfoValue.NameThai,
        readonly: true,
        validations: [
          {
            name: ValidatorField.required,
            validator: Validators.required,
            message: "This field is required"
          },
        ]
      },
      {
        type: typeField.input,
        label: "SubLevel",
        inputType: inputType.number,
        name: "SubLevel",
        disabled: this.denySave,
        value: this.InfoValue.SubLevel,
        validations: [
          {
            name: ValidatorField.required,
            validator: Validators.required,
            message: "This field is required"
          },
        ]
      },
    ];
    // let ExcludeList = this.regConfig.map((item) => item.name);
  }

  // set communicate
  SetCommunicatetoParent(): void {
    if (this.isValid) {
      this.communicateService.toParent(this.InfoValue);
    }
  }

  // submit dynamic form
  submitDynamicForm(InfoValue?: AllowedEmployee): void {
    if (InfoValue) {
      if (!this.denySave) {
        let template = InfoValue;
        // Template
        for (let key in template) {
          // console.log(key);
          this.InfoValue[key] = template[key];
        }
        this.isValid = true;
        //debug here
        // console.log(JSON.stringify(InfoValue));
        this.SetCommunicatetoParent();
      }
    }
  }

  // event from component
  FromComponents(): void {
    this.subscription2 = this.serviceShared.ToParent$.subscribe(data => {
      if (data.name.indexOf("EmpCode") !== -1 || data.name.indexOf("NameThai") !== -1) {
        this.serviceDialogs.dialogSelectEmployee(this.viewCon, { info: undefined, multi: false, option: true })
          .subscribe((emp: Employee) => {
            if (emp) {
              let temp = ["EmpCode", "NameThai"]
              temp.forEach(item => {
                this.serviceShared.toChild(
                  {
                    name: item,
                    value: emp[item]
                  });
              });

              this.InfoValue.EmpCode = emp.EmpCode;
              this.InfoValue.NameThai = emp.NameThai;
            }
          });
      }
    });
  }

}
