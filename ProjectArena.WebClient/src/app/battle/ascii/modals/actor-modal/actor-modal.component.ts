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

@Component({
  selector: 'app-actor-modal',
  templateUrl: './actor-modal.component.html',
  styleUrls: ['./actor-modal.component.scss']
})
export class ActorModalComponent implements IModal<any>, OnDestroy {

  onClose = new Subject<any>();
  onCancel = new Subject<any>();

  actorChar: string;
  actorColor: string;
  actorName: string;
  actorHealth: number;
  actorMaxHealth: number;
  actorDescription: string;
  buffs: {char: string, color: string, description: string, duration: number}[];

  constructor(
    @Inject(MODAL_DATA) data: Actor,
    private overlay: OverlayRef
  ) {
    this.actorChar = data.defaultVisualization.char;
    this.actorColor = `rgba(${data.defaultVisualization.color.r},${data.defaultVisualization.color.g},
      ${data.defaultVisualization.color.b},${data.defaultVisualization.color.a})`;
    this.actorName = data.name;
    this.actorDescription = data.description;
    this.actorHealth = Math.ceil(data.health);
    this.actorMaxHealth = data.maxHealth;
    this.buffs = data.buffs.map(buff => {
      return {
        char: buff.char,
        color: `rgba(${buff.color.r},${buff.color.g},
          ${buff.color.b},1)`,
        description: buff.description,
        duration: Math.round(buff.duration * 100) / 100
      };
    });
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
    this.overlay.dispose();
  }
}
