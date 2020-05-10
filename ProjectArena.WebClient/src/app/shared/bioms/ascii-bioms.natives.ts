import { Color } from '../models/color.model';

export const asciiBioms: {[biom: number]: {char: string, color: Color, backgroundColor: Color}[] } = {
  0: [
    {
      char: '-',
      color: { r: 0, g: 180, b: 0, a: 1 },
      backgroundColor: { r: 0, g: 20, b: 0 }
    },
    {
      char: '*',
      color: { r: 0, g: 160, b: 0, a: 1 },
      backgroundColor: { r: 0, g: 20, b: 0 }
    },
    {
      char: 'Y',
      color: { r: 139, g: 69, b: 19, a: 1 },
      backgroundColor: { r: 0, g: 20, b: 0 }
    },
    {
      char: 'Y',
      color: { r: 255, g: 165, b: 79, a: 1 },
      backgroundColor: { r: 0, g: 20, b: 0 }
    }
  ]
};
