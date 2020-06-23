import { Visualization } from '../visualization.model';
import { ActionAnimation } from '../action-animation.model';

export interface DecorationNative {
  name: string;
  description: string;
  active: boolean;
  visualization: Visualization;
  action: ActionAnimation;
  onDeathAction: ActionAnimation;
}
