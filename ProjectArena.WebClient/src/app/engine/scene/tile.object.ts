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

  get isRoot() {
    return true;
  }

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
    return 99999;
  }

  get volume() {
    return 99999;
  }

  get freeVolume() {
    return 99999;
  }

  get height() {
    return this.actors.reduce((a, b) => a + b.height, 0);
  }

  constructor(scene: Scene, id: number, x: number, y: number) {
    this.actors = [];
    this.id = id;
    this.parentScene = scene;
    this.positionX = x;
    this.positionY = y;
  }

  getActorZ(actor: Actor) {
    const index = this.actors.indexOf(actor);
    let result = 0;
    for (let  i = 0; i < index; i++) {
      result += this.actors[i].height;
    }
    return result;
  }

  handleEffects(effects: string[], power: number, containerized: boolean, order: number, startingTime, issuer: Actor) {
    let resultPower = power;
    for (const actor of this.actors) {
      const powerChange = actor.handleEffects(effects, power, true, order + 1, startingTime, issuer) - power;
      resultPower += powerChange;
    }
    return resultPower;
  }

  kill() { }

  applyBuff(buff: Buff) { }

  purgeBuffs() { }

  update() {
    for (const actor of this.actors) {
      actor.update();
    }
  }

  changeDurability(durability: number) { }

  move(target: IActor) { }

  addActorOnTop(actor: Actor) {
    this.actors.push(actor);
    actor.parentActor = this;
  }

  addActor(actor: Actor, index: number) {
    if (index >= this.actors.length) {
      this.addActorOnTop(actor);
      return;
    }
    actor.parentActor = this;
    this.actors.splice(Math.max(0, index), 0, actor);
  }


  removeActor(actor: Actor) {
    this.parentScene.removedActors.push(actor.reference);
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

  getActiveActors(): Actor[] {
    const activeActors = [];
    for (const actor of this.actors) {
      activeActors.push(...actor.getActiveActors());
    }
    return activeActors;
  }

  getNeighboursByScope(scope: number) {
    return [this];
  }

  getChildrenByScope(height: number, scope: number) {
    const actors: Actor[] = [];
    const lowerBound = height - scope;
    const highBound = height + scope;
    let z = 0;
    for (const actor of this.actors) {
      if (z > highBound) {
        break;
      }
      z += actor.height;
      if (z > lowerBound) {
        actors.push(actor);
      }
    }
    return actors;
  }

  setChanged() { }
}
