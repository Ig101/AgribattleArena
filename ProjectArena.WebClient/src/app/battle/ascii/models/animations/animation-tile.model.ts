import { Color } from 'src/app/shared/models/color.model';

export interface AnimationTile {
  x: number;
  y: number;
  char: string;
  color: Color;
  unitAlpha: boolean;
  unitColorMultiplier: number;
  overflowHealth: boolean;
  priority: number;
  ignoreHeight: boolean;
  workingOnSpecEffects: boolean;
}
