import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from 'src/app/shared/shared.module';
import { AsciiRoutingModule } from './ascii-routing.module';
import { AsciiBattleComponent } from './ascii-battle/ascii-battle.component';
import { AsciiBattleStorageService } from './services/ascii-battle-storage.service';
import { AsciiBattleSynchronizerService } from './services/ascii-battle-synchronizer.service';
import { AsciiBattlePathCreatorService } from './services/ascii-battle-path-creator.service';
import { InitiativeBlockComponent } from './ascii-battle/initiative-block/initiative-block.component';



@NgModule({
  declarations: [AsciiBattleComponent, InitiativeBlockComponent],
  providers: [
    AsciiBattleStorageService,
    AsciiBattleSynchronizerService,
    AsciiBattlePathCreatorService
  ],
  imports: [
    SharedModule,
    AsciiRoutingModule
  ]
})
export class AsciiModule { }
