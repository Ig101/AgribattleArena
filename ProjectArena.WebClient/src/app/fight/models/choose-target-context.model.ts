import { ModalPositioning } from './modal-positioning.model';
import { Observer } from 'rxjs';
import { Actor } from 'src/app/engine/scene/actor.object';
import { IActor } from 'src/app/engine/interfaces/actor.interface';

export interface ChooseTargetContext {
  modalPosition: ModalPositioning;
  selectionsObserver: Observer<Actor>;
  actors: Actor[];
}
