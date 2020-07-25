import { Component, OnInit, OnDestroy, Inject, HostListener } from '@angular/core';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { Subject } from 'rxjs';
import { MODAL_DATA } from 'src/app/shared/services/modal.service';
import { ActiveDecoration } from '../../models/scene/active-decoration.model';
import { OverlayRef } from '@angular/cdk/overlay';
import { ModalObject } from '../../models/modals/modal-object.model';
import { IconDefinition } from 'src/app/shared/models/icon-definition.model';
import { CharsService } from 'src/app/shared/services/chars.service';

@Component({
  selector: 'app-scene-object-modal',
  templateUrl: './scene-object-modal.component.html',
  styleUrls: ['./scene-object-modal.component.scss']
})
export class SceneObjectModalComponent implements IModal<ModalObject>, OnDestroy {

  onClose = new Subject<ModalObject>();
  onCancel = new Subject<ModalObject>();

  definition: IconDefinition;
  color: string;
  name: string;
  description: string;
  health: { current: number, max: number };
  tabs: ModalObject[];

  constructor(
    @Inject(MODAL_DATA) data: ModalObject,
    private overlay: OverlayRef,
    public charsService: CharsService
  ) {
    this.definition = {
      char: data.char,
      color: data.color
    };
    this.color = data.colorString;
    this.name = data.name;
    this.description = data.description;
    this.health = data.health;
    if (this.health) {
      this.health.current = Math.ceil(this.health.current);
    }
    this.tabs = data.anotherObjects;
  }

  ngOnDestroy() {
    this.onClose.unsubscribe();
    this.onCancel.unsubscribe();
  }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event) {
    event.preventDefault();
  }

  @HostListener('mouseup', ['$event'])
  onClick(event) {
    if (event.button === 0) {
      event.stopPropagation();
    }
    this.close();
  }

  @HostListener('keypress', ['$event'])
  onKey(event) {
    this.close();
  }

  changeTab(item: ModalObject) {
    this.onClose.next(item);
    this.overlay.detach();
    this.overlay.dispose();
  }

  close() {
    this.onClose.next(undefined);
    this.overlay.detach();
    this.overlay.dispose();
  }
}
