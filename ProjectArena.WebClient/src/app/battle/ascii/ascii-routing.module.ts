import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { AsciiBattleComponent } from './ascii-battle/ascii-battle.component';


const routes: Routes = [
  {path: '', component: AsciiBattleComponent },
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
