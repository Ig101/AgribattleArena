import { Injectable, EventEmitter } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { ExternalResponse } from '../models/external-response.model';
import { Synchronizer } from '../models/battle/synchronizer.model';
import { BattleSynchronizationActionEnum } from '../models/enum/battle-synchronization-action.enum';

const BATTLE_PREPARE = 'BattlePrepare';
const BATTLE_SYNC_ERROR = 'BattleSynchronizationError';
const BATTLE_START_GAME = 'BattleStartGame';
const BATTLE_MOVE = 'BattleMove';
const BATTLE_ATTACK = 'BattleAttack';
const BATTLE_CAST = 'BattleCast';
const BATTLE_WAIT = 'BattleWait';
const BATTLE_DECORATION = 'BattleDecoration';
const BATTLE_END_TURN = 'BattleEndTurn';
const BATTLE_END_GAME = 'BattleEndGame';
const BATTLE_SKIP_TURN = 'BattleSkipTurn';
const BATTLE_NO_ACTORS_DRAW = 'BattleNoActorsDraw';

type BattleHubReturnMethod = typeof BATTLE_PREPARE | typeof BATTLE_ATTACK | typeof BATTLE_CAST | typeof BATTLE_DECORATION |
    typeof BATTLE_END_GAME | typeof BATTLE_END_TURN | typeof BATTLE_MOVE | typeof BATTLE_NO_ACTORS_DRAW |
    typeof BATTLE_SKIP_TURN | typeof BATTLE_START_GAME | typeof BATTLE_SYNC_ERROR | typeof BATTLE_WAIT;

@Injectable({
  providedIn: 'root'
})
export class ArenaHubService {

  onClose = new EventEmitter();

  firstActionVersion: number;
  battleSynchronizationActionsList: { action: BattleSynchronizationActionEnum, sync: Synchronizer }[] = [];
  battleSynchronizationActionsNotifier = new Subject<any>();

  prepareForBattleNotifier = new BehaviorSubject<boolean>(false);

  synchronizationErrorState = new BehaviorSubject<boolean>(false);

  connected: boolean;

  private hubConnection: signalR.HubConnection;

  constructor() {
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

  connect(): Observable<ExternalResponse<any>> {
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

  private catchHubError(error: any, errorScreenOpaque: number) {
      console.log(error);
  }

  addNewListener(methodName: BattleHubReturnMethod, listener: (synchronizer: Synchronizer) => void) {
      this.hubConnection.on(methodName, listener);
  }

  orderAttack(actorId: number, targetX: number, targetY: number, errorScreenOpaque: number = 0.5) {
    this.hubConnection
      .invoke('OrderAttackAsync', actorId, targetX, targetY).catch(err => this.catchHubError(err, errorScreenOpaque));
  }

  orderMove(actorId: number, targetX: number, targetY: number, errorScreenOpaque: number = 0.5) {
    this.hubConnection
      .invoke('OrderMoveAsync', actorId, targetX, targetY).catch(err => this.catchHubError(err, errorScreenOpaque));
  }

  orderCast(actorId: number, skillId: number, targetX: number, targetY: number, errorScreenOpaque: number = 0.5) {
    this.hubConnection
      .invoke('OrderCastAsync', actorId, skillId, targetX, targetY).catch(err => this.catchHubError(err, errorScreenOpaque));
  }

  orderWait(actorId: number, errorScreenOpaque: number = 0.5) {
    this.hubConnection
      .invoke('OrderWaitAsync', actorId).catch(err => this.catchHubError(err, errorScreenOpaque));
  }

  pickBattleSynchronizationAction(currentVersion: number) {
    if (this.battleSynchronizationActionsList.length === 0) {
      return undefined;
    }
    const synchronizationObject = this.battleSynchronizationActionsList[0];
    while (this.battleSynchronizationActionsList[0].sync.version <= currentVersion) {
      this.battleSynchronizationActionsList.shift();
    }
    if (this.battleSynchronizationActionsList[0].sync.version !== currentVersion + 1) {
      return undefined;
    }
    this.battleSynchronizationActionsList.shift();
    if (this.battleSynchronizationActionsList.length > 0) {
      this.firstActionVersion = this.battleSynchronizationActionsList[0].sync.version;
    }
    if (!this.prepareForBattleNotifier.value) {
      this.prepareForBattleNotifier.next(false);
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
    this.addNewListener(BATTLE_PREPARE, () => this.prepareForBattleNotifier.next(true) );
    this.addNewListener(BATTLE_SYNC_ERROR, () => this.synchronizationErrorState.next(true) );
    this.addNewListener(BATTLE_START_GAME, (synchronizer: Synchronizer) =>
      this.registerBattleSynchronizationAction(BattleSynchronizationActionEnum.StartGame, synchronizer));
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
    this.addNewListener(BATTLE_SKIP_TURN, (synchronizer: Synchronizer) =>
    this.registerBattleSynchronizationAction(BattleSynchronizationActionEnum.SkipTurn, synchronizer));
    this.addNewListener(BATTLE_NO_ACTORS_DRAW, (synchronizer: Synchronizer) =>
    this.registerBattleSynchronizationAction(BattleSynchronizationActionEnum.NoActorsDraw, synchronizer));
  }
}
