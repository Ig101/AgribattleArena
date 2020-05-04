import { ComponentType } from '@angular/cdk/portal';
import { Observable } from 'rxjs';

export interface IModal<Tout> {
  onClose: Observable<Tout>;
  onCancel: Observable<Tout>;

  close();
}
