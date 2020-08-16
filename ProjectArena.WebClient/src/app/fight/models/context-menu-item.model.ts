import { ContextMenuSystemTypesEnum } from './enums/context-menu-system-types.enum';
import { Action } from 'src/app/engine/models/abstract/action.model';

export interface ContextMenuItem {
  systemType?: ContextMenuSystemTypesEnum;
  action: Action;
  character: string;
  name: string;
  notAvailable: boolean;
  notAvailableReason?: string;
}
