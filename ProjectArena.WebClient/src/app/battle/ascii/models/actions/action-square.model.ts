import { ActionSquareTypeEnum } from '../enum/action-square-type.enum';

export interface ActionSquare {
  x: number;
  y: number;
  remainedPoints: number;
  parentSquares: ActionSquare[];
  type: ActionSquareTypeEnum;
  topSquare?: boolean;
  leftSquare?: boolean;
  bottomSquare?: boolean;
  rightSquare?: boolean;
}
