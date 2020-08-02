import { ActionInfo } from './action-info.model';
import { Synchronizer } from './synchronizer.model';

export interface ActionSynchronizationDto {
  version: number;
  code: string;
  synchronizer: Synchronizer;
}
