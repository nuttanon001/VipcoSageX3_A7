import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { TaskStatusMaster } from '../shared/task-status-master.model';
import { TaskStatusMasterService } from '../shared/task-status-master.service';
import { TaskStatusMasterCommunicateService } from '../shared/task-status-master-communicate.service';
import { TaskStatusDetailService } from '../shared/task-status-detail.service';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';
import { BaseInfoComponent } from 'src/app/shared2/baseclases/base-info-component';
import { TaskStatusDetail } from '../shared/task-status-detail.model';
import { typeField, inputType, ValidatorField } from 'src/app/shared2/dynamic-form/field-config.model';
import { Validators } from '@angular/forms';
import { ShareService } from 'src/app/shared2/share.service';
import { Workgroup } from 'src/app/dimension-datas/shared/workgroup.model';
import { Employee } from 'src/app/employees/shared/employee.model';

@Component({
  selector: 'app-task-status-info',
  templateUrl: './task-status-info.component.html',
  styleUrls: ['./task-status-info.component.scss'],
  providers: [ShareService]
})
export class TaskStatusInfoComponent extends BaseInfoComponent<TaskStatusMaster, TaskStatusMasterService, TaskStatusMasterCommunicateService> {

  constructor(
    service: TaskStatusMasterService,
    serviceCom: TaskStatusMasterCommunicateService,
    private serviceShared : ShareService,
    private serviceDetail: TaskStatusDetailService,
    private serviceDialogs: DialogsService,
    private viewCon: ViewContainerRef,
  ) { super(service, serviceCom) }

  // Parameters

  // Methods
  onGetDataByKey(InfoValue: TaskStatusMaster): void {
    if (InfoValue && InfoValue.TaskStatusMasterId) {
      // if set copy
      this.isCopying = InfoValue.Copying;

      this.service.getOneKeyNumber(InfoValue)
        .subscribe(dbData => {
          this.InfoValue = dbData;
          this.isValid = true;

          if (this.InfoValue.TaskStatusMasterId) {
            // new Array
            this.InfoValue.TaskStatusDetails = new Array;
            // get array of data from database
            this.serviceDetail.getByMasterId(this.InfoValue.TaskStatusMasterId)
              .subscribe(data => {
                // this.InfoValue.ActualDetails = data.slice();
                // $id
                if (data) {
                  data.forEach(item => {
                    let temp: TaskStatusDetail = {
                      TaskStatusDetailId: 0
                    };
                    // loop deep clone without $id don't need it
                    for (let key in item) {
                      if (key.indexOf("$id") === -1) {
                        temp[key] = item[key];
                      }
                    }

                    // Set copying id is 0 , create and modify is undefined.
                    if (this.isCopying) {
                      temp.TaskStatusDetailId = 0;
                      temp.Creator = undefined;
                      temp.CreateDate = undefined;
                      temp.Modifyer = undefined;
                      temp.ModifyDate = undefined;
                    }

                    this.InfoValue.TaskStatusDetails.push(temp);
                  });
                  this.InfoValue.TaskStatusDetails = this.InfoValue.TaskStatusDetails.slice();
                }
              });
          }
        }, error => console.error(error), () => {
          if (this.isCopying) {
            this.InfoValue.TaskStatusMasterId = 0;
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
        TaskStatusMasterId: 0,
        TaskStatusDetails: new Array,
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
        label: "WorkGroupCode",
        inputType: inputType.text,
        name: "WorkGroupCode",
        disabled: this.denySave,
        value: this.InfoValue.WorkGroupCode,
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
        label: "WorkGroupName",
        inputType: inputType.text,
        name: "WorkGroupName",
        disabled: this.denySave,
        value: this.InfoValue.WorkGroupName,
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
        label: "Remark",
        inputType: inputType.text,
        name: "Remark",
        disabled: this.denySave,
        value: this.InfoValue.Remark,
        validations: [
          {
            name: ValidatorField.maxLength,
            validator: Validators.maxLength(250),
            message: "maximum length of a field in 250"
          },
        ]
      },
    ];
    // let ExcludeList = this.regConfig.map((item) => item.name);
  }

  // set communicate
  SetCommunicatetoParent(): void {
    if (this.isValid) {
      if (this.InfoValue.TaskStatusDetails.length > 0) {
        // debug here
        console.log("communicateService", JSON.stringify(this.InfoValue));
        this.communicateService.toParent(this.InfoValue);
      }
    }
  }

  // submit dynamic form
  submitDynamicForm(InfoValue?: TaskStatusMaster): void {
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
      if (data.name.indexOf("WorkGroupCode") !== -1 || data.name.indexOf("WorkGroupName") !== -1) {
        this.serviceDialogs.dialogSelectGroup(this.viewCon)
          .subscribe((group:Workgroup) => {
            let temp = ["WorkGroupCode", "WorkGroupName"]
            temp.forEach(item => {
              this.serviceShared.toChild(
                {
                  name: item,
                  value: group[item]
                });
            });

            this.InfoValue.WorkGroupCode = group.WorkGroupCode;
            this.InfoValue.WorkGroupName = group.WorkGroupName;
          });
      }
    });
  }

  // Precaution
  OnDetailAction(Item: { data?: TaskStatusDetail, option: number }): void {
    if (this.denySave) {
      return;
    }
   
    if (this.denySave) {
      return;
    }
    if (Item.option === 1 && !Item.data) {

      this.serviceDialogs.dialogSelectEmployee(this.viewCon, { info: undefined, multi: true, option: false })
        .subscribe((employees: Array<Employee>) => {
          if (employees) {
            employees.forEach(emp => {
              if (!this.InfoValue.TaskStatusDetails.find(item => item.EmployeeCode === emp.EmpCode)) {
                this.InfoValue.TaskStatusDetails.push({
                  TaskStatusDetailId: 0,
                  TaskStatusMasterId: this.InfoValue.TaskStatusMasterId,
                  Email: "",
                  EmployeeCode: emp.EmpCode,
                  Name: emp.NameThai,
                  Remark: "",
                });
                this.InfoValue.TaskStatusDetails = this.InfoValue.TaskStatusDetails.slice();
              }
            });
            this.SetCommunicatetoParent();
          }
        });
    }
    else if (Item.option === 2) {
      const detail = this.InfoValue.TaskStatusDetails.find(value => value.EmployeeCode == Item.data.EmployeeCode);
      if (detail) {
        detail.Email = Item.data.Email;
        detail.Remark = Item.data.Remark;
      }
      this.InfoValue.TaskStatusDetails = this.InfoValue.TaskStatusDetails.slice();
      this.SetCommunicatetoParent();
    }
    else if (Item.option === 0) {
      let indexItem = this.InfoValue.TaskStatusDetails.indexOf(Item.data);
      this.InfoValue.TaskStatusDetails.splice(indexItem, 1);
      this.InfoValue.TaskStatusDetails = this.InfoValue.TaskStatusDetails.slice();
      this.SetCommunicatetoParent();
    }
  }
}
