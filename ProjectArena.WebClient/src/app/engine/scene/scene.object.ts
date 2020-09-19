import { Tile } from './tile.object';
import { Player } from './abstract/player.object';
import { Observer } from 'rxjs/internal/types';
import { Actor } from './actor.object';
import { TileStub } from '../models/abstract/tile-stub.model';
import { ChangeDefinition } from '../models/abstract/change-definition.model';
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
import { RewardInfo } from 'src/app/shared/models/synchronization/reward-info.model';
import { ActorSynchronization } from 'src/app/shared/models/synchronization/objects/actor-synchronization.model';
import { Color } from 'src/app/shared/models/color.model';
import { rangeBetween } from 'src/app/helpers/math.helper';
import { INativesCollection } from '../interfaces/natives-collection.interface';
import { getHashFromString } from 'src/app/helpers/extensions/hash.extension';
import { BiomEnum } from 'src/app/shared/models/enum/biom.enum';
import { ActionClassEnum } from '../models/enums/action-class.enum';
import { getMostPrioritizedAction, SCENE_FRAME_TIME } from '../engine.helper';
import { randomBytes } from 'crypto';
import { ReactionSynchronization } from 'src/app/shared/models/synchronization/objects/reaction-synchronization.model';
import { Synchronizer } from 'src/app/shared/models/synchronization/synchronizer.model';
import { BattlePlayerStatusEnum } from 'src/app/shared/models/enum/player-battle-status.enum';

export class Scene {

  id: string;
  hash: number;
  currentPlayer: Player;

  biom: BiomEnum;

  visualizationChanged = true;

  removedActors: ActorReference[] = [];

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
  waitingAction: () => void;
  turnReallyStarted = true;

  constructor(
    public actionsSub: Observer<ActionInfo>,
    public synchronizersSub: Observer<Synchronizer>,
    public desyncSub: Observer<boolean>,
    public nativesCollection: INativesCollection,
    private endGameSub: Observer<BattlePlayerStatusEnum>,
    synchronizer: FullSynchronizationInfo,
    reward: RewardInfo,
    turnInfo: StartTurnInfo) {

    // TODO Desyncs

    this.desyncSub.next(false);
    this.endGameSub.next(BattlePlayerStatusEnum.Playing);

    this.timeLine = synchronizer.timeLine;
    this.idCounterPosition = synchronizer.idCounterPosition;
    this.changed = false;
    this.players = synchronizer.players.map(x => new Player(x));
    this.currentPlayer = this.players.find(x => x.id === synchronizer.currentPlayerId);
    this.width = synchronizer.width;
    this.height = synchronizer.height;
    this.id = synchronizer.id;
    this.hash = getHashFromString(this.id);
    this.biom = synchronizer.biom;
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
    const synchronizer = {
      actors,
      removedActors: this.removedActors,
      idCounterPosition: this.idCounterPosition
    };
    this.removedActors.length = 0;
    this.changed = false;
    return synchronizer;
  }

  private act(actor: Actor, action: Action, type: ActionType, x?: number, y?: number, target?: IActor) {
    if (!actor || !action || action.remainedTime > 0) {
      this.desyncSub.next(true);
      return;
    }
    action.remainedTime = action.native.cooldown;
    this.turnTime -= action.native.timeCost;
    switch (type) {
      case ActionType.Targeted:
        actor.actTargeted(action, x, y);
        break;
      case ActionType.OnObject:
        actor.actOnObject(action, target);
        break;
    }
    this.visualizationChanged = true;
    this.changed = true;
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
    this.turnReallyStarted = false;
    if (this.currentActor) {
      this.currentActor.initiativePosition += this.currentActor.turnCost;
    }
    const actor = this.findActorByReference(definition.tempActor);
    for (let x = 0; x < this.width; x++) {
      for (let y = 0; y < this.height; y++) {
        this.tiles[x][y].update();
      }
    }

    if (actor?.isAlive) {
      this.currentActor = actor;
      this.currentActor.setChanged();
      this.currentActor.initiativePosition += this.currentActor.turnCost;
      this.turnTime = definition.time;
    }

    if (this.changes.length > 0) {
      this.changed = true;
    } else {
      this.synchronizersSub.next(this.createSynchronizerAndClearChanges());
    }
    this.visualizationChanged = true;
  }

  private processMessage(message: SynchronizationMessageDto) {
    switch (message.id) {
      case SynchronizationMessageType.ActionDone:
        this.actFromActionInfo(message.action);
        break;
      case SynchronizationMessageType.TurnStarted:
        this.startTurn(message.startTurnInfo);
        break;
      case SynchronizationMessageType.Defeated:
        this.reward = message.reward;
        this.endGameSub.next(BattlePlayerStatusEnum.Defeated);
        break;
      case SynchronizationMessageType.Victorious:
        this.reward = message.reward;
        this.endGameSub.next(BattlePlayerStatusEnum.Victorious);
        break;
    }
  }

