import { Targets } from '../targets.model';

export interface SyncSkill {
  id: number;
  range: number;
  nativeId: string;
  cd: number;
  cost: number;
  preparationTime: number;
  availableTargets: Targets;
  onlyVisibleTargets: boolean;
}
