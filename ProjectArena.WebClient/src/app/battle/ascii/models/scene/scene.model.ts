import { Actor } from './actor.model';
import { ActivaDecoration } from './active-decoration.model';
import { SpecEffect } from './spec-effect.model';
import { Tile } from './tile.model';

export interface Scene {
  tiles: Tile[][];
  width: number;
  height: number;
}
