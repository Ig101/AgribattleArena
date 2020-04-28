import { Color } from 'src/app/shared/models/color.model';

export interface Skill {
  id: number;

  name: string;
  description: string;
  action: Animation;

  range: number;
  cd: number;
  mod: number;
  cost: number;
  preparationTime: number;
}
