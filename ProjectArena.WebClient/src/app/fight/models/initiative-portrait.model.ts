import { IconDefinition } from 'src/app/shared/models/icon-definition.model';
import { Actor } from 'src/app/engine/scene/actor.object';

export interface InitiativePortrait {
  color: string;
  actor: Actor;
  definition: IconDefinition;
}
