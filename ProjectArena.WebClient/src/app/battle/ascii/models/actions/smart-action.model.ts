import { SmartActionTypeEnum } from '../enum/smart-action-type.enum';

export interface SmartAction {
  hotKey: string;
  needHold: boolean;
  pressedTime: number;
  type: SmartActionTypeEnum;
  position: number;
}
