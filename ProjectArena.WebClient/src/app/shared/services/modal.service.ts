import { Injectable, InjectionToken, Injector } from '@angular/core';
import { Overlay, OverlayRef, GlobalPositionStrategy } from '@angular/cdk/overlay';
import { IModal } from '../interfaces/modal.interface';
import { ComponentPortal, ComponentType, PortalInjector } from '@angular/cdk/portal';
import { config } from 'process';
import { IModalConstructor, IModalConstructorWithoutArgs } from '../interfaces/modal-constructor.interface';

export const MODAL_DATA = new InjectionToken<any>('MODAL_DATA');

@Injectable()
export class ModalService {

  constructor(
    private overlay: Overlay,
    private injector: Injector
  ) { }

  private createInjector<T>(overlayRef: OverlayRef, data: T) {
    const injectionTokens = new WeakMap();
    injectionTokens.set(OverlayRef, overlayRef);
    injectionTokens.set(MODAL_DATA, data);
    return new PortalInjector(this.injector, injectionTokens);
  }

  private createInjectorWithoutArgs(overlayRef: OverlayRef) {
    const injectionTokens = new WeakMap();
    injectionTokens.set(OverlayRef, overlayRef);
    return new PortalInjector(this.injector, injectionTokens);
  }

  openModal<T extends IModal<unknown>, D>(component: IModalConstructor<D, unknown, T>, data: D): IModal<unknown> {
    const overlayRef = this.overlay.create({
      positionStrategy: new GlobalPositionStrategy().left('0px').top('0px'),
      panelClass: 'overlay'
    });
    const injector = this.createInjector<D>(overlayRef, data);
    const portal = new ComponentPortal(component, null, injector);
    const containerRef = overlayRef.attach(portal);
    return containerRef.instance;
  }

  openModalWithoutArgs<T extends IModal<unknown>>(component: IModalConstructorWithoutArgs<unknown, T>): IModal<unknown> {
    const overlayRef = this.overlay.create({
      positionStrategy: new GlobalPositionStrategy().left('0px').top('0px'),
      panelClass: 'overlay'
    });
    const injector = this.createInjectorWithoutArgs(overlayRef);
    const portal = new ComponentPortal(component, null, injector);
    const containerRef = overlayRef.attach(portal);
    return containerRef.instance;
  }
}
