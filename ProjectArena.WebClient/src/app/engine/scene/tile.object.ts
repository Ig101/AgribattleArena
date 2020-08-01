import { Actor } from 'src/app/battle/ascii/models/scene/actor.model';
import { Scene } from 'src/app/battle/ascii/models/scene/scene.model';
import { IActor } from '../interfaces/actor.interface';
import { Effect } from '../content/effects';
import { Buff } from './abstract/buff.object';

// Dummy actor that cannot be changed
export class Tile implements IActor {

  id: number;

  positionX: number;
  positionY: number;

  actors: Actor[];
  parentScene: Scene;

  get isAlive() {
    return true;
  }

  set isAlive(_) { }

  get owner() {
    return undefined;
  }

  set owner(_) { }

  get x() {
    return this.positionX;
  }

  get y() {
    return this.positionY;
  }

  processEffect(effect: Effect, power: number, order: number) { }

  applyBuff(buff: Buff) { }

  purgeBuffs() { }

  changeDurability(durability: number) { }
}
