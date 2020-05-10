import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AsciiLobbyComponent } from './ascii-lobby/ascii-lobby.component';
import { AsciiRoutingModule } from './ascii-routing.module';
import { AsciiLobbyStorageService } from './services/ascii-lobby-storage.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { TavernComponent } from './components/tavern/tavern.component';
import { CharactersListComponent } from './components/characters-list/characters-list.component';
import { TavernModalComponent } from './modals/tavern-modal/tavern-modal.component';
import { SettingsModalComponent } from './modals/settings-modal/settings-modal.component';
import { ModalLoadingComponent } from './modals/modal-loading/modal-loading.component';



@NgModule({
  declarations: [
    AsciiLobbyComponent,
    TavernComponent,
    CharactersListComponent,
    TavernModalComponent,
    SettingsModalComponent,
    ModalLoadingComponent],
  imports: [
    SharedModule,
    AsciiRoutingModule
  ],
  providers: [
    AsciiLobbyStorageService
  ]
})
export class AsciiModule { }
