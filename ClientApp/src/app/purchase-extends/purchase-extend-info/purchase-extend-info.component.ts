import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { PurchaseExtend } from '../shared/purchase-extend.model';
import { PurchaseExtendService } from '../shared/purchase-extend.service';
import { BaseInfoComponent } from 'src/app/shared2/baseclases/base-info-component';
import { PurchaseExtendCommunicateService } from '../shared/purchase-extend-communicate.service';
import * as moment from "moment";
import { typeField, inputType, ValidatorField, ReturnValue } from 'src/app/shared2/dynamic-form/field-config.model';
import { Validators } from '@angular/forms';
import { ShareService } from 'src/app/shared2/share.service';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';
import { PurchaseRequestPure } from '../shared/purchase-request-pure.model';
import { PurchaseLineExtendService } from '../shared/purchase-line-extend.service';
import { PurchaseLineExtend } from '../shared/purchase-line-extend.model';
import { PurchaseRequestLinePure } from '../shared/purchase-request-line-pure.model';

@Component({
  selector: 'app-purchase-extend-info',
  templateUrl: './purchase-extend-info.component.html',
  styleUrls: ['./purchase-extend-info.component.scss']
})
export class PurchaseExtendInfoComponent
  extends BaseInfoComponent<PurchaseExtend, PurchaseExtendService, PurchaseExtendCommunicateService> {
  constructor(
    service: PurchaseExtendService,
    serviceCom: PurchaseExtendCommunicateService,
    private servicePurExtendLine:PurchaseLineExtendService,
    private serviceShared: ShareService,
    private serviceDialogs: DialogsService,
    private viewCon: ViewContainerRef,
  ) { super(service, serviceCom) }

  // Parameters

  // Methods
  onGetDataByKey(InfoValue: PurchaseExtend): void {
    if (InfoValue && InfoValue.PurchaseExtendId) {
      // if set copy
      this.isCopying = InfoValue.Copying;

      this.service.getOneKeyNumber(InfoValue)
        .subscribe(dbData => {
          this.InfoValue = dbData;
          this.isValid = true;

          if (this.InfoValue.PurchaseExtendId) {
            this.InfoValue.PurchaseLineExtends = new Array;
            this.servicePurExtendLine.getByMasterId(this.InfoValue.PurchaseExtendId)
              .subscribe(data => {
                // this.InfoValue.ActualDetails = data.slice();
                // $id
                if (data) {
                  data.forEach(item => {
                    let temp: PurchaseLineExtend = {
                      PurchaseLineExtendId: 0
                    };
                    // loop deep clone without $id don't need it
                    for (let key in item) {
                      if (key.indexOf("$id") === -1) {
                        temp[key] = item[key];
                      }
                    }

                    // Set copying id is 0 , create and modify is undefined.
                    if (this.isCopying) {
                      temp.PurchaseLineExtendId = 0;
                      temp.Creator = undefined;
                      temp.CreateDate = undefined;
                      temp.Modifyer = undefined;
                      temp.ModifyDate = undefined;
                    }

                    this.InfoValue.PurchaseLineExtends.push(temp);
                  });
                  this.InfoValue.PurchaseLineExtends = this.InfoValue.PurchaseLineExtends.slice();
                }
              });

          }

        }, error => console.error(error), () => {
          if (this.isCopying) {
            this.InfoValue.PurchaseExtendId = 0;
            // Clear
            this.InfoValue.Creator = undefined;
            this.InfoValue.CreateDate = undefined;
            this.InfoValue.Modifyer = undefined;
            this.InfoValue.ModifyDate = undefined;
            // Set Valid
            this.isValid = false;
          }
          this.buildForm();
        });
    }
    else {
      this.InfoValue = {
        PurchaseExtendId: 0,
        PrReceivedDate: moment().toDate(),
        PrReceivedTime: moment().local().format("HH:mm"),
        PurchaseLineExtends : new Array
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
        label: "PRNumber",
        inputType: inputType.text,
        name: "PRNumber",
        disabled: this.denySave,
        value: this.InfoValue.PRNumber,
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
        type: typeField.textarea,
        label: "Remark",
        name: "Remark",
        disabled: this.denySave,
        value: this.InfoValue.Remark,
        validations: [
          {
            name: ValidatorField.maxLength,
            validator: Validators.maxLength(350),
            message: "This character limit is 350"
          },
        ]
      },
      {
        type: typeField.date,
        label: "Received Date",
        name: "PrReceivedDate",
        disabled: this.denySave,
        value: this.InfoValue.PrReceivedDate,
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
        label: "Received Time",
        inputType: inputType.time,
        name: "PrReceivedTime",
        disabled: this.denySave,
        value: this.InfoValue.PrReceivedTime,
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
  submitDynamicForm(InfoValue?: ReturnValue<PurchaseExtend>): void {
    if (InfoValue) {
      if (!this.denySave) {
        let template: PurchaseExtend = InfoValue.value;
        // Template
        for (let key in template) {
          // console.log(key);
          this.InfoValue[key] = template[key];
        }
        this.isValid = InfoValue.valid;
        //debug here
        // console.log(JSON.stringify(InfoValue));
        this.SetCommunicatetoParent();
      }
    }
  }

  // event from component
  FromComponents(): void {
    this.subscription2 = this.serviceShared.ToParent$.subscribe(data => {
      if (data.name.indexOf("PRNumber") !== -1) {
        this.serviceDialogs.dialogSelectPurchaseRequest(this.viewCon, { info: undefined, multi: false, option: true })
          .subscribe((purePR: PurchaseRequestPure) => {
            if (purePR) {
              this.serviceShared.toChild(
                {
                  name: data.name,
                  value: purePR.PrNumber
                });

              this.InfoValue.PRNumber = purePR.PrNumber;
              this.InfoValue.PrSageHeaderId = purePR.PrSageHeaderId;

              // get purchase request line item from database sagex3
              this.servicePurExtendLine.getPurchaseReqLineExtendByPurchaseRequest(this.InfoValue.PRNumber)
                .subscribe((pureLine: Array<PurchaseRequestLinePure>) => {
                  // console.log(pureLine);

                  if (pureLine) {
                    pureLine.forEach(pure => {
                      this.InfoValue.PurchaseLineExtends.push({
                        PurchaseLineExtendId: 0,
                        PurchaseExtendId: this.InfoValue.PurchaseExtendId,
                        ItemCode: pure.ItemCode,
                        ItemName: pure.ItemName.substring(0,498),
                        PrLine: pure.PrLine,
                        PrNumber: pure.PrNumber,
                        Quantity: pure.Quantity,
                        PrSageLineId: pure.PrSageLineId
                      });
                    });
                  }

                  this.InfoValue.PurchaseLineExtends = this.InfoValue.PurchaseLineExtends.slice();
                });
            }
          });
      } 
    });
  }
  
  // Purchase line extends
  OnDetailAction(Item: { data?: PurchaseLineExtend, option: number }): void {
    if (this.denySave || !this.InfoValue.PRNumber) {
      return;
    }
    // add new item in array
    if (Item.option === 1 && !Item.data) {
      this.servicePurExtendLine.getPurchaseReqLineExtendByPurchaseRequest(this.InfoValue.PRNumber)
        .subscribe((pureLine: Array<PurchaseRequestLinePure>) => {
          if (pureLine) {
            pureLine.forEach(pure => {
              if (!this.InfoValue.PurchaseLineExtends.find(item => item.PrLine === pure.PrLine)) {
                this.InfoValue.PurchaseLineExtends.push({
                  PurchaseLineExtendId: 0,
                  PurchaseExtendId: this.InfoValue.PurchaseExtendId,
                  ItemCode: pure.ItemCode,
                  ItemName: pure.ItemName,
                  PrLine: pure.PrLine,
                  PrNumber: pure.PrNumber,
                  Quantity: pure.Quantity,
                  PrSageLineId: pure.PrSageLineId
                });
              }
              else // else already has item in array try to update
              {
                this.OnPurchaseLineHaveUpdateValue({
                  PurchaseLineExtendId: 0,
                  ItemCode: pure.ItemCode,
                  ItemName: pure.ItemName,
                  PrLine: pure.PrLine,
                  PrNumber: pure.PrNumber,
                  Quantity: pure.Quantity,
                  PrSageLineId: pure.PrSageLineId
                });
              }
              this.InfoValue.PurchaseLineExtends = this.InfoValue.PurchaseLineExtends.slice();
          });
          this.SetCommunicatetoParent();
        }
      });
    }
    //update item in array
    else if (Item.option === 2) {
      this.OnPurchaseLineHaveUpdateValue(Item.data);
    }
    // Remove item in array
    else if (Item.option === 0) {
      let indexItem = this.InfoValue.PurchaseLineExtends.indexOf(Item.data);
      this.InfoValue.PurchaseLineExtends.splice(indexItem, 1);
      this.InfoValue.PurchaseLineExtends = this.InfoValue.PurchaseLineExtends.slice();
      this.SetCommunicatetoParent();
    }
  }
  
  // Detail Update
  OnPurchaseLineHaveUpdateValue(uPurchase?: PurchaseLineExtend): void {
    const temp = this.InfoValue.PurchaseLineExtends.find(item => item.PrLine === uPurchase.PrLine);
    if (temp) {
      temp.ItemCode = uPurchase.ItemCode;
      temp.ItemName = uPurchase.ItemName;
      temp.PrLine = uPurchase.PrLine;
      temp.PrNumber = uPurchase.PrNumber;
      temp.Quantity = uPurchase.Quantity;
      temp.PrSageLineId = uPurchase.PrSageLineId;

    }
    this.InfoValue.PurchaseLineExtends = this.InfoValue.PurchaseLineExtends.slice();
    this.SetCommunicatetoParent();
  }
}
