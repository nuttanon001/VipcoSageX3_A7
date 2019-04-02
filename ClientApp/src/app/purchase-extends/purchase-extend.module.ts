import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
// Components
import { PurchaseExtendCenterComponent } from './purchase-extend-center.component';
import { PurchaseExtendMasterComponent } from './purchase-extend-master/purchase-extend-master.component';
import { PurchaseExtendTableComponent } from './purchase-extend-table/purchase-extend-table.component';
import { PurchaseExtendInfoComponent } from './purchase-extend-info/purchase-extend-info.component';
// Modules
import { SharedModule } from '../shared2/shared.module';
import { CustomMaterialModule } from '../shared2/customer-material.module';
import { PurchaseExtendRoutingModule } from './purchase-extend-routing.module';
import { PurchaseOverHeaderMasterComponent } from './purchase-over-header-master/purchase-over-header-master.component';
import { PurchaseOverHeaderTableComponent } from './purchase-over-header-table/purchase-over-header-table.component';
import { PurchaseOverHeaderInfoComponent } from './purchase-over-header-info/purchase-over-header-info.component';
import { PurchaseExtendSubTableComponent } from './purchase-extend-sub-table/purchase-extend-sub-table.component';
import { PurchaseLineExtendSubTableComponent } from './purchase-line-extend-sub-table/purchase-line-extend-sub-table.component';

@NgModule({
  declarations: [
    PurchaseExtendCenterComponent,
    PurchaseExtendMasterComponent,
    PurchaseExtendTableComponent,
    PurchaseExtendInfoComponent,
    PurchaseOverHeaderMasterComponent,
    PurchaseOverHeaderTableComponent,
    PurchaseOverHeaderInfoComponent,
    PurchaseExtendSubTableComponent,
    PurchaseLineExtendSubTableComponent],
  imports: [
    FormsModule,
    SharedModule,
    CommonModule,
    ReactiveFormsModule,
    CustomMaterialModule,
    PurchaseExtendRoutingModule
  ]
})
export class PurchaseExtendModule { }
