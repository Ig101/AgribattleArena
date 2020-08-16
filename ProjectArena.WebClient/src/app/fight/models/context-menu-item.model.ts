import { ContextMenuSystemTypesEnum } from './enums/context-menu-system-types.enum';
import { Action } from 'src/app/engine/models/abstract/action.model';

export interface ContextMenuItem {
  systemType?: ContextMenuSystemTypesEnum;
  action: Action;
  character: string;
  name: string;
  left: number;
  top: number;
  notAvailable: boolean;
  notAvailableReason?: string;
  leftTooltip: boolean;
}
