import { Actor } from './actor.model';
import { Color } from 'src/app/shared/models/color.model';

export interface Buff {
  id: number;

  char: string;
  color: Color;
  name: string;
  description: string;
  onApplyAnimation: Animation;
  effectAnimation: Animation;
  onPurgeAnimation: Animation;

  onApplyEffect: (actor: Actor) => void;
  onRemoveEffect: (actor: Actor) => void;

  mod: number;
  duration?: number;
}
