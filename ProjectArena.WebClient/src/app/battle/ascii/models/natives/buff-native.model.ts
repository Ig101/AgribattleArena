import { Actor } from '../scene/actor.model';
import { Color } from 'src/app/shared/models/color.model';
import { ActionAnimation } from '../action-animation.model';
import { BuffAnimation } from '../buff-animation.model';

export interface BuffNative {
  char: string;
  color: Color;
  name: string;
  description: string;
  onApplyAnimation: ActionAnimation;
  effectAnimation: ActionAnimation;
  onActionEffectAnimation: ActionAnimation;
  onPurgeAnimation: ActionAnimation;
  passiveAnimation: BuffAnimation;
}
