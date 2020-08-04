import { Tile } from './tile.object';
import { Player } from './abstract/player.object';
import { Observer } from 'rxjs/internal/types';
import { Actor } from './actor.object';
import { TileStub } from '../models/abstract/tile-stub.model';
import { ChangeDefinition } from '../models/abstract/change-definition.model';
import { Synchronizer } from 'src/app/shared/models/battle/synchronizer.model';
import { ActionInfo } from 'src/app/shared/models/synchronization/action-info.model';
import { StartTurnInfo } from 'src/app/shared/models/synchronization/start-turn-info.model';

export const SCENE_FRAME_TIME = 1 / 30;

export class Scene {

  id: string;

  removedActors: number[] = [];

  actionsSub: Observer<ActionInfo>;
  synchronizersSub: Observer<Synchronizer>;

  tileStubs: TileStub[] = [];
  changes: ChangeDefinition[] = [];
  timeLine = 0;
  lastTime: number;

  width: number;
  height: number;
  tiles: Tile[][];
  players: Player[];

  currentActor: Actor;
  turnStarted = false;
  turnTime: number;

  pushChanges(changes: ChangeDefinition[]) {
    if (changes && changes.length > 0) {
      this.changes.push(...changes);
    }
  }

  pushTimeline() {
    const newTime = performance.now();
    const shift = newTime - this.lastTime;
    this.lastTime = newTime;
    this.timeLine += shift;
    const currentChanges = this.changes.filter(x => x.time <= this.timeLine);
    this.changes = this.changes.filter(x => x.time > this.timeLine);
    this.tileStubs = this.tileStubs.filter(x => x.endTime > this.timeLine);
    for (const change of currentChanges) {
      change.action();
    }

    if (this.timeLine > 100000000) {
      this.timeLine = 0;
    }
  }

  startTurn(definition: StartTurnInfo) {
    for (let x = 0; x < this.width; x++) {
      for (let y = 0; y < this.height; y++) {
        this.tiles[x][y].update();
      }
    }
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
