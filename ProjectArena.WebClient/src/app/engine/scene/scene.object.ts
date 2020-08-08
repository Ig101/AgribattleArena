import { Tile } from './tile.object';
import { Player } from './abstract/player.object';
import { Observer } from 'rxjs/internal/types';
import { Actor } from './actor.object';
import { TileStub } from '../models/abstract/tile-stub.model';
import { ChangeDefinition } from '../models/abstract/change-definition.model';
import { Synchronizer } from 'src/app/shared/models/battle/synchronizer.model';
import { ActionInfo } from 'src/app/shared/models/synchronization/action-info.model';
import { StartTurnInfo } from 'src/app/shared/models/synchronization/start-turn-info.model';
import { Log } from '../models/abstract/log.model';
import { ActorReference } from 'src/app/shared/models/synchronization/objects/actor-reference.model';
import { SynchronizationMessageDto } from 'src/app/shared/models/synchronization/synchronization-message-dto.model';
import { ActionType } from 'src/app/shared/models/enum/action-type.enum';
import { SynchronizationMessageType } from 'src/app/shared/models/enum/synchronization-message-type.enum';
import { FullSynchronizationInfo } from 'src/app/shared/models/synchronization/full-synchronization-info.model';
import { Action } from '../models/abstract/action.model';
import { Target } from '@angular/compiler';
import { IActor } from '../interfaces/actor.interface';
import { INativesCollection } from '../interfaces/natives-collection.interface';
import { RewardInfo } from 'src/app/shared/models/synchronization/reward-info.model';
import { ActorSynchronization } from 'src/app/shared/models/synchronization/objects/actor-synchronization.model';
import { Color } from 'src/app/shared/models/color.model';
import { rangeBetween } from 'src/app/helpers/math.helper';

export const SCENE_FRAME_TIME = 1000 / 30;

export class Scene {

  id: string;
  currentPlayer: Player;

  removedActors: number[] = [];

  tileStubs: TileStub[] = [];
  changes: ChangeDefinition[] = [];
  changed: boolean;
  logs: Log[] = [];
  timeLine = 0;
  idCounterPosition = 0;
  lastTime: number;

  width: number;
  height: number;
  tiles: Tile[][];
  players: Player[];

  currentActor: Actor;
  turnTime = 0;

  reward: RewardInfo;

  waitingMessages: SynchronizationMessageDto[] = [];

  constructor(
    public actionsSub: Observer<ActionInfo>,
    public synchronizersSub: Observer<Synchronizer>,
    public desyncSub: Observer<boolean>,
    public nativesCollection: INativesCollection,
    private endGameSub: Observer<boolean>,
    synchronizer: FullSynchronizationInfo,
    reward: RewardInfo,
    turnInfo: StartTurnInfo) {

    // TODO Desyncs

    this.desyncSub.next(false);
    this.endGameSub.next(false);

    this.timeLine = synchronizer.timeLine;
    this.idCounterPosition = synchronizer.idCounterPosition;
    this.changed = false;
    this.players = synchronizer.players.map(x => new Player(x));
    this.currentPlayer = this.players.find(x => x.id === synchronizer.currentPlayerId);
    this.width = synchronizer.width;
    this.height = synchronizer.height;
    this.id = synchronizer.id;
    this.tiles = new Array<Tile[]>(this.width);
    let tilesCounter = 1;
    for (let x = 0; x < this.width; x++) {
      this.tiles[x] = new Array<Tile>(this.height);
      for (let y = 0; y < this.height; y++) {
        this.tiles[x][y] = new Tile(this, tilesCounter, x, y);
        tilesCounter++;
      }
    }
    for (const actor of synchronizer.actors) {
      const tile = this.getTileById(actor.parentId);
      tile.actors.push(new Actor(this, tile, actor));
    }
    this.reward = reward;
    this.waitingMessages = synchronizer.waitingActions;
    if (turnInfo) {
      const actor = this.findActorByReference(turnInfo.tempActor);
      if (actor && actor.isAlive) {
        this.currentActor = actor;
        this.turnTime = turnInfo.time;
      }
    }
  }

