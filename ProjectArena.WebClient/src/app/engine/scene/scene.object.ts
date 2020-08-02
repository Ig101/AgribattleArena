import { Tile } from './tile.object';
import { Player } from './abstract/player.object';
import { Observer } from 'rxjs/internal/types';
import { ActionDefinition } from '../models/action-definition.model';
import { Actor } from './actor.object';

export class Scene {

  id: string;

  definitionsSub: Observer<ActionDefinition>;

  width: number;
  height: number;
  tiles: Tile[][];
  players: Player[];

  currentActor: Actor;
  turnStarted = false;
  turnTime: number;

  startTurn() {
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
      // TODO send start turn and start turn after message
    } else {
      // TODO send end turn
    }
  }
}
