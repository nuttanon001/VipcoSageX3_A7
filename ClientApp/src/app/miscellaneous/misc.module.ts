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
    JournalTableComponent
  ],
  providers: [
    MiscService
  ]
})
export class MiscModule { }
