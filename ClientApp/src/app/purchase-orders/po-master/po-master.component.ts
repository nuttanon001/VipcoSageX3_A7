import { Component, OnInit, ViewContainerRef, ViewChild } from '@angular/core';
import { BaseMasterComponent } from '../../shared/base-master-component';
import { PurchaseOrder } from '../shared/purchase-order.model';
import { PurchaseOrderService } from '../shared/purchase-order.service';
import { PurchaseOrderCommunicateService } from '../shared/purchase-order-communicate.service';
import { AuthService } from '../../core/auth/auth.service';
import { DialogsService } from '../../dialogs/shared/dialogs.service';
import { PoTableComponent } from '../po-table/po-table.component';

@Component({
  selector: 'app-po-master',
  templateUrl: './po-master.component.html',
  styleUrls: ['./po-master.component.scss']
})
export class PoMasterComponent extends BaseMasterComponent<PurchaseOrder,PurchaseOrderService,PurchaseOrderCommunicateService> {
  constructor(
    service: PurchaseOrderService,
    serviceCom: PurchaseOrderCommunicateService,
    serviceAuth: AuthService,
    serviceDialogs: DialogsService,
    viewCon: ViewContainerRef
  ) {
    super(service, serviceCom, serviceAuth, serviceDialogs, viewCon);
  }
  // Parameter
  @ViewChild(PoTableComponent)
  private tableComponent: PoTableComponent;
  // on reload data
  onReloadData(): void {
    this.tableComponent.reloadData();
  }

  onCheckStatus(infoValue?: PurchaseOrder): boolean {
    return true;
  }
}
