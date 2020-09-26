import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { UserResolverService } from './shared/resolvers/user-resolver.service';

const routes: Routes = [
  {path: 'fight', loadChildren: () => import('./fight/fight.module').then(x => x.FightModule), resolve: { user: UserResolverService } },
  {path: 'auth', loadChildren: () => import('./auth/auth.module').then(x => x.AuthModule), resolve: { user: UserResolverService } },
  {path: '**', redirectTo: 'auth'}
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
