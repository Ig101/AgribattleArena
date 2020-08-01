import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SceneService } from './services/scene.service';
import { SharedModule } from '../shared/shared.module';



@NgModule({
  declarations: [],
  imports: [
    SharedModule
  ],
  exports: [
    SceneService
  ],
  providers: [
    SceneService
  ]
})
export class EngineModule { }
