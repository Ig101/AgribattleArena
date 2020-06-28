import { Injectable, EventEmitter } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { ExternalResponse } from '../models/external-response.model';
import { Synchronizer } from '../models/battle/synchronizer.model';
import { BattleSynchronizationActionEnum } from '../models/enum/battle-synchronization-action.enum';
import { LoadingScene } from '../models/loading/loading-scene.model';
import { LoadingTile } from '../models/loading/loading-tile.model';
import { tileNatives, actorNatives, decorationNatives } from 'src/app/battle/ascii/natives';
import { heightImpact, brightImpact } from 'src/app/battle/ascii/helpers/scene-draw.helper';
import { UserService } from './user.service';
import { LoadingService } from './loading.service';
import { DailyChanges } from '../models/daily-changes.model';

const DAILY_UPDATE = 'DailyUpdate';
const BATTLE_SYNC_ERROR = 'BattleSynchronizationError';
const BATTLE_USUCCESSFUL_ACTION = 'BattleUnsuccessfulAction';
const BATTLE_START_GAME = 'BattleStartGame';
const BATTLE_MOVE = 'BattleMove';
const BATTLE_ATTACK = 'BattleAttack';
const BATTLE_CAST = 'BattleCast';
const BATTLE_WAIT = 'BattleWait';
const BATTLE_DECORATION = 'BattleDecoration';
const BATTLE_END_TURN = 'BattleEndTurn';
const BATTLE_END_GAME = 'BattleEndGame';
const BATTLE_LEAVE = 'BattleLeave';
const BATTLE_NO_ACTORS_DRAW = 'BattleNoActorsDraw';

type BattleHubReturnMethod = typeof DAILY_UPDATE | typeof BATTLE_ATTACK | typeof BATTLE_CAST | typeof BATTLE_DECORATION |
    typeof BATTLE_END_GAME | typeof BATTLE_END_TURN | typeof BATTLE_MOVE | typeof BATTLE_NO_ACTORS_DRAW |
    typeof BATTLE_LEAVE | typeof BATTLE_START_GAME | typeof BATTLE_SYNC_ERROR | typeof BATTLE_WAIT | typeof BATTLE_USUCCESSFUL_ACTION;

@Injectable({
  providedIn: 'root'
})
export class ArenaHubService {

  onClose = new EventEmitter();

  firstActionVersion: number;
  battleSynchronizationActionsList: { action: BattleSynchronizationActionEnum, sync: Synchronizer }[] = [];
  battleSynchronizationActionsNotifier = new Subject<any>();
  dailyUpdateNotifier = new Subject<DailyChanges>();

  prepareForBattleNotifier = new BehaviorSubject<LoadingScene>(undefined);

  synchronizationErrorState = new BehaviorSubject<boolean>(false);
  unsuccessfulActionSubject = new Subject<boolean>();

  connected: boolean;

  private userId: string;
  private hubConnection: signalR.HubConnection;

  constructor(
    private loadingService: LoadingService
    ) {
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl('hub')
    .build();
    this.addBattleListeners();
    this.hubConnection.onclose(() => {
        if (this.connected) {
            this.onClose.emit();
        }
    });
  }

  connect(userId: string): Observable<ExternalResponse<any>> {
    this.userId = userId;
    const subject = new Subject<ExternalResponse<any>>();
    this.hubConnection
        .start()
        .then(() => {
            this.connected = true;
            subject.next({
                statusCode: 200
            } as ExternalResponse<any>);
            subject.complete();
        })
        .catch(() => {
            subject.next({
                statusCode: 500,
                errors: ['Unexpected connection error. Try again later...']
            } as ExternalResponse<any>);
            subject.complete();
            this.onClose.emit();
        });
    return subject;
  }

  disconnect(): any {
    this.connected = false;
    this.hubConnection.stop();
  }

  private catchHubError(error: any) {
    console.error(error);
    this.loadingService.startLoading({
      title: 'Server connection is lost. Refresh the page or try again later...'
    }, 0, true);
  }

  addNewListener(methodName: BattleHubReturnMethod, listener: (synchronizer: Synchronizer) => void) {
      this.hubConnection.on(methodName, listener);
  }

  orderAttack(sceneId: string, actorId: number, targetX: number, targetY: number) {
    this.hubConnection
      .invoke('OrderAttackAsync', sceneId, actorId, targetX, targetY).catch(err => this.catchHubError(err));
  }

  orderMove(sceneId: string, actorId: number, targetX: number, targetY: number) {
    this.hubConnection
      .invoke('OrderMoveAsync', sceneId, actorId, targetX, targetY).catch(err => this.catchHubError(err));
  }

  orderCast(sceneId: string, actorId: number, skillId: number, targetX: number, targetY: number) {
    this.hubConnection
      .invoke('OrderCastAsync', sceneId, actorId, skillId, targetX, targetY).catch(err => this.catchHubError(err));
  }

