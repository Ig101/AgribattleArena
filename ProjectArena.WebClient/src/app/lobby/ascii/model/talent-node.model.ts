import { CharacterClassEnum } from './enums/character-class.enum';

export interface TalentNode {
  x: number;
  y: number;
  id: string;
  name: string;
  description: string;
  class: CharacterClassEnum;
  classPoints: number;
  strength: number;
  willpower: number;
  constitution: number;
  speed: number;
  exceptions: string[];
}
