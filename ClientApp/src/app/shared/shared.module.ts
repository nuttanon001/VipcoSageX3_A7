import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
// Module
import { CustomMaterialModule } from './customer-material.module';
// Component

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    CustomMaterialModule,
  ],
  declarations: [],
  exports: [],
  entryComponents: []
})
export class SharedModule { }
