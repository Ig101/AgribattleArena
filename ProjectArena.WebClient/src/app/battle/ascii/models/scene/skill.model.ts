import { TwoPhaseActionAnimation } from '../two-phase-action-animation.model';
import { Targets } from 'src/app/shared/models/battle/targets.model';

export interface Skill {
  id: number;

  name: string;
  description: string;
  action: TwoPhaseActionAnimation;

  range: number;
  cd: number;
  cost: number;
  preparationTime: number;
  availableTargets: Targets;
  onlyVisibleTargets: boolean;
}
