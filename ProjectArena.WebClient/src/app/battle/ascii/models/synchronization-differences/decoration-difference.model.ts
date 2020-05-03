import { ActiveDecoration } from '../scene/active-decoration.model';

export interface DecorationDifference {
  x: number;
  y: number;
  decoration: ActiveDecoration;
  healthChange: number;
  changedPosition: boolean;
}
