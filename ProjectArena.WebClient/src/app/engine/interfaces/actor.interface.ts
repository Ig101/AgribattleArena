import { Player } from '../scene/abstract/player.object';

export interface IActor {
  id: number;

  x: number;
  y: number;

  isAlive: boolean;
  owner: Player;
}
