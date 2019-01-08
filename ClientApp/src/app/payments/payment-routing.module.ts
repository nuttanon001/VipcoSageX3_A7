import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PaymentCenterComponent } from './payment-center.component';
import { PaymentMasterComponent } from './payment-master/payment-master.component';

const routes: Routes = [{
  path: "",
  component: PaymentCenterComponent,
  children: [
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
