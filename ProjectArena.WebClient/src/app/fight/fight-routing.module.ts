import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FightComponent } from './fight.component';
import { FightResolverService } from './resolvers/fight-resolver.service';

const routes: Routes = [
  {path: '', component: FightComponent, resolve: { loadingStatus: FightResolverService } },
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
export class FightRoutingModule { }
