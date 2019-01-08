import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MiscCenterComponent } from './misc-center.component';
import { MiscMasterComponent } from './misc-master/misc-master.component';

const routes: Routes = [{
  path: "",
  component: MiscCenterComponent,
  children: [
    {
      path: ":key",
      component: MiscMasterComponent,
    },
    {
      path: "",
      component: MiscMasterComponent,
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MiscRoutingModule { }
