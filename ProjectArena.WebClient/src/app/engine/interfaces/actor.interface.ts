import { Player } from '../scene/abstract/player.object';
import { Buff } from '../models/abstract/buff.model';
import { Actor } from '../scene/actor.object';

export interface IActor {
  readonly id: number;

  readonly tags: string[];
  readonly isRoot: boolean;

  readonly x: number;
  readonly y: number;
  readonly z: number;

  readonly durability: number;
  readonly volume: number;
  readonly freeVolume: number;
  readonly isAlive: boolean;

  owner: Player;
  actors: Actor[];

  handleEffects(effects: string[], power: number, containerized: boolean, order: number, startingTime: number, issuer: Actor): number;

  applyBuff(buff: Buff);

  purgeBuffs();

  kill();

  changeDurability(durability: number);

  removeActor(actor: Actor);

  addActorOnTop(actor: Actor);

  addActor(actor: Actor, index: number);

  getActorZ(actor: Actor);

  move(target: IActor);

  getChildrenByScope(height: number, scope: number): Actor[];

  getNeighboursByScope(scope: number): IActor[];
}
