import { Player } from '../scene/abstract/player.object';
import { Effect } from '../content/effects';
import { Buff } from '../scene/abstract/buff.object';

export interface IActor {
  id: number;

  x: number;
  y: number;

  isAlive: boolean;
  owner: Player;

  processEffect(effect: Effect, power: number, order: number);

  applyBuff(buff: Buff);

  purgeBuffs();

  changeDurability(durability: number);
}
