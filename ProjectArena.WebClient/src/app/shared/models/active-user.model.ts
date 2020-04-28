import { User } from './user.model';
import { UserStateEnum } from './enum/user-state.enum';

export interface ActiveUser extends User {
  id: string;
  email: string;
  state: UserStateEnum;
}
