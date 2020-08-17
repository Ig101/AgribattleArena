import { Actor } from '../../scene/actor.object';
import { Observable, Observer } from 'rxjs';
import { IActor } from '../../interfaces/actor.interface';
import { ChangeDefinition } from './change-definition.model';
import { ActionClassEnum } from '../enums/action-class.enum';
import { ActionNative } from 'src/app/content/models/action-native.model';

export interface Action {
  id: string;
  native: ActionNative;
  remainedTime: number;
}
