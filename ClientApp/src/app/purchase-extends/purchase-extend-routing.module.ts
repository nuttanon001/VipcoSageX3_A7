import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PurchaseExtendCenterComponent } from './purchase-extend-center.component';
import { PurchaseExtendMasterComponent } from './purchase-extend-master/purchase-extend-master.component';

const routes: Routes = [{
  path: "",
  component: PurchaseExtendCenterComponent,
  children: [
    {
      path: ":key",
      component: PurchaseExtendMasterComponent,
    },
    {
      path: "",
      component: PurchaseExtendMasterComponent,
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PurchaseExtendRoutingModule { }
