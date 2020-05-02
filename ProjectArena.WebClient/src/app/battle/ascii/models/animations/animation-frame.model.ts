import { AnimationTile } from './animation-tile.model';

export interface AnimationFrame {
  updateSynchronizer: boolean;
  animationTiles: AnimationTile[];
}
