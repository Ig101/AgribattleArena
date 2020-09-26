import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SceneService } from './services/scene.service';
import { SharedModule } from '../shared/shared.module';
import { SynchronizationService } from './services/synchronization.service';



@NgModule({
  declarations: [],
  imports: [
    SharedModule
  ],
  providers: [
    SceneService,
    SynchronizationService
  ]
})
export class EngineModule { }
