import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { EngineModule } from '../engine/engine.module';
import { FightComponent } from './fight.component';
import { FightRoutingModule } from './fight-routing.module';
import { SynchronizationService } from '../engine/services/synchronization.service';
import { ContextMenuComponent } from './context-menu/context-menu.component';
import { ContextMenuItemComponent } from './context-menu/context-menu-item/context-menu-item.component';
import { MenuModalComponent } from './modals/menu-modal/menu-modal.component';
import { RewardForModalComponent } from './modals/reward-for-modal/reward-for-modal.component';
import { InitiativeBlockComponent } from './initiative-block/initiative-block.component';
import { TargetChooseModalComponent } from './modals/target-choose-modal/target-choose-modal.component';



@NgModule({
  declarations: [
    FightComponent,
    ContextMenuComponent,
    ContextMenuItemComponent,
    MenuModalComponent,
    RewardForModalComponent,
    InitiativeBlockComponent,
    TargetChooseModalComponent
  ],
  imports: [
    SharedModule,
    EngineModule,
    FightRoutingModule
  ]
})
export class FightModule {

}
