// Angular Code
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
// Modules
import { StockOnhandRoutingModule } from './stock-onhand-routing.module';
import { CustomMaterialModule } from '../shared/customer-material.module';
// Services
import { StockOnhandService } from './shared/stock-onhand.service';
// Components
import { StockOnhandCenterComponent } from './stock-onhand-center.component';
import { StockOnhandMasterComponent } from './stock-onhand-master/stock-onhand-master.component';
import { StockLocaltionTableComponent } from './stock-localtion-table/stock-localtion-table.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CustomMaterialModule,
    StockOnhandRoutingModule
  ],
  declarations: [
    StockOnhandCenterComponent,
    StockOnhandMasterComponent,
    StockLocaltionTableComponent
  ],
  providers: [
    StockOnhandService
  ]
})
export class StockOnhandModule { }
