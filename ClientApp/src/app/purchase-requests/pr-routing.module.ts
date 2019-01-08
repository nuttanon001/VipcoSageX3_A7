import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PrCenterComponent } from './pr-center.component';
import { PrMasterComponent } from './pr-master/pr-master.component';

const routes: Routes = [{
  path: "",
  component: PrCenterComponent,
  children: [
    {
      path: ":key",
      component: PrMasterComponent,
    },
    {
      path: "",
      component: PrMasterComponent,
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PrRoutingModule { }
