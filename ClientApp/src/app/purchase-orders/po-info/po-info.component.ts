import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { BaseInfoComponent } from '../../shared/base-info-component';
import { PurchaseOrder } from '../shared/purchase-order.model';
import { PurchaseOrderService } from '../shared/purchase-order.service';
import { PurchaseOrderCommunicateService } from '../shared/purchase-order-communicate.service';
import { DialogsService } from '../../dialogs/shared/dialogs.service';
import { FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-po-info',
  templateUrl: './po-info.component.html',
  styleUrls: ['./po-info.component.scss']
})
export class PoInfoComponent extends BaseInfoComponent<PurchaseOrder,PurchaseOrderService,PurchaseOrderCommunicateService> {
  constructor(
    service: PurchaseOrderService,
    serviceCom: PurchaseOrderCommunicateService,
    serviceDialogs: DialogsService,
    viewCon: ViewContainerRef,
    private fb: FormBuilder,
  ) {
    super(service, serviceCom);
  }

  onGetDataByKey(value: PurchaseOrder): void {
    if (value) {
      this.service.getOneKeyNumber(value)
        .subscribe(dbData => {
          this.InfoValue = dbData;
        }, error => console.error(error));
    } 
  }

  buildForm(): void {
    // Not use
  }
}
