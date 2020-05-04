import { Actor } from './actor.model';
import { Color } from 'src/app/shared/models/color.model';
import { ActionAnimation } from '../action-animation.model';
import { BuffAnimation } from '../buff-animation.model';

export interface Buff {
  id: number;

  char: string;
  color: Color;
  name: string;
  description: string;
  onApplyAnimation: ActionAnimation;
  effectAnimation: ActionAnimation;
  onPurgeAnimation: ActionAnimation;
  onActionEffectAnimation: ActionAnimation;

  passiveAnimation: BuffAnimation;

  duration?: number;
}
