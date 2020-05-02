import { AnimationTile } from './animation-tile.model';
import { Synchronizer } from 'src/app/shared/models/battle/synchronizer.model';
import { BattleSynchronizationActionEnum } from 'src/app/shared/models/enum/battle-synchronization-action.enum';

export interface AnimationDeclaration {
  waitingForSynchronizer?: BattleSynchronizationActionEnum;
  updateSynchronizer?: { action: BattleSynchronizationActionEnum, sync: Synchronizer };
  animationTiles: AnimationTile[][];
}
