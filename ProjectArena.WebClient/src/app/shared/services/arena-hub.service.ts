import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Observable, Subject } from 'rxjs';
import { ExternalResponse } from '../models/external-response.model';
import { Synchronizer } from '../models/battle/synchronizer.model';

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

@Injectable()
export class ArenaHubService {

  private hubConnection: signalR.HubConnection;

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl('hub')
    .build();
    this.addBattleListeners();
  }

  connect(): Observable<ExternalResponse<any>> {
    const subject = new Subject<ExternalResponse<any>>();
    this.hubConnection
        .start()
        .then(() => {
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
        });
    return subject;
  }

  disconnect(): any {
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

  private prepareListener() {
    console.log('BattlePrepare');
  }

  private syncErrorListener() {
    console.log('SynchronizationError');
  }

  private startGameListener(synchronizer: Synchronizer) {
    console.log({action: 'StartGame', sync: synchronizer});
  }

  private moveListener(synchronizer: Synchronizer) {
    console.log({action: 'Move', sync: synchronizer});
  }

  private attackListener(synchronizer: Synchronizer) {
    console.log({action: 'Attack', sync: synchronizer});
  }

  private castListener(synchronizer: Synchronizer) {
    console.log({action: 'Cast', sync: synchronizer});
  }

  private waitListener(synchronizer: Synchronizer) {
    console.log({action: 'Wait', sync: synchronizer});
  }

  private decorationListener(synchronizer: Synchronizer) {
    console.log({action: 'Decoration', sync: synchronizer});
  }

  private endTurnListener(synchronizer: Synchronizer) {
    console.log({action: 'EndTurn', sync: synchronizer});
  }

  private endGameListener(synchronizer: Synchronizer) {
    console.log({action: 'EndGame', sync: synchronizer});
  }

  private skipTurnListener(synchronizer: Synchronizer) {
    console.log({action: 'SkipTurn', sync: synchronizer});
  }

  private noActorsDrawListener(synchronizer: Synchronizer) {
    console.log({action: 'NoActorsDraw', sync: synchronizer});
  }

  private  addBattleListeners() {
    this.addNewListener(BATTLE_PREPARE, () => this.prepareListener());
    this.addNewListener(BATTLE_SYNC_ERROR, () => this.syncErrorListener());
    this.addNewListener(BATTLE_START_GAME, (synchronizer: Synchronizer) => this.startGameListener(synchronizer));
    this.addNewListener(BATTLE_MOVE, (synchronizer: Synchronizer) => this.moveListener(synchronizer));
    this.addNewListener(BATTLE_ATTACK, (synchronizer: Synchronizer) => this.attackListener(synchronizer));
    this.addNewListener(BATTLE_CAST, (synchronizer: Synchronizer) => this.castListener(synchronizer));
    this.addNewListener(BATTLE_WAIT, (synchronizer: Synchronizer) => this.waitListener(synchronizer));
    this.addNewListener(BATTLE_DECORATION, (synchronizer: Synchronizer) => this.decorationListener(synchronizer));
    this.addNewListener(BATTLE_END_TURN, (synchronizer: Synchronizer) => this.endTurnListener(synchronizer));
    this.addNewListener(BATTLE_END_GAME, (synchronizer: Synchronizer) => this.endGameListener(synchronizer));
    this.addNewListener(BATTLE_SKIP_TURN, (synchronizer: Synchronizer) => this.skipTurnListener(synchronizer));
    this.addNewListener(BATTLE_NO_ACTORS_DRAW, (synchronizer: Synchronizer) => this.noActorsDrawListener(synchronizer));
  }
}
