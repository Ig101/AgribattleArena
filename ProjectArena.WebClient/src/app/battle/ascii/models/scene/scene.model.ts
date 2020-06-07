import { Actor } from './actor.model';
import { ActiveDecoration } from './active-decoration.model';
import { SpecEffect } from './spec-effect.model';
import { Tile } from './tile.model';
import { BiomEnum } from 'src/app/shared/models/enum/biom.enum';

export interface Scene {
  id: string;
  biom: BiomEnum;
  hash: number;
  actors: Actor[];
  decorations: ActiveDecoration[];
  effects: SpecEffect[];
  tiles: Tile[][];
  width: number;
  height: number;
}