  pickBattleSynchronizationAction(currentVersion: number) {
    if (this.battleSynchronizationActionsList.length === 0) {
      return undefined;
    }
    while (this.battleSynchronizationActionsList[0].sync.version <= currentVersion) {
      this.battleSynchronizationActionsList.shift();
    }
    if (this.battleSynchronizationActionsList[0].sync.version !== currentVersion + 1) {
      return undefined;
    }
    const synchronizationObject = this.battleSynchronizationActionsList[0];
    this.battleSynchronizationActionsList.shift();
    if (this.battleSynchronizationActionsList.length > 0) {
      this.firstActionVersion = this.battleSynchronizationActionsList[0].sync.version;
    }
    if (this.prepareForBattleNotifier.value) {
      this.prepareForBattleNotifier.next(undefined);
    }
    return synchronizationObject;
  }

  private registerBattleSynchronizationAction(action: BattleSynchronizationActionEnum, sync: Synchronizer) {
    const synchronizationObject = { action, sync };
    this.battleSynchronizationActionsList.push(synchronizationObject);
    this.battleSynchronizationActionsList.sort((a, b) => a.sync.version - b.sync.version);
    this.firstActionVersion = this.battleSynchronizationActionsList[0].sync.version;
    this.battleSynchronizationActionsNotifier.next();
  }

  private  addBattleListeners() {
    this.hubConnection.on(DAILY_UPDATE, (info: DailyChanges) => { this.dailyUpdateNotifier.next(info); });

    this.addNewListener(BATTLE_USUCCESSFUL_ACTION, () => this.unsuccessfulActionSubject.next() );
    this.addNewListener(BATTLE_SYNC_ERROR, () => this.synchronizationErrorState.next(true) );
    this.addNewListener(BATTLE_START_GAME, (synchronizer: Synchronizer) => {
      const currentPlayer = synchronizer.players.find(x => x.userId === this.userId);
      const loadingScene = {
        tiles: new Array<LoadingTile[]>(synchronizer.tilesetWidth),
        width: synchronizer.tilesetWidth,
        height: synchronizer.tilesetHeight
      } as LoadingScene;
      for (let x = 0; x < loadingScene.width; x++) {
        loadingScene.tiles[x] = new Array<LoadingTile>(synchronizer.tilesetHeight);
      }
      for (const tile of synchronizer.changedTiles) {
        const tileNative = tileNatives[tile.nativeId];
        loadingScene.tiles[tile.x][tile.y] = {
          char: tileNative.visualization.char,
          color: heightImpact(tile.height, tileNative.visualization.color),
          backgroundColor: heightImpact(tile.height, tileNative.backgroundColor),
          bright: tileNative.bright
        };
      }
      for (const actor of synchronizer.changedActors) {
        const actorNative = actorNatives[actor.visualization];
        const owner = synchronizer.players.find(x => x.id === actor.ownerId);
        loadingScene.tiles[actor.x][actor.y].char = actorNative.visualization.char;
        loadingScene.tiles[actor.x][actor.y].color = heightImpact(actor.z, brightImpact(loadingScene.tiles[actor.x][actor.y].bright,
        actorNative.visualization.color));
      }
      for (const decoration of synchronizer.changedDecorations) {
        const decorationNative = decorationNatives[decoration.visualization];
        const owner = synchronizer.players.find(x => x.id === decoration.ownerId);
        loadingScene.tiles[decoration.x][decoration.y].char = decorationNative.visualization.char;
        loadingScene.tiles[decoration.x][decoration.y].color = heightImpact(decoration.z,
          brightImpact(loadingScene.tiles[decoration.x][decoration.y].bright,
          decorationNative.visualization.color));
      }
      this.prepareForBattleNotifier.next(loadingScene);
      this.registerBattleSynchronizationAction(BattleSynchronizationActionEnum.StartGame, synchronizer);
    });
    this.addNewListener(BATTLE_MOVE, (synchronizer: Synchronizer) =>
    this.registerBattleSynchronizationAction(BattleSynchronizationActionEnum.Move, synchronizer));
    this.addNewListener(BATTLE_ATTACK, (synchronizer: Synchronizer) =>
    this.registerBattleSynchronizationAction(BattleSynchronizationActionEnum.Attack, synchronizer));
    this.addNewListener(BATTLE_CAST, (synchronizer: Synchronizer) =>
    this.registerBattleSynchronizationAction(BattleSynchronizationActionEnum.Cast, synchronizer));
    this.addNewListener(BATTLE_WAIT, (synchronizer: Synchronizer) =>
    this.registerBattleSynchronizationAction(BattleSynchronizationActionEnum.Wait, synchronizer));
    this.addNewListener(BATTLE_DECORATION, (synchronizer: Synchronizer) =>
    this.registerBattleSynchronizationAction(BattleSynchronizationActionEnum.Decoration, synchronizer));
    this.addNewListener(BATTLE_END_TURN, (synchronizer: Synchronizer) =>
    this.registerBattleSynchronizationAction(BattleSynchronizationActionEnum.EndTurn, synchronizer));
    this.addNewListener(BATTLE_END_GAME, (synchronizer: Synchronizer) =>
    this.registerBattleSynchronizationAction(BattleSynchronizationActionEnum.EndGame, synchronizer));
    this.addNewListener(BATTLE_LEAVE, (synchronizer: Synchronizer) =>
    this.registerBattleSynchronizationAction(BattleSynchronizationActionEnum.Leave, synchronizer));
    this.addNewListener(BATTLE_NO_ACTORS_DRAW, (synchronizer: Synchronizer) =>
    this.registerBattleSynchronizationAction(BattleSynchronizationActionEnum.NoActorsDraw, synchronizer));
  }
}
