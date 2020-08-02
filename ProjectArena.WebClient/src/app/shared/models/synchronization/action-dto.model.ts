import { ActionInfo } from './action-info.model';

export interface ActionDto {
  version: number;
  code: string;
  action: ActionInfo;
}
