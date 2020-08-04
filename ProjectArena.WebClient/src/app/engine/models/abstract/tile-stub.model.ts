import { Color } from 'src/app/shared/models/color.model';

export interface TileStub {
  height: number;
  x: number;
  y: number;
  char: string;
  actionColored: boolean;
  color: Color;
  backgroundColor: Color;
}
