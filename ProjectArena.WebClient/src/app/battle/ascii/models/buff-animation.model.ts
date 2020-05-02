import { Actor } from './scene/actor.model';
import { Color } from 'src/app/shared/models/color.model';

export interface BuffAnimation {
  colorDifference?: Color;
  changedChar?: boolean;
  doSomethingWithBearer: (animation: BuffAnimation, bearer: Actor) => void;
  resetEffect: (animation: BuffAnimation, bearer: Actor) => void;
}
