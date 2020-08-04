import { Color } from 'src/app/shared/models/color.model';

export interface TileStub {
  endTime: number;
  height: number;
  x: number;
  y: number;
  char: string;
  actionColored: boolean;
  color: Color;
  backgroundColor: Color;
}