  private clearExtraLogs() {
    const maxLogsCount = 50;
    if (this.logs.length > maxLogsCount) {
      this.logs.splice(0, this.logs.length - maxLogsCount);
    }
  }

  private findActorByReference(reference: ActorReference) {
    return this.tiles[reference.x][reference.y].findActor(reference.id);
  }

  private createSynchronizerAndClearChanges(): Synchronizer {
    const actors: ActorSynchronization[] = [];
    for (let x = 0; x < this.width; x++) {
      for (let y = 0; y < this.height; y++) {
        actors.push(...this.tiles[x][y].createSynchronizerAndClearInfo());
      }
    }
    // TODO Synchronizer
    this.changed = false;
    return undefined;
  }

  private act(actor: Actor, action: Action, type: ActionType, x?: number, y?: number, target?: IActor) {
    if (!actor || !action || action.remainedTime > 0) {
      this.desyncSub.next(true);
      return;
    }
    action.remainedTime = action.cooldown;
    this.turnTime -= action.timeCost;
    switch (type) {
      case ActionType.Untargeted:
        actor.actUntargeted(action);
        break;
      case ActionType.Targeted:
        actor.actTargeted(action, x, y);
        break;
      case ActionType.OnObject:
        actor.actOnObject(action, target);
        break;
    }
  }

  private actFromActionInfo(definition: ActionInfo) {
    const actor = this.findActorByReference(definition.actor);
    const action = actor.actions.find(x => x.id === definition.id);
    const target = definition.type === ActionType.OnObject ?
      this.findActorByReference({ x: definition.x, y: definition.y, id: definition.targetId }) :
      undefined;
    this.act(actor, action, definition.type, definition.x, definition.y, target);
  }

  private startTurn(definition: StartTurnInfo) {
    const actor = this.findActorByReference(definition.tempActor);
    for (let x = 0; x < this.width; x++) {
      for (let y = 0; y < this.height; y++) {
        this.tiles[x][y].update();
      }
    }

    if (this.changes.length > 0) {
      this.changed = true;
    } else {
      this.synchronizersSub.next(this.createSynchronizerAndClearChanges());
    }

    if (actor.isAlive) {
      this.currentActor = actor;
      this.turnTime = definition.time;
    }
  }

  private processMessage(message: SynchronizationMessageDto) {
    switch (message.id) {
      case SynchronizationMessageType.ActionDone:
        this.actFromActionInfo(message.action);
        break;
      case SynchronizationMessageType.TurnStarted:
        this.startTurn(message.startTurnInfo);
        break;
      case SynchronizationMessageType.Rewarded:
        this.reward = message.reward;
        break;
      case SynchronizationMessageType.GameEnded:
        this.reward = message.reward;
        this.endGameSub.next(true);
        break;
    }
  }

  getTileById(id: number) {
    const tileX = Math.floor((id - 1) / this.height);
    const tileY = (id - 1) % this.height;
    return this.tiles[tileX][tileY];
  }

  createNewActor(
    x: number,
    y: number,
    char: string,
    color: Color,
    backgroundColor: Color,
    owner: Player,
    tags: string[],
    durability: number,
    turnCost: number,
    height: number,
    volume: number,
    freeVolume: number,
    preparationReactions: string[],
    activeReactions: string[],
    clearReactions: string[],
    actions: string[],
    parent?: IActor,
    position?: number
  ): Actor {
    const id = this.idCounterPosition++;
    const synchronization = {
      reference: {
        id, x, y
      },
      char,
      color,
      backgroundColor,
      ownerId: owner.id,
      tags,
      durability,
      maxDurability: durability,
      turnCost,
      initiativePosition: turnCost,
      height,
      volume,
      freeVolume,
      preparationReactions,
      activeReactions,
      clearReactions,
      actions: actions.map(a => ({
        id: a,
        remainedTime: 0
      })),
      actors: [],
      buffs: []
    } as ActorSynchronization;
    const actor = new Actor(this, parent, synchronization);
    if (parent) {
      if (position !== undefined) {
        parent.addActor(actor, position);
      } else {
        parent.addActorOnTop(actor);
      }
    }
    return actor;
  }

