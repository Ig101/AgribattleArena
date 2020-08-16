import { Action } from 'src/app/engine/models/abstract/action.model';
import { ModalPositioning } from './modal-positioning.model';

export interface ContextMenuContext {
  positioning: ModalPositioning;
  actions: { action: Action, error: string }[];
}
