import { Actor } from 'src/app/battle/ascii/models/scene/actor.model';
import { Scene } from 'src/app/battle/ascii/models/scene/scene.model';
import { IActor } from '../interfaces/actor.interface';

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
}
