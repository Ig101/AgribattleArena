import { Color } from '../color.model';

export interface LoadingTile {
  char: string;
  color: Color;
  backgroundColor: Color;
  bright: boolean;
  height: number;
}
