import { Character } from 'src/app/lobby/ascii/model/character.model';

export interface User {
  name: string;
  uniqueId: string;
  roster: Character[];
}
