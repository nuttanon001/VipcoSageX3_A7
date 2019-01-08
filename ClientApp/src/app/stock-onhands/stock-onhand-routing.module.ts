import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StockOnhandCenterComponent } from './stock-onhand-center.component';
import { StockOnhandMasterComponent } from './stock-onhand-master/stock-onhand-master.component';

const routes: Routes = [{
  path: "",
  component: StockOnhandCenterComponent,
  children: [
    {
      path: ":key",
      component: StockOnhandMasterComponent,
    },
    {
      path: "",
      component: StockOnhandMasterComponent,
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StockOnhandRoutingModule { }
