import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { LobbyResolverService } from './resolvers/lobby-resolver.service';

const routes: Routes = [
  {path: '', loadChildren: () => import('./ascii/ascii.module').then(x => x.AsciiModule),
    resolve: { lobby: LobbyResolverService } },
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
export class LobbyRoutingModule { }
