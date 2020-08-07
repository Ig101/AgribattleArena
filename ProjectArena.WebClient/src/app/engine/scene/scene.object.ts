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

export const SCENE_FRAME_TIME = 1000 / 30;

export class Scene {

  id: string;

  removedActors: number[] = [];

  actionsSub: Observer<ActionInfo>;
  synchronizersSub: Observer<Synchronizer>;

  tileStubs: TileStub[] = [];
  changes: ChangeDefinition[] = [];
  changed: boolean;
  logs: Log[] = [];
  timeLine = 0;
  lastTime: number;

  width: number;
  height: number;
  tiles: Tile[][];
  players: Player[];

  currentActor: Actor;
  turnTime: number;

  waitingMessages: SynchronizationMessageDto[] = [];

  constructor(synchronizer: FullSynchronizationInfo, turnInfo: StartTurnInfo) {
    // TODO Constructor
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
    // TODO Synchronizer
    this.changed = false;
    return undefined;
  }

  private act(definition: ActionInfo) {
    const actor = this.findActorByReference(definition.actor);
    const action = actor.actions.find(x => x.id === definition.id);
    // TODO Desync
    switch (definition.type) {
      case ActionType.Untargeted:
        actor.actUntargeted(action);
        break;
      case ActionType.Targeted:
        actor.actTargeted(action, definition.x, definition.y);
        break;
      case ActionType.OnObject:
        const target = this.findActorByReference({ x: definition.x, y: definition.y, id: definition.targetId });
        actor.actOnObject(action, target);
        break;
    }
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
      this.timeLine = definition.time;
    }
  }

  private processMessage(message: SynchronizationMessageDto) {
    switch (message.id) {
      case SynchronizationMessageType.ActionDone:
        this.act(message.action);
        break;
      case SynchronizationMessageType.TurnStarted:
        this.startTurn(message.startTurnInfo);
        break;
    }
    // TODO Reward and EndGame
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

  intendedAction(definition: ActionInfo) {
    if (this.waitingMessages.length !== 0 || this.changes.length !== 0 || definition.actor.id !== this.currentActor.id) {
      return;
    }
    this.actionsSub.next(definition);
    this.act(definition);
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
