import { Character } from './character.model';
import { Color } from 'src/app/shared/models/color.model';

export interface CharacterListElement {
  character: Character;
  color: string;
}
