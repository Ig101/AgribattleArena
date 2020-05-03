import { Color } from 'src/app/shared/models/color.model';

export interface FloatingText {
  text: string;
  color: Color;
  time: number;
  x: number;
  y: number;
  height: number;
}
