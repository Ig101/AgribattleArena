import { ActionSquareTypeEnum } from '../enum/action-square-type.enum';

export interface ActionSquare {
  x: number;
  y: number;
  remainedPoints: number;
  remainedSteps: number;
  parentSquares: ActionSquare[];
  type: ActionSquareTypeEnum;
  isActor: boolean;
  topSquare?: boolean;
  leftSquare?: boolean;
  bottomSquare?: boolean;
  rightSquare?: boolean;
}
