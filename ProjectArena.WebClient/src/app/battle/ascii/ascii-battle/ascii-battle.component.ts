import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BattleResolverService } from '../../resolvers/battle-resolver.service';
import { AsciiBattleStorageService } from '../services/ascii-battle-storage.service';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';
import { Subscription } from 'rxjs';
import { Synchronizer } from 'src/app/shared/models/battle/synchronizer.model';
import { BattleSynchronizationActionEnum } from 'src/app/shared/models/enum/battle-synchronization-action.enum';
import { UserService } from 'src/app/shared/services/user.service';
import { BattlePlayerStatusEnum } from 'src/app/shared/models/enum/player-battle-status.enum';
import { Scene } from '../models/scene/scene.model';
import { newArray } from '@angular/compiler/src/util';
import { Tile } from '../models/scene/tile.model';
import { tileNatives, actorNatives, skillNatives, buffNatives } from '../natives';
import { Player } from '../models/player.model';
import { convertTile, convertActor, convertDecoration, convertEffect } from '../helpers/scene-create.helper';
import { Actor } from '../models/scene/actor.model';
import { ActiveDecoration } from '../models/scene/active-decoration.model';
import { SpecEffect } from '../models/scene/spec-effect.model';
import { removeFromArray } from 'src/app/helpers/extensions/array.extension';
import { synchronizeActor, synchronizeTile, synchronizeDecoration, synchronizeEffect } from '../helpers/scene-update.helper';
import { AsciiBattleSynchronizerService } from '../services/ascii-battle-synchronizer.service';

// TODO Error instead of console.logs

@Component({
  selector: 'app-ascii-battle',
  templateUrl: './ascii-battle.component.html',
  styleUrls: ['./ascii-battle.component.scss']
})
export class AsciiBattleComponent implements OnInit, OnDestroy {

  onCloseSubscription: Subscription;
  arenaActionsSubscription: Subscription;
  synchronizationErrorSubscription: Subscription;

  receivingMessagesFromHubAllowed = false;
  specificActionResponseForWait: {
    actorId: number,
    action: BattleSynchronizationActionEnum
  };

  constructor(
    private activatedRoute: ActivatedRoute,
    private battleResolver: BattleResolverService,
    private battleStorageService: AsciiBattleStorageService,
    private battleSynchronizerService: AsciiBattleSynchronizerService,
    private arenaHub: ArenaHubService,
    private userService: UserService
  ) {
    this.onCloseSubscription = arenaHub.onClose.subscribe(() => {
      console.log('Connection error');
    });
    this.synchronizationErrorSubscription = arenaHub.synchronizationErrorState.subscribe((value) => {
      if (value) {
        console.log('Synchronization error');
      }
    });
    this.arenaActionsSubscription = arenaHub.battleSynchronizationActionsNotifier.subscribe(() => {
      if (this.receivingMessagesFromHubAllowed) {
        this.processActionsFromQueue();
      }
    });
  }

  ngOnDestroy(): void {
    this.battleStorageService.clear();
    this.onCloseSubscription.unsubscribe();
    this.arenaActionsSubscription.unsubscribe();
    this.synchronizationErrorSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.battleStorageService.version = 0;
    const loadBattle = this.activatedRoute.snapshot.data.battle;
    if (loadBattle) {
      console.log('Restore game');
      const snapshot = this.battleResolver.popBattleSnapshot();
      this.battleSynchronizerService.restoreSceneFromSnapshot(snapshot);
      this.battleStorageService.version = snapshot.version;
    }
    this.processActionsFromQueue();
  }

  private processActionsFromQueue() {
    if (this.battleStorageService.version + 1 < this.arenaHub.firstActionVersion) {
      console.log('Version issue');
      return;
    }
    this.receivingMessagesFromHubAllowed = false;
    let action: { action: BattleSynchronizationActionEnum, sync: Synchronizer };
    // tslint:disable-next-line: no-conditional-assignment
    while (action = this.arenaHub.pickBattleSynchronizationAction(this.battleStorageService.version)) {
      if (this.specificActionResponseForWait &&
        (action.action !== this.specificActionResponseForWait.action ||
        !action.sync.tempActor ||
        action.sync.tempActor !== this.specificActionResponseForWait.actorId)) {
        console.log('Awaiting action version issue');
        return;
      }
      this.specificActionResponseForWait = undefined;
      const currentPlayer = action.sync.players.find(x => x.id === this.userService.user.id);
      if (currentPlayer.status === BattlePlayerStatusEnum.Defeated) {
        console.log('DEFEAT');
        return;
      }
      switch (action.action) {
        case BattleSynchronizationActionEnum.StartGame:
          console.log('Start game');
          // TODO Add some introducing animations
          this.battleSynchronizerService.restoreSceneFromSnapshot(action.sync);
          break;
        case BattleSynchronizationActionEnum.EndGame:
          if (currentPlayer.status === BattlePlayerStatusEnum.Victorious) {
            console.log('VICTORY');
          }
          this.battleSynchronizerService.synchronizeScene(action.sync);
          return;
        case BattleSynchronizationActionEnum.NoActorsDraw:
          console.log('DRAW');
          this.battleSynchronizerService.synchronizeScene(action.sync);
          break;
        case BattleSynchronizationActionEnum.SkipTurn:
          console.log('Skip turn');
          this.battleSynchronizerService.synchronizeScene(action.sync);
          break;
        case BattleSynchronizationActionEnum.EndTurn:
          console.log('End turn');
          this.battleSynchronizerService.synchronizeScene(action.sync);
          break;
        case BattleSynchronizationActionEnum.Attack:
          console.log('Attack');
          this.battleSynchronizerService.synchronizeScene(action.sync);
          break;
        case BattleSynchronizationActionEnum.Cast:
          console.log('Cast');
          this.battleSynchronizerService.synchronizeScene(action.sync);
          break;
        case BattleSynchronizationActionEnum.Decoration:
          console.log('Decoration acts');
          this.battleSynchronizerService.synchronizeScene(action.sync);
          break;
        case BattleSynchronizationActionEnum.Move:
          console.log('Move');
          this.battleSynchronizerService.synchronizeScene(action.sync);
          break;
        case BattleSynchronizationActionEnum.Wait:
          console.log('Wait');
          this.battleSynchronizerService.synchronizeScene(action.sync);
          break;
      }
      this.battleStorageService.version = action.sync.version;
      if (this.battleStorageService.version + 1 < this.arenaHub.firstActionVersion) {
        console.log('Version issue');
        return;
      }
    }
    this.receivingMessagesFromHubAllowed = true;
  }
}
