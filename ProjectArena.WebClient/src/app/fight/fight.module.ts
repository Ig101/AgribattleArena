import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { EngineModule } from '../engine/engine.module';
import { FightComponent } from './fight.component';
import { FightRoutingModule } from './fight-routing.module';
import { SynchronizationService } from '../engine/services/synchronization.service';
import { ContextMenuComponent } from './context-menu/context-menu.component';
import { ContextMenuItemComponent } from './context-menu/context-menu-item/context-menu-item.component';



@NgModule({
  declarations: [FightComponent, ContextMenuComponent, ContextMenuItemComponent],
  imports: [
    SharedModule,
    EngineModule,
    FightRoutingModule
  ]
})
export class FightModule {

}
