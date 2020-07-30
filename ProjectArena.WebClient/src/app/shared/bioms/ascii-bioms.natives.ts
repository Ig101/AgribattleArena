import { Color } from '../models/color.model';

export const asciiBioms: {[biom: number]: {char: string, color: Color, backgroundColor: Color, probability: number}[] } = {
  0: [
    {
      char: 'grass',
      color: { r: 89, g: 114, b: 255, a: 1 },
      backgroundColor: { r: 16, g: 22, b: 50 },
      probability: 60
    },
    {
      char: 'rock',
      color: { r: 72, g: 77, b: 123, a: 1 },
      backgroundColor: { r: 16, g: 22, b: 50 },
      probability: 3
    },
    {
      char: 'tree',
      color: { r: 160, g: 0, b: 200, a: 1 },
      backgroundColor: { r: 16, g: 22, b: 50 },
      probability: 10
    }
  ]
};
