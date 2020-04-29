import { TagSynergy } from 'src/app/shared/models/battle/tag-synergy.model';
import { Visualization } from '../visualization.model';
import { Player } from '../player.model';
import { ActionAnimation } from '../action-animation.model';

export interface ActiveDecoration {
  id: number;

  name: string;
  description: string;
  visualization: Visualization;
  action: ActionAnimation;
  onDeathAction: ActionAnimation;

  mod: number;
  initiativePosition: number;
  health: number;
  owner: Player;
  isAlive: boolean;
  x: number;
  y: number;
  z: number;
  maxHealth: number;
  armor: TagSynergy[];
}
