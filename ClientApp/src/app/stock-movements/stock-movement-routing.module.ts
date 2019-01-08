import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StockMovementCenterComponent } from './stock-movement-center.component';
import { StockMovementMasterComponent } from './stock-movement-master/stock-movement-master.component';

const routes: Routes = [{
  path: "",
  component: StockMovementCenterComponent,
  children: [
    {
      path: ":key",
      component: StockMovementMasterComponent,
    },
    {
      path: "",
      component: StockMovementMasterComponent,
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StockMovementRoutingModule { }
