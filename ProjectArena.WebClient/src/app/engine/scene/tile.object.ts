import { IActor } from '../interfaces/actor.interface';
import { Effect } from '../content/effects';
import { Buff } from '../models/abstract/buff.model';
import { Actor } from './actor.object';
import { Scene } from './scene.object';
import { removeFromArray } from 'src/app/helpers/extensions/array.extension';

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

  get z() {
    return 0;
  }

  get height() {
    return this.actors.reduce((a, b) => a + b.height, 0);
  }

  getActorZ(actor: Actor) {
    const index = this.actors.indexOf(actor);
    let result = 0;
    for (let  i = 0; i < index; i++) {
      result += this.actors[i].height;
    }
    return result;
  }

  processEffect(effect: Effect, power: number, order: number) {
    for (const actor of this.actors) {
      actor.processEffect(effect, power, order + 1);
    }
  }

  applyBuff(buff: Buff) { }

  purgeBuffs() { }

  changeDurability(durability: number) { }

  move(x: number, y: number) { }

  addActor(actor: Actor) {
    this.actors.push(actor);
  }

  removeActor(actor: Actor) {
    removeFromArray(this.actors, actor);
  }
}
