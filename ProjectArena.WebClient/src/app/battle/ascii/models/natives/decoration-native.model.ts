import { Visualization } from '../visualization.model';
import { ActionAnimation } from '../action-animation.model';

export interface DecorationNative {
  name: string;
  description: string;
  visualization: Visualization;
  action: ActionAnimation;
  onDeathAction: ActionAnimation;

  alternativeForm: boolean;
  enemyName?: string;
  enemyDescription?: string;
  enemyVisualization?: Visualization;
  enemyAction?: ActionAnimation;
  enemyOnDeathAction?: ActionAnimation;
}
