import { Component, OnInit, OnDestroy, Inject, HostListener } from '@angular/core';
import { ActiveDecoration } from '../../models/scene/active-decoration.model';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { Subject } from 'rxjs';
import { MODAL_DATA } from 'src/app/shared/services/modal.service';
import { OverlayRef } from '@angular/cdk/overlay';
import { SpecEffect } from '../../models/scene/spec-effect.model';

@Component({
  selector: 'app-decoration-modal',
  templateUrl: './decoration-modal.component.html',
  styleUrls: ['./decoration-modal.component.scss']
})
export class DecorationModalComponent implements IModal<any>, OnDestroy {

  onClose = new Subject<any>();
  onCancel = new Subject<any>();

  decorationChar: string;
  decorationColor: string;
  decorationName: string;
  decorationDescription: string;
  decorationHealth: number;
  decorationMaxHealth: number;

  constructor(
    @Inject(MODAL_DATA) data: ActiveDecoration,
    private overlay: OverlayRef
  ) {
    this.decorationChar = data.visualization.char;
    this.decorationColor = `rgba(${data.visualization.color.r},${data.visualization.color.g},
      ${data.visualization.color.b},${data.visualization.color.a})`;
    this.decorationName = data.name;
    this.decorationDescription = data.description;
    this.decorationHealth = Math.ceil(data.health);
    this.decorationMaxHealth = data.maxHealth;
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

  close() {
    this.onClose.next();
    this.overlay.detach();
    this.overlay.dispose();
  }
}