  getTileById(id: number) {
    const tileX = Math.floor((id - 1) / this.height);
    const tileY = (id - 1) % this.height;
    return this.tiles[tileX][tileY];
  }

  createNewActor(
    source: Actor,
    x: number,
    y: number,
    char: string,
    color: Color,
    owner: Player,
    tags: string[],
    durability: number,
    turnCost: number,
    height: number,
    volume: number,
    freeVolume: number,
    preparationReactions: ReactionSynchronization[],
    activeReactions: ReactionSynchronization[],
    clearReactions: ReactionSynchronization[],
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
      ownerId: owner.id,
      tags,
      durability,
      maxDurability: durability,
      turnCost,
      initiativePosition: source.initiativePosition + Math.random() / 1000 - this.currentActor.turnCost + turnCost,
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

  getAllActiveActors(): Actor[] {
    const activeActors = [];
    for (let x = 0; x < this.width; x++) {
      for (let y = 0; y < this.height; y++) {
        activeActors.push(...this.tiles[x][y].getActiveActors());
      }
    }
    return activeActors;
  }

  pushChanges(changes: ChangeDefinition[]) {
    if (changes && changes.length > 0) {
      this.changes.push(...changes);
    }
  }

  update() {
    const newTime = performance.now();
    const shift = (newTime - this.lastTime) / 1000;
    this.lastTime = newTime;

    this.timeLine += shift;
    this.turnTime -= shift;
    if (this.changes.length === 0) {
      if (this.changed) {
        this.synchronizersSub.next(this.createSynchronizerAndClearChanges());
      }
      if (this.waitingMessages.length > 0) {
        this.processMessage(this.waitingMessages.shift());
      } else {
        this.turnReallyStarted = true;
        if (this.waitingAction) {
          this.waitingAction();
        }
      }
    } else {
      const currentChanges = this.changes.filter(x => x.time <= this.timeLine);
      this.changes = this.changes.filter(x => x.time > this.timeLine);
      for (const change of currentChanges) {
        if (change.tileStubs) {
          this.tileStubs.push(...change.tileStubs);
          this.tileStubs.sort((a, b) => b.priority - a.priority);
          this.visualizationChanged = true;
        }
        if (change.logs) {
          this.logs.push(...change.logs);
        }
        if (change.action) {
          change.action();
        }
      }
      this.visualizationChanged = true;
    }
    const tileStubsCount = this.tileStubs.length;
    this.tileStubs = this.tileStubs.filter(x => x.endTime > this.timeLine);
    if (this.tileStubs.length !== tileStubsCount) {
      this.visualizationChanged = true;
    }
    this.clearExtraLogs();

    if (this.timeLine > 1000000000) {
      this.timeLine = 0;
    }

    return shift;
  }

  pushMessages(...messages: SynchronizationMessageDto[]) {
    this.waitingMessages.push(...messages);
  }

  private intendedAction(actor: Actor, action: Action, type: ActionType, x?: number, y?: number, target?: IActor) {
    if (this.waitingMessages.length > 0 || this.changes.length > 0) {
      this.waitingAction = () => this.intendedAction(actor, action, type, x, y, target);
      return;
    }
    if (!actor.isAlive || !actor.actions.some(a => a.id === action.id) || (target && !target.isAlive)) {
      return;
    }
    const definition = {
      actor: actor.reference,
      id: action.id,
      type,
      x,
      y,
      targetId: target?.id
    } as ActionInfo;
    this.actionsSub.next(definition);
    this.act(actor, action, type, x, y, target);
  }

  intendedTargetedAction(action: Action, x: number, y: number) {
    this.intendedAction(this.currentActor, action, ActionType.Targeted, x, y);
  }

  intendedOnObjectAction(action: Action, target: IActor) {
    this.intendedAction(this.currentActor, action, ActionType.OnObject, target.x, target.y, target);
  }

  intendedUseAction(usable: Actor, x: number, y: number) {
    if (this.waitingMessages.length > 0 || this.changes.length > 0) {
      this.waitingAction = () => this.intendedUseAction(usable, x, y);
      return;
    }
    if (!usable.isAlive) {
      return;
    }
    const actions = usable.actions.filter(a => a.native.actionClass === ActionClassEnum.Use);
    const action = getMostPrioritizedAction(actions);
    if (actions === undefined) {
      return;
    }
    const definition = {
      actor: usable.reference,
      id: action.id,
      type: ActionType.Targeted,
      x,
      y
    } as ActionInfo;
    this.actionsSub.next(definition);
    this.act(usable, action, ActionType.Targeted, x, y);
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
