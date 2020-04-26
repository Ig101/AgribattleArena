import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from 'src/app/shared/shared.module';
import { AsciiRoutingModule } from './ascii-routing.module';



@NgModule({
  declarations: [],
  imports: [
    SharedModule,
    AsciiRoutingModule
  ]
})
export class AsciiModule { }