  getShiftedTimeframe(frames: number) {
    return this.timeLine = frames * SCENE_FRAME_TIME;
  }

  pushChanges(changes: ChangeDefinition[]) {
    if (changes && changes.length > 0) {
      this.changes.push(...changes);
    }
  }

  update() {
    const newTime = performance.now();
    const shift = newTime - this.lastTime;
    this.lastTime = newTime;

    this.timeLine += shift;
    this.turnTime -= shift;
    if (this.changes.length === 0) {
      if (this.changed) {
        this.synchronizersSub.next(this.createSynchronizerAndClearChanges());
      }
      if (this.waitingMessages.length > 0) {
        this.processMessage(this.waitingMessages.shift());
      }
    } else {
      const currentChanges = this.changes.filter(x => x.time <= this.timeLine);
      this.changes = this.changes.filter(x => x.time > this.timeLine);
      for (const change of currentChanges) {
        this.tileStubs.push(...change.tileStubs);
        if (change.logs) {
          this.logs.push(...change.logs);
        }
        change.action();
      }
    }
    this.tileStubs = this.tileStubs.filter(x => x.endTime > this.timeLine);
    this.clearExtraLogs();

    if (this.timeLine > 1000000000) {
      this.timeLine = 0;
    }
  }

  pushMessages(...messages: SynchronizationMessageDto[]) {
    this.waitingMessages.push(...messages);
  }

  canActGenerally(actor: Actor) {
    return this.canCast(actor, undefined, undefined, undefined);
  }

  canCast(actor: Actor, action: Action, x: number, y: number) {
    return this.waitingMessages.length === 0 &&
      this.changes.length === 0 &&
      actor.id === this.currentActor.id &&
      (!action ||
        (action.remainedTime <= 0 &&
        (!x || action.range >= rangeBetween(x, y, actor.x, actor.y)))) &&
      this.turnTime > 0;
  }

  private intendedAction(actor: Actor, action: Action, type: ActionType, x?: number, y?: number, target?: IActor) {
    if (!this.canCast(actor, action, x, y)) {
      return;
    }
    const definition = {
      actor: actor.reference,
      id: action.id,
      type,
      x,
      y,
      targetId: target.id
    } as ActionInfo;
    this.actionsSub.next(definition);
    this.turnTime -= action.timeCost;
    this.act(actor, action, type, x, y, target);
  }

  intendedUntargetedAction(actor: Actor, action: Action) {
    this.intendedAction(actor, action, ActionType.Untargeted);
  }

  intendedTargetedAction(actor: Actor, action: Action, x: number, y: number) {
    this.intendedAction(actor, action, ActionType.Targeted, x, y);
  }

  intendedOnObjectAction(actor: Actor, action: Action, target: IActor) {
    this.intendedAction(actor, action, ActionType.OnObject, undefined, undefined, target);
  }
  /*
    Make it server side
    startTurnAddReturnIsTurnStarted() {
    this.turnStarted = false;
    if (this.currentActor) {
      this.currentActor.initiativePosition += this.currentActor.turnCost;
    }
    const readyActors: Actor[] = [];
    for (let x = 0; x < this.width; x++) {
      for (let y = 0; y < this.height; y++) {
        readyActors.push(...this.tiles[x][y].startTurn());
      }
    }
    const nextPlayerCandidate = readyActors.length > 0 ?
      readyActors.reduce((a, b) => a.initiativePosition < b.initiativePosition ? a : b) :
      undefined;
    if (nextPlayerCandidate) {
      this.currentActor = nextPlayerCandidate;
      return true;
    } else {
      return false;
    }
  }*/
}
