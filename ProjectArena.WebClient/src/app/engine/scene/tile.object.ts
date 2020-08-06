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

  get isAlive() {
    return true;
  }

  get durability() {
    return 9999999;
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

  handleEffects(effects: string[], power: number, containerized: boolean, order: number, startingTime) {
    let resultPower = power;
    for (const actor of this.actors) {
      const powerChange = actor.handleEffects(effects, power, true, order + 1, startingTime) - power;
      resultPower += powerChange;
    }
    return resultPower;
  }

  kill() { }

  applyBuff(buff: Buff) { }

  purgeBuffs() { }

  updateBuffs() {
    for (const actor of this.actors) {
      actor.updateBuffs();
    }
  }

  update() {
    this.updateBuffs();
  }

  changeDurability(durability: number) { }

  move(target: IActor) { }

  addActor(actor: Actor) {
    this.actors.push(actor);
  }

  removeActor(actor: Actor) {
    removeFromArray(this.actors, actor);
  }

  findActor(id: number): Actor {
    for (const actor of this.actors) {
      const neededActor = actor.findActor(id);
      if (neededActor) {
        return neededActor;
      }
    }
    return undefined;
  }

  createSynchronizerAndClearInfo() {
    return this.actors.filter(x => x.changed).map(x => x.createSynchronizerAndClearInfo());
  }
}
