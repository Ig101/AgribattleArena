import { LoadingTile } from './loading-tile.model';

export interface LoadingScene {
  tiles: LoadingTile[][];
  width: number;
  height: number;
}
