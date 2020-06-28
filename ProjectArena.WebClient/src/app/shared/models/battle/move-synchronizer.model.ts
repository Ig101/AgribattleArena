import { MoveInfo } from './move-info.model';

export interface MoveSynchronizer {
  id: string;
  moves: MoveInfo[];
}
