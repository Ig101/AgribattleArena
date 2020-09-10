import { ActionInfo } from './action-info.model';

export interface Action {
  version: number;
  code: string;
  newCode: string;
  action: ActionInfo;
}
