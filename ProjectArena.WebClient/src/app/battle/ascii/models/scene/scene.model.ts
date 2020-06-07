import { Actor } from './actor.model';
import { ActiveDecoration } from './active-decoration.model';
import { SpecEffect } from './spec-effect.model';
import { Tile } from './tile.model';

export interface Scene {
  id: string;
  actors: Actor[];
  decorations: ActiveDecoration[];
  effects: SpecEffect[];
  tiles: Tile[][];
  width: number;
  height: number;
}
