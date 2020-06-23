import { Visualization } from '../visualization.model';
import { ActionAnimation } from '../action-animation.model';

export interface EffectNative {
  name: string;
  description: string;
  visualization: Visualization;
  action: ActionAnimation;
  onActionEffectAnimation: ActionAnimation;
  onDeathAction: ActionAnimation;
}
