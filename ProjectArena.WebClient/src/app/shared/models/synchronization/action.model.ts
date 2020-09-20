import { ActionInfo } from './action-info.model';

export interface Action {
  sceneId: string;
  version: number;
  code: string;
  newCode: string;
  action: ActionInfo;
}
