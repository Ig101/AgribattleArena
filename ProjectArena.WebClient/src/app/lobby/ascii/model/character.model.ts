import { Point } from 'src/app/shared/models/point.model';

export interface Character {
  id: string;
  name: string;
  nativeId: string;
  talents: Point[];
}
