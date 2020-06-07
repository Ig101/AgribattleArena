import { Component, OnInit, Inject, OnDestroy, HostListener } from '@angular/core';
import { MODAL_DATA } from 'src/app/shared/services/modal.service';
import { EndGameDeclaration } from '../../models/modals/end-game-declaration.model';
import { OverlayRef } from '@angular/cdk/overlay';
import { Router } from '@angular/router';
import { LoadingService } from 'src/app/shared/services/loading.service';
import { UserService } from 'src/app/shared/services/user.service';
import { Actor } from '../../models/scene/actor.model';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { Observable, Subject } from 'rxjs';
import { Color } from 'src/app/shared/models/color.model';
import { ModalObject } from '../../models/modals/modal-object.model';

@Component({
  selector: 'app-actor-modal',
  templateUrl: './actor-modal.component.html',
  styleUrls: ['./actor-modal.component.scss']
})
export class ActorModalComponent implements IModal<ModalObject>, OnDestroy {

  onClose = new Subject<ModalObject>();
  onCancel = new Subject<ModalObject>();

  actorChar: string;
  actorColor: string;
  actorName: string;
  actorHealth: number;
  actorMaxHealth: number;
  actorDescription: string;
  tabs: ModalObject[];
  buffs: {char: string, color: string, description: string, duration: number}[];

  constructor(
    @Inject(MODAL_DATA) data: ModalObject,
    private overlay: OverlayRef
  ) {
    this.actorChar = data.char;
    this.actorColor = data.color;
    this.actorName = data.name;
    this.actorDescription = data.description;
    this.actorHealth = Math.ceil(data.health.current);
    this.actorMaxHealth = data.health.max;
    this.buffs = data.actor.buffs.map(buff => {
      return {
        char: buff.char,
        color: `rgba(${buff.color.r},${buff.color.g},
          ${buff.color.b},1)`,
        description: buff.description,
        duration: Math.round(buff.duration * 100) / 100
      };
    });
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
