import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MiscRoutingModule } from './misc-routing.module';
import { MiscService } from './shared/misc.service';
import { MiscCenterComponent } from './misc-center.component';
import { MiscMasterComponent } from './misc-master/misc-master.component';
import { IssueTableComponent } from './issue-table/issue-table.component';
import { JournalTableComponent } from './journal-table/journal-table.component';
import { ReactiveFormsModule } from '@angular/forms';
import { CustomMaterialModule } from '../shared/customer-material.module';
import { InvoiceSupBpComponent } from './invoice-sup-bp/invoice-sup-bp.component';
import { Journal2Component } from './journal2/journal2.component';
import { InvoiceOutDueComponent } from './invoice-out-due/invoice-out-due.component';

@NgModule({
  imports: [
    CommonModule,
    MiscRoutingModule,
    ReactiveFormsModule,
    CustomMaterialModule,
  ],
  declarations: [
    MiscCenterComponent,
    MiscMasterComponent,
    IssueTableComponent,
    JournalTableComponent,
    InvoiceSupBpComponent,
    Journal2Component,
    InvoiceOutDueComponent
  ],
  providers: [
    MiscService
  ]
})
export class MiscModule { }
