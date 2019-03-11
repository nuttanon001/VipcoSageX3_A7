import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MiscCenterComponent } from './misc-center.component';
import { MiscMasterComponent } from './misc-master/misc-master.component';
import { InvoiceSupBpComponent } from './invoice-sup-bp/invoice-sup-bp.component';
import { Journal2Component } from './journal2/journal2.component';
import { InvoiceOutDueComponent } from './invoice-out-due/invoice-out-due.component';

const routes: Routes = [{
  path: "",
  component: MiscCenterComponent,
  children: [
    {
      path: "out-due",
      component: InvoiceOutDueComponent,
    },
    {
      path: "invoices",
      component: InvoiceSupBpComponent,
    },
    {
      path: "journal2",
      component: Journal2Component,
    },
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
