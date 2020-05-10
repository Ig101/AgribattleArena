import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { AsciiLobbyComponent } from './ascii-lobby/ascii-lobby.component';


const routes: Routes = [
  {path: '', component: AsciiLobbyComponent },
  {path: '**', redirectTo: ''}
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class AsciiRoutingModule { }
