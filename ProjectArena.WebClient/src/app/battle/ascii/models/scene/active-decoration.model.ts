import { TagSynergy } from 'src/app/shared/models/battle/tag-synergy.model';
import { Visualization } from '../visualization.model';
import { Player } from '../player.model';

export interface ActivaDecoration {
  id: number;

  name: string;
  description: string;
  visualization: Visualization;
  action: Animation;
  onDeathAction: Animation;

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
