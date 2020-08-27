import { Actor } from 'src/app/engine/scene/actor.object';
import { ChangeDefinition } from 'src/app/engine/models/abstract/change-definition.model';

export interface ReactionNative {
  respondsOn: string;

  action: (actor: Actor, power: number, mod: number, containerized: boolean,
           order: number, startingTime: number, issuer: Actor)
    => { power: number, changes: ChangeDefinition[] };
}
