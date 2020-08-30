import { Component, OnInit, OnDestroy, Inject, ViewChild, ElementRef, HostListener } from '@angular/core';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { Observable, Subject, Subscription } from 'rxjs';
import { MODAL_DATA } from 'src/app/shared/services/modal.service';
import { ChooseTargetContext } from '../../models/choose-target-context.model';
import { Actor } from 'src/app/engine/scene/actor.object';
import { OverlayRef } from '@angular/cdk/overlay';
import { IActor } from 'src/app/engine/interfaces/actor.interface';

@Component({
  selector: 'app-target-choose-modal',
  templateUrl: './target-choose-modal.component.html',
  styleUrls: ['./target-choose-modal.component.scss']
})
export class TargetChooseModalComponent implements OnInit, OnDestroy, IModal<Actor> {

  onClose = new Subject<Actor>();
  onCancel = new Subject<Actor>();

  updatePositionSub: Subscription;

  @ViewChild('overlay', { static: true }) overlayElement: ElementRef;

  get actors() {
    return this.data.actors;
  }

  left: number;
  top: number;

  constructor(
    @Inject(MODAL_DATA) private data: ChooseTargetContext,
    private overlay: OverlayRef
  ) { }

  ngOnDestroy(): void {
    this.onClose.unsubscribe();
    this.onCancel.unsubscribe();
    this.updatePositionSub.unsubscribe();
  }

  ngOnInit() {
    this.data.selectionsObserver.next(this.data.actors[0]);
    this.updatePosition();
    this.updatePositionSub = this.data.modalPosition.updateSubject.subscribe(_ => {
      this.updatePosition();
    });
  }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event) {
    event.preventDefault();
  }


  updatePosition() {
    this.left = this.data.modalPosition.left + this.data.modalPosition.textHeight * 0.3;
    this.top = Math.max(20, Math.min(this.overlayElement.nativeElement.offsetHeight - 220, this.data.modalPosition.top - 100));
  }

  close() {
    this.data.selectionsObserver.next(undefined);
    this.onCancel.next();
    this.overlay.detach();
    this.overlay.dispose();
  }

  mouseEnter(actor: Actor) {
    this.data.selectionsObserver.next(actor);
  }

  mouseLeave() {
    this.data.selectionsObserver.next(this.data.actors[0]);
  }

  chooseActor(actor: Actor) {
    this.data.selectionsObserver.next(undefined);
    this.onClose.next(actor);
    this.overlay.detach();
    this.overlay.dispose();
  }

  closeOnClick(event) {
    if (event.target !== event.currentTarget) {
      return;
    }
    this.close();
  }
}
