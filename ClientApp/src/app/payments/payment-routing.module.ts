// Angular Core
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// Components
import { PaymentCenterComponent } from './payment-center.component';
import { RetentionComponent } from './retention/retention.component';
import { PaymentSubComponent } from './payment-sub/payment-sub.component';
import { PaymentMasterComponent } from './payment-master/payment-master.component';

const routes: Routes = [{
  path: "",
  component: PaymentCenterComponent,
  children: [
    {
      path: "retention",
      component: RetentionComponent,
    },
    {
      path: "payment-sub",
      component: PaymentSubComponent,
    },
    {
      path: ":key",
      component: PaymentMasterComponent,
    },
    {
      path: "",
      component: PaymentMasterComponent,
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PaymentRoutingModule { }
