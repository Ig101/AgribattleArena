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
import { convertTile, convertActor, convertDecoration, convertEffect } from '../helpers/scene.helper';
import { Actor } from '../models/scene/actor.model';
import { ActiveDecoration } from '../models/scene/active-decoration.model';
import { SpecEffect } from '../models/scene/spec-effect.model';

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
    private arenaHub: ArenaHubService,
    private userService: UserService
  ) {
    this.onCloseSubscription = arenaHub.onClose.subscribe(() => {
      console.log('Connection error');
    });
    this.synchronizationErrorSubscription = arenaHub.synchronizationErrorState.subscribe(() => {
      console.log('Synchronization error');
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
      this.restoreSceneFromSnapshot(snapshot);
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
        action.sync.tempActor.id !== this.specificActionResponseForWait.actorId)) {
        console.log('Awaiting action version issue');
        return;
      }
      this.specificActionResponseForWait = undefined;
      const currentPlayer = this.battleStorageService.players.find(x => x.id === this.userService.user.id);
      if (currentPlayer.status === BattlePlayerStatusEnum.Defeated) {
        console.log('DEFEAT');
        return;
      }
      switch (action.action) {
        case BattleSynchronizationActionEnum.StartGame:
          console.log('Start game');
          // TODO Add some introducing animations
          this.restoreSceneFromSnapshot(action.sync);
          break;
        case BattleSynchronizationActionEnum.EndGame:
          if (currentPlayer.status === BattlePlayerStatusEnum.Victorious) {
            console.log('VICTORY');
          }
          return;
          break;
        case BattleSynchronizationActionEnum.NoActorsDraw:
          console.log('DRAW');
          break;
        case BattleSynchronizationActionEnum.SkipTurn:
          console.log('Skip turn');
          break;
        case BattleSynchronizationActionEnum.EndTurn:
          console.log('End turn');
          break;
        case BattleSynchronizationActionEnum.Attack:
          console.log('Attack');
          break;
        case BattleSynchronizationActionEnum.Cast:
          console.log('Cast');
          break;
        case BattleSynchronizationActionEnum.Decoration:
          console.log('Decoration acts');
          break;
        case BattleSynchronizationActionEnum.Move:
          console.log('Move');
          break;
        case BattleSynchronizationActionEnum.Wait:
          console.log('Wait');
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

  private restoreSceneFromSnapshot(synchronizer: Synchronizer) {
    // Players
    let currentPlayer: Player;
    this.battleStorageService.players = synchronizer.players.map(player => {
      const isCurrentPlayer = player.id === this.userService.user.id;
      const newPlayer = {
        id: player.id,
        team: player.team,
        name: isCurrentPlayer ? this.userService.user.name : 'Demonspawn gang',
        keyActors: player.keyActorsSync,
        turnsSkipped: player.turnsSkipped,
        status: player.status
      };
      if (isCurrentPlayer) {
        currentPlayer = newPlayer;
      }
      return newPlayer;
    });
    this.battleStorageService.turnTime = synchronizer.turnTime;

    // Tiles
    const tiles = new Array<Tile[]>(synchronizer.tilesetWidth);
    for (let x = 0; x < synchronizer.tilesetWidth; x++) {
      tiles[x] = new Array<Tile>(synchronizer.tilesetHeight);
    }
    for (const tile of synchronizer.changedTiles) {
      const owner = this.battleStorageService.players.find(x => x.id === tile.ownerId);
      tiles[tile.x][tile.y] = convertTile(tile, owner);
    }

    // Actors
    const actors: Actor[] = [];
    for (const actor of synchronizer.changedActors) {
      const owner = this.battleStorageService.players.find(x => x.id === actor.ownerId);
      const newActor = convertActor(actor, owner, owner && currentPlayer.team === owner.team);
      tiles[newActor.x][newActor.y].actor = newActor;
      actors.push(newActor);
    }

    // Decorations
    const decorations: ActiveDecoration[] = [];
    for (const decoration of synchronizer.changedDecorations) {
      const owner = this.battleStorageService.players.find(x => x.id === decoration.ownerId);
      const newDecoration = convertDecoration(decoration, owner, owner && currentPlayer.team === owner.team);
      tiles[newDecoration.x][newDecoration.y].decoration = newDecoration;
      decorations.push(newDecoration);
    }

    // SpecEffects
    const effects: SpecEffect[] = [];
    for (const specEffect of synchronizer.changedEffects) {
      const owner = this.battleStorageService.players.find(x => x.id === specEffect.ownerId);
      const newEffect = convertEffect(specEffect, owner, owner && currentPlayer.team === owner.team);
      tiles[newEffect.x][newEffect.y].specEffects.push(newEffect);
      effects.push(newEffect);
    }

    // TempValues
    this.battleStorageService.currentActor = synchronizer.tempActor ?
      actors.find(x => x.id === synchronizer.tempActor.id) :
      undefined;
    this.battleStorageService.currentDecoration = synchronizer.tempDecoration ?
      decorations.find(x => x.id === synchronizer.tempDecoration.id) :
      undefined;

    this.battleStorageService.scene = {
      actors,
      decorations,
      effects,
      tiles,
      width: synchronizer.tilesetWidth,
      height: synchronizer.tilesetHeight
    } as Scene;
  }

  private applySynchronizationInfo(synchronizer: Synchronizer, onlyForIds?: number[]) {

  }

}
