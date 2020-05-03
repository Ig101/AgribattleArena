import { Visualization } from '../visualization.model';
import { ActionAnimation } from '../action-animation.model';

export interface TileNative {
  name: string;
  description: string;
  visualization: Visualization;
  backgroundColor: { r: number, g: number, b: number };
  bright: boolean;
  action: ActionAnimation;
  onActionEffectAnimation: ActionAnimation;
  onStepAction: ActionAnimation;
}
