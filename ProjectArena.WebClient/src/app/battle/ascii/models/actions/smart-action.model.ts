import { SmartActionTypeEnum } from '../enum/smart-action-type.enum';
import { AsciiBattleComponent } from '../../ascii-battle/ascii-battle.component';

export interface SmartAction {
  hotKey: string;
  keyVisualization: string;
  type: SmartActionTypeEnum;
  title: string;
  smartValue: number;
  pressed: boolean;
  smartObject?: any;
  actions: (() => void)[];
  disabled: string;
}
