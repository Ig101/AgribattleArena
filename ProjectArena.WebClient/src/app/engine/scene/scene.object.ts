import { Actor } from 'src/app/battle/ascii/models/scene/actor.model';
import { Tile } from './tile.object';
import { Player } from './abstract/player.object';
import { Observer } from 'rxjs/internal/types';
import { ActionDefinition } from '../models/action-definition.model';

export class Scene {

  id: string;

  definitionsSub: Observer<ActionDefinition>;

  tiles: Tile[][];
  players: Player[];
}
