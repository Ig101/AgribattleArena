import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { EngineModule } from '../engine/engine.module';
import { FightComponent } from './fight.component';
import { FightRoutingModule } from './fight-routing.module';
import { SynchronizationService } from '../engine/services/synchronization.service';



@NgModule({
  declarations: [FightComponent],
  imports: [
    SharedModule,
    EngineModule,
    FightRoutingModule
  ]
})
export class FightModule {

}
