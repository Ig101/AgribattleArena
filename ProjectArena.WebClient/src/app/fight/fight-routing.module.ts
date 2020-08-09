import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FightComponent } from './fight.component';

const routes: Routes = [
  {path: '', component: FightComponent },
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
