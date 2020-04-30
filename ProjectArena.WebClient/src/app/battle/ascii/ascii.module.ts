import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from 'src/app/shared/shared.module';
import { AsciiRoutingModule } from './ascii-routing.module';
import { AsciiBattleComponent } from './ascii-battle/ascii-battle.component';
import { AsciiBattleStorageService } from './services/ascii-battle-storage.service';
import { AsciiBattleSynchronizerService } from './services/ascii-battle-synchronizer.service';



@NgModule({
  declarations: [AsciiBattleComponent],
  providers: [
    AsciiBattleStorageService,
    AsciiBattleSynchronizerService
  ],
  imports: [
    SharedModule,
    AsciiRoutingModule
  ]
})
export class AsciiModule { }
