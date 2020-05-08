import { Component, OnInit, Inject, HostListener, OnDestroy } from '@angular/core';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { Observable, Subscribable, Subscription, Subject } from 'rxjs';
import { MODAL_DATA } from 'src/app/shared/services/modal.service';
import { EndGameDeclaration } from '../../models/modals/end-game-declaration.model';
import { OverlayRef } from '@angular/cdk/overlay';
import { Router } from '@angular/router';
import { LoadingService } from 'src/app/shared/services/loading.service';
import { UserService } from 'src/app/shared/services/user.service';
import { UserStateEnum } from 'src/app/shared/models/enum/user-state.enum';

@Component({
  selector: 'app-victory-modal',
  templateUrl: './victory-modal.component.html',
  styleUrls: ['./victory-modal.component.scss']
})
export class VictoryModalComponent implements OnInit, OnDestroy, IModal<any> {

  onClose = new Subject<any>();
  onCancel = new Subject<any>();

  killed = false;

  loadingSubscription: Subscription;

  victory: boolean;

  constructor(
    @Inject(MODAL_DATA) data: EndGameDeclaration,
    private overlay: OverlayRef,
    private router: Router,
    private loadingService: LoadingService,
    private userService: UserService
  ) {
    this.victory = data.victory;
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.onClose.unsubscribe();
    this.onCancel.unsubscribe();
    if (this.loadingSubscription) {
      this.loadingSubscription.unsubscribe();
    }
  }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event) {
    event.preventDefault();
  }

  @HostListener('click', ['$event'])
  onClick(event) {
    if (!this.killed) {
      this.onAction();
    }
  }

  onAction() {
    this.killed = true;
    this.loadingSubscription = this.loadingService.startLoading({
      title: 'Returning...'
    }).subscribe(() => {
      this.onClose.next();
      this.onClose.complete();
      this.overlay.detach();
      this.overlay.dispose();
      this.userService.user.state = UserStateEnum.Lobby;
      this.router.navigate(['auth']);
    });
  }

  close() { }
}
