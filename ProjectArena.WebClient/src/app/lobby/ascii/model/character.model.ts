import { Point } from 'src/app/shared/models/point.model';

export interface Character {
  id: string;
  name: string;
  isKeyCharacter: boolean;
  nativeId: string;
  talents: Point[];
}
