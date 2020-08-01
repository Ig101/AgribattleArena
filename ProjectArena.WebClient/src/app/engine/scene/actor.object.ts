import { Scene } from './scene.object';
import { IActor } from '../interfaces/actor.interface';
import { Buff } from 'src/app/battle/ascii/models/scene/buff.model';
import { Player } from './abstract/player.object';
import { Action } from '../models/abstract/action.model';
import { Reaction } from '../models/abstract/reaction.model';

export class Actor implements IActor {

  id: number;

  durability: number;
  speed: number;
  volume: number;
  freeVolume: number;
  isActive: boolean;

  isAlive: boolean;

  buffs: Buff[];
  selfActions: Action[];
  selfReactions: Reaction[];

  actors: Actor[];
  parentActor: IActor;
  parentScene: Scene;
  owner: Player;

  get x() {
    return this.parentActor.x;
  }

  get y() {
    return this.parentActor.y;
  }
}
