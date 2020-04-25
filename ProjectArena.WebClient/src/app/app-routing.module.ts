import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { UserResolverService } from './shared/resolvers/user-resolver.service';

const routes: Routes = [
  // {path: 'game', loadChildren: () => import('./game/game.module').then(x => x.GameModule) },
  {path: 'lobby', loadChildren: () => import('./lobby/lobby.module').then(x => x.LobbyModule) },
  {path: '**', redirectTo: 'lobby'}
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule { }
