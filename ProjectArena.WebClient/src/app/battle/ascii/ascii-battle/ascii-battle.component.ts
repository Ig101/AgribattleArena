import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BattleResolverService } from '../../resolvers/battle-resolver.service';
import { AsciiBattleStorageService } from '../services/ascii-battle-storage.service';

@Component({
  selector: 'app-ascii-battle',
  templateUrl: './ascii-battle.component.html',
  styleUrls: ['./ascii-battle.component.scss']
})
export class AsciiBattleComponent implements OnInit, OnDestroy {

  constructor(
    private activatedRoute: ActivatedRoute,
    private battleResolver: BattleResolverService,
    private battleStorageService: AsciiBattleStorageService
  ) { }

  ngOnDestroy(): void {
    this.battleStorageService.clear();
  }

  ngOnInit(): void {
    const newBattle = this.activatedRoute.snapshot.data.battle;

  }

}
