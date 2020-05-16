import { User } from './user.model';
import { UserStateEnum } from './enum/user-state.enum';
import { CharacterForSale } from 'src/app/lobby/ascii/model/character-for-sale.model';
import { Character } from 'src/app/lobby/ascii/model/character.model';
import { TalentNode } from 'src/app/lobby/ascii/model/talent-node.model';

export interface ActiveUser extends User {
  id: string;
  email: string;
  state: UserStateEnum;
  experience: number;
  tavern: CharacterForSale[];
  talentsMap: TalentNode[];
}
