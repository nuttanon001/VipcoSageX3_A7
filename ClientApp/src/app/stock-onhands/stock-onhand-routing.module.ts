import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StockOnhandCenterComponent } from './stock-onhand-center.component';
import { StockOnhandMasterComponent } from './stock-onhand-master/stock-onhand-master.component';
import { Onhandmk2Component } from './onhandmk2/onhandmk2.component';
import { StockBalanceComponent } from './stock-balance/stock-balance.component';
import { IssusWorkgroupComponent } from './issus-workgroup/issus-workgroup.component';

const routes: Routes = [{
  path: "",
  component: StockOnhandCenterComponent,
  children: [
    {
      path: "onhandmk2",
      component: Onhandmk2Component,
    },
    {
      path: "stock-balance",
      component: StockBalanceComponent
    },
    {
      path: "issus-workgroup",
      component: IssusWorkgroupComponent
    },
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
