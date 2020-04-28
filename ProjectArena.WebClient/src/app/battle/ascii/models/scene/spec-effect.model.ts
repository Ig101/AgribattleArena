import { Player } from '../player.model';
import { Visualization } from '../visualization.model';

export interface SpecEffect {
  id: number;

  name: string;
  description: string;
  visualization: Visualization;
  action: Animation;
  onDeathAction: Animation;

  owner: Player;
  isAlive: boolean;
  x: number;
  y: number;
  z: number;
  duration?: number;
  mod: number;
}
