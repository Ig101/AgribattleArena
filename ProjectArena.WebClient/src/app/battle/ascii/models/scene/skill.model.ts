import { TwoPhaseActionAnimation } from '../two-phase-action-animation.model';

export interface Skill {
  id: number;

  name: string;
  description: string;
  action: TwoPhaseActionAnimation;

  range: number;
  cd: number;
  mod: number;
  cost: number;
  preparationTime: number;
  meleeOnly: boolean;
}
