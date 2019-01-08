import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DimensionRoutingModule } from './dimension-routing.module';
import { ProjectCodeService } from './shared/project-code.service';
import { WorkgroupService } from './shared/workgroup.service';
import { BomLevelService } from './shared/bom-level.service';
import { BankService } from './shared/bank.service';
import { SupplierService } from './shared/supplier.service';
import { CategoryService } from './shared/category.service';

@NgModule({
  imports: [
    CommonModule,
    DimensionRoutingModule
  ],
  declarations: [],
  providers: [
    ProjectCodeService,
    WorkgroupService,
    BomLevelService,
    BankService,
    SupplierService,
    CategoryService
  ]
})
export class DimensionModule { }
