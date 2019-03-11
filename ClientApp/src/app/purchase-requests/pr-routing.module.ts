import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PrCenterComponent } from './pr-center.component';
import { PrMasterComponent } from './pr-master/pr-master.component';
import { PoSubReportComponent } from './po-sub-report/po-sub-report.component';
import { PrOutstandingComponent } from './pr-outstanding/pr-outstanding.component';
import { PoOutstandingComponent } from './po-outstanding/po-outstanding.component';

const routes: Routes = [{
  path: "",
  component: PrCenterComponent,
  children: [
    {
      path: "pr-outstanding",
      component: PrOutstandingComponent,
    },
    {
      path: "po-outstanding",
      component: PoOutstandingComponent,
    },
    {
      path: "posub-report",
      component: PoSubReportComponent,
    },
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
