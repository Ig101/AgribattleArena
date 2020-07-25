import { Color } from 'src/app/shared/models/color.model';
import { Actor } from '../scene/actor.model';

export interface ModalObject {
  id: number;
  char: string;
  color: Color;
  colorString: string;
  name: string;
  description: string;
  health: { current: number, max: number };
  actor?: Actor;
  anotherObjects: ModalObject[];
}
