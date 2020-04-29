import { Player } from '../player.model';
import { Visualization } from '../visualization.model';
import { ActionAnimation } from '../action-animation.model';

export interface SpecEffect {
  id: number;

  name: string;
  description: string;
  visualization: Visualization;
  action: ActionAnimation;
  onDeathAction: ActionAnimation;

  owner: Player;
  isAlive: boolean;
  x: number;
  y: number;
  z: number;
  duration?: number;
  mod: number;
}
