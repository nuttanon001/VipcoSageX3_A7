// AngularCore
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
// Modules
import { CustomMaterialModule } from '../shared/customer-material.module';
import { PuchaseOrderRoutingModule } from './purchase-order-routing.module';
// Services
import { PurchaseOrderService } from './shared/purchase-order.service';
import { PurchaseOrderCommunicateService } from './shared/purchase-order-communicate.service';
// Components
import { PoCenterComponent } from './po-center.component';
import { PoInfoComponent } from './po-info/po-info.component';
import { PoTableComponent } from './po-table/po-table.component';
import { PoMasterComponent } from './po-master/po-master.component';
import { DimensionModule } from '../dimension-datas/dimension.module';

@NgModule({
  imports: [
    CommonModule,
    DimensionModule,
    ReactiveFormsModule,
    CustomMaterialModule,
    PuchaseOrderRoutingModule
  ],
  declarations: [
    PoCenterComponent,
    PoMasterComponent,
    PoTableComponent,
    PoInfoComponent
  ],
  providers: [
    PurchaseOrderService,
    PurchaseOrderCommunicateService
  ]
})
export class PurchaseOrderModule { }
