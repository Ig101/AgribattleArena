import { Visualization } from '../visualization.model';
import { Player } from '../player.model';
import { Actor } from './actor.model';
import { ActivaDecoration } from './active-decoration.model';
import { SpecEffect } from './spec-effect.model';
import { Color } from 'src/app/shared/models/color.model';

export interface Tile {
  x: number;
  y: number;

  name: string;
  description: string;
  visualization: Visualization;
  backgroundColor: Color;
  bright: boolean;
  action: Animation;
  onStepAction: Animation;

  actor: Actor;
  decoration: ActivaDecoration;
  specEffects: SpecEffect[];
  height: number;
  owner: Player;
}
