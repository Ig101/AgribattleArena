import { Targets } from '../targets.model';

export interface SyncSkill {
  id: number;
  range: number;
  visualization: string;
  cd: number;
  cost: number;
  preparationTime: number;
  availableTargets: Targets;
  onlyVisibleTargets: boolean;
}
