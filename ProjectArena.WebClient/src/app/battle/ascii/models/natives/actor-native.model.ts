import { Visualization } from '../visualization.model';
import { TwoPhaseActionAnimation } from '../two-phase-action-animation.model';

export interface ActorNative {
  name: string;
  description: string;
  visualization: Visualization;
}
