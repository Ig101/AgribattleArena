import { Color } from 'src/app/shared/models/color.model';
import { DEFAULT_HEIGHT } from 'src/app/content/content.helper';

export function brightImpact(bright: boolean, color: Color) {
  if (bright) {
    return {
      r: color.r * 0.2,
      g: color.g * 0.2,
      b: color.b * 0.2,
      a: color.a
    };
  }
  return color;
}

export function heightImpact(z: number, color: Color): Color {
  if (z !== 0) {
    return {
      r: Math.min(255, Math.max(0, color.r * (1 + (z - DEFAULT_HEIGHT) / 600))),
      g: Math.min(255, Math.max(0, color.g * (1 + (z - DEFAULT_HEIGHT) / 600))),
      b: Math.min(255, Math.max(0, color.b * (1 + (z - DEFAULT_HEIGHT) / 600))),
      a: color.a
    };
  }
  return color;
}
