import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PoCenterComponent } from './po-center.component';
import { PoMasterComponent } from './po-master/po-master.component';

const routes: Routes = [{
  path: "",
  component: PoCenterComponent,
  children: [
    {
      path: ":key",
      component: PoMasterComponent,
    },
    {
      path: "",
      component: PoMasterComponent,
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PuchaseOrderRoutingModule { }
