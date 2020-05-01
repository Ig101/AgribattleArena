import { Visualization } from '../visualization.model';
import { Player } from '../player.model';
import { Actor } from './actor.model';
import { ActiveDecoration } from './active-decoration.model';
import { SpecEffect } from './spec-effect.model';
import { Color } from 'src/app/shared/models/color.model';
import { ActionAnimation } from '../action-animation.model';

export interface Tile {
  x: number;
  y: number;

  name: string;
  nativeId: string;
  description: string;
  visualization: Visualization;
  backgroundColor: Color;
  bright: boolean;
  action: ActionAnimation;
  onStepAction: ActionAnimation;

  actor: Actor;
  decoration: ActiveDecoration;
  specEffects: SpecEffect[];
  height: number;
  owner: Player;
  unbearable: boolean;
}
