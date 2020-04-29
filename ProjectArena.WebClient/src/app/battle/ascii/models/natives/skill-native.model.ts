import { ActionAnimation } from '../action-animation.model';

export interface SkillNative {
  name: string;
  description: string;
  action: ActionAnimation;

  alternativeForm: boolean;
  enemyName?: string;
  enemyAction?: ActionAnimation;
}
