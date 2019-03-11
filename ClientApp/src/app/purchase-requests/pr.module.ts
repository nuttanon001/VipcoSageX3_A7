import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PrRoutingModule } from './pr-routing.module';
import { PrService } from './shared/pr.service';
import { PrCenterComponent } from './pr-center.component';
import { PrMasterComponent } from './pr-master/pr-master.component';
import { PrTableComponent } from './pr-table/pr-table.component';
import { CustomMaterialModule } from '../shared/customer-material.module';
import { ReactiveFormsModule } from '@angular/forms';
import { PoSubReportComponent } from './po-sub-report/po-sub-report.component';
import { PrOutstandingComponent } from './pr-outstanding/pr-outstanding.component';
import { PoOutstandingComponent } from './po-outstanding/po-outstanding.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CustomMaterialModule,
    PrRoutingModule
  ],
  declarations: [
    PrCenterComponent,
    PrMasterComponent,
    PrTableComponent,
    PoSubReportComponent,
    PrOutstandingComponent,
    PoOutstandingComponent
  ],
  providers: [
    PrService
  ]
})
export class PrModule { }
