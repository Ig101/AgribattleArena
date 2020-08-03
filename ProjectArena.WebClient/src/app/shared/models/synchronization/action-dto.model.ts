import { ActionInfo } from './action-info.model';

export interface ActionDto {
  version: number;
  code: string;
  newCode: string;
  action: ActionInfo;
}
