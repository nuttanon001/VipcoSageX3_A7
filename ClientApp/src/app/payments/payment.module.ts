import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PaymentRoutingModule } from './payment-routing.module';
import { PaymentService } from './shared/payment.service';
import { ReactiveFormsModule } from '@angular/forms';
import { CustomMaterialModule } from '../shared/customer-material.module';
import { PaymentCenterComponent } from '../payments/payment-center.component';
import { PaymentMasterComponent } from './payment-master/payment-master.component';
import { RetentionComponent } from './retention/retention.component';
import { PaymentSubComponent } from './payment-sub/payment-sub.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PaymentRoutingModule,
    CustomMaterialModule,
  ],
  declarations: [
    PaymentCenterComponent,
    PaymentMasterComponent,
    RetentionComponent,
    PaymentSubComponent
  ],
  providers: [
    PaymentService
  ]
})
export class PaymentModule { }
