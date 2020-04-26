import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../shared/shared.module';
import { BattleRoutingModule } from './battle-routing.module';
import { BattleResolverService } from './resolvers/battle-resolver.service';



@NgModule({
  imports: [
    SharedModule,
    BattleRoutingModule
  ],
  providers: [
    BattleResolverService
  ]
})
export class BattleModule { }
