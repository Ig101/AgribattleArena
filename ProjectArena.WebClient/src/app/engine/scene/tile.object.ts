import { IActor } from '../interfaces/actor.interface';
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

  get tags() {
    return [];
  }

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

  handleEffects(effects: string[], power: number, containerized: boolean, order: number) {
    for (const actor of this.actors) {
      actor.handleEffects(effects, power, true, order + 1);
    }
  }

  applyBuff(buff: Buff) { }

  purgeBuffs() { }

  changeDurability(durability: number) { }

  move(target: IActor) { }

  addActor(actor: Actor) {
    this.actors.push(actor);
  }

  removeActor(actor: Actor) {
    removeFromArray(this.actors, actor);
  }

  startTurn() {
    const result = [];
    for (const actor of this.actors) {
      result.push(...actor.startTurn());
    }
    return result;
  }
}
