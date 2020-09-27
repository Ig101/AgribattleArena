import { Tile } from './tile.object';
import { Player } from './abstract/player.object';
import { Observer } from 'rxjs/internal/types';
import { Actor } from './actor.object';
import { TileStub } from '../models/abstract/tile-stub.model';
import { ChangeDefinition } from '../models/abstract/change-definition.model';
import { Log } from '../models/abstract/log.model';
import { ActorReference } from 'src/app/shared/models/synchronization/objects/actor-reference.model';
import { FinishSceneMessage } from 'src/app/shared/models/synchronization/finish-scene-message.model';
import { ActionType } from 'src/app/shared/models/enum/action-type.enum';
import { FinishSceneMessageType } from 'src/app/shared/models/enum/finish-scene-message-type.enum';
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
import { FRAMES_PER_TURN, getMostPrioritizedAction, SCENE_FRAME_TIME } from '../engine.helper';
import { randomBytes } from 'crypto';
import { ReactionSynchronization } from 'src/app/shared/models/synchronization/objects/reaction-synchronization.model';
import { Synchronizer } from 'src/app/shared/models/synchronization/synchronizer.model';
import { BattlePlayerStatusEnum } from 'src/app/shared/models/enum/player-battle-status.enum';

export class Scene {

  id: string;
  hash: number;
  currentPlayer: Player;
  currentActor: Actor;
  automatic = false;
  waitingInput = false;

  biom: BiomEnum;

  visualizationChanged = true;
  changed: boolean;

  removedActors: ActorReference[] = [];

  tileStubs: TileStub[] = [];
  changes: ChangeDefinition[] = [];
  logs: Log[] = [];
  timeLine = 0;
  idCounterPosition = 0;
  lastTime: number;

  width: number;
  height: number;
  tiles: Tile[][];
  players: Player[];

  framesCounter = 0;

  waitingAction: () => void;

  constructor(
    public desyncSub: Observer<boolean>,
    public nativesCollection: INativesCollection,
    private endGameSub: Observer<FullSynchronizationInfo>,
    synchronizer: FullSynchronizationInfo) {

    this.desyncSub.next(false);

    this.timeLine = synchronizer.timeLine;
    this.automatic = synchronizer.automatic;
    this.idCounterPosition = synchronizer.idCounterPosition;
    this.players = synchronizer.players.map(x => new Player(x));
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
      const tile = this.tiles[actor.reference.x][actor.reference.y];
      tile.actors.push(new Actor(this, tile, actor));
    }
    this.currentActor = this.findActorByReference(synchronizer.currentActor);
    this.currentPlayer = this.currentActor.owner;
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

  private createFullSynchronizer(): FullSynchronizationInfo {
    const actors: ActorSynchronization[] = [];
    for (let x = 0; x < this.width; x++) {
      for (let y = 0; y < this.height; y++) {
        actors.push(...this.tiles[x][y].createFullSynchronizer());
      }
    }
    return {
      id: this.id,
      automatic: this.automatic,
      timeLine: this.timeLine,
      idCounterPosition: this.idCounterPosition,
      currentActor: this.currentActor.reference,
      actors,
      players: this.players.map(p => ({
        id: p.id,
        battlePlayerStatus: p.battlePlayerStatus,
        keyActors: p.keyActors
      })),
      width: this.width,
      height: this.height,
      biom: this.biom
    };
  }

  private act(actor: Actor, action: Action, type: ActionType, x?: number, y?: number, target?: IActor) {
    if (!actor || !action || action.remainedTime > 0) {
      this.desyncSub.next(true);
      return;
    }
    action.remainedTime = action.native.cooldown;
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
    initiative: number,
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
      initiative,
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

  private actOnResetting(playerAction: { action: Action, type: ActionType, x?: number, y?: number, target?: IActor }) {
    // TODO Actors AI Actions
    if (playerAction) {
      this.act(this.currentActor, playerAction.action, playerAction.type, playerAction.x, playerAction.y, playerAction.target);
    }
  }

  private checkWinCondition() {
    if (this.currentPlayer.keyActors.length === 0) {
      this.currentPlayer.battlePlayerStatus = BattlePlayerStatusEnum.Defeated;
      this.endGameSub.next(this.createFullSynchronizer());
    } else if (!this.players.some(x => x.id !== this.currentPlayer.id && x.keyActors.length > 0)) {
      this.currentPlayer.battlePlayerStatus = BattlePlayerStatusEnum.Victorious;
      this.endGameSub.next(this.createFullSynchronizer());
    }
  }

  private resetTurn() {
    for (let x = 0; x < this.width; x++) {
      for (let y = 0; y < this.height; y++) {
        this.tiles[x][y].update();
      }
    }
    this.checkWinCondition();
    if (this.automatic ||
      !this.currentActor ||
      !this.currentActor.isAlive ||
      !this.currentActor.actions.some(x =>
        x.remainedTime <= 0 &&
        (x.native.actionClass === ActionClassEnum.Attack ||
        x.native.actionClass === ActionClassEnum.Move ||
        x.native.actionClass === ActionClassEnum.Default))) {
      this.actOnResetting(undefined);
    } else if (this.waitingAction) {
      this.waitingAction();
      this.waitingAction = undefined;
    } else {
      this.waitingInput = true;
    }
  }

  update() {
    const newTime = performance.now();
    const shift = (newTime - this.lastTime) / 1000;
    this.lastTime = newTime;

    if (this.automatic) {
      this.waitingInput = false;
    }

    if (this.waitingInput) {
      return shift;
    }

    this.timeLine += shift;

    if (this.framesCounter > FRAMES_PER_TURN) {
      this.framesCounter = 0;
      this.resetTurn();
    } else {
      this.framesCounter++;
    }

    if (this.changes.length !== 0) {
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

  private intendedAction(action: Action, type: ActionType, x?: number, y?: number, target?: IActor) {
    if (!this.currentActor.isAlive || !this.currentActor.actions.some(a => a.id === action.id) || (target && !target.isAlive)) {
      return;
    }
    this.waitingInput = false;
    this.actOnResetting({ action, type, x, y, target });
  }

  intendedTargetedAction(action: Action, x: number, y: number) {
    if (!this.waitingInput && !this.automatic) {
      this.waitingAction = () => this.intendedAction(action, ActionType.Targeted, x, y);
    } else {
      this.intendedAction(action, ActionType.Targeted, x, y);
    }
  }

  intendedOnObjectAction(action: Action, target: IActor) {
    if (!this.waitingInput && !this.automatic) {
      this.waitingAction = () => this.intendedAction(action, ActionType.OnObject, target.x, target.y, target);
    } else {
      this.intendedAction(action, ActionType.OnObject, target.x, target.y, target);
    }
  }
}
