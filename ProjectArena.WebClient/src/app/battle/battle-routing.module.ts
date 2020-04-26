import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { BattleResolverService } from './resolvers/battle-resolver.service';

const routes: Routes = [
  {path: '', loadChildren: () => import('./ascii/ascii.module').then(x => x.AsciiModule),
    resolve: { battle: BattleResolverService } },
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
export class BattleRoutingModule { }
