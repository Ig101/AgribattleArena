import { ComponentType } from '@angular/cdk/portal';
import { OverlayRef } from '@angular/cdk/overlay';
import { IModal } from './modal.interface';

export interface IModalConstructor<Tin, Tout, TComponent extends IModal<Tout>> extends ComponentType<TComponent> {
  new (data: Tin, overlayRef: OverlayRef, ...args: any[]): TComponent;
}
