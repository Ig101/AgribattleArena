import { FullSynchronizationInfo } from './full-synchronization-info.model';
import { Synchronizer } from './synchronizer.model';

export interface Synchronization {
  sceneId: string;
  code: string;
  synchronizer: FullSynchronizationInfo;
}
