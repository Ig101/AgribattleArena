import { Observable } from 'rxjs';

export interface ModalPositioning {
  left: number;
  top: number;
  textHeight: number;
  updateSubject: Observable<any>;
}
