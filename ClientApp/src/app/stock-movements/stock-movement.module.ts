import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StockMovementRoutingModule } from './stock-movement-routing.module';
import { StockMovementCenterComponent } from './stock-movement-center.component';
import { StockMovementMasterComponent } from './stock-movement-master/stock-movement-master.component';
import { StockMovementsService } from './shared/stock-movements.service';
import { ReactiveFormsModule } from '@angular/forms';
import { CustomMaterialModule } from '../shared/customer-material.module';
import { StockMovement2TableComponent } from './stock-movement2-table/stock-movement2-table.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CustomMaterialModule,
    StockMovementRoutingModule
  ],
  declarations: [
    StockMovementCenterComponent,
    StockMovementMasterComponent,
    StockMovement2TableComponent
  ],
  providers: [
    StockMovementsService
  ]
})
export class StockMovementModule { }
