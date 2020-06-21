import { Component, OnInit, HostListener, OnDestroy, Inject } from '@angular/core';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { Subject, Subscription } from 'rxjs';
import { ComponentSizeEnum } from 'src/app/shared/models/enum/component-size.enum';
import { MODAL_DATA } from 'src/app/shared/services/modal.service';
import { EndGameDeclaration } from '../../models/modals/end-game-declaration.model';
import { OverlayRef } from '@angular/cdk/overlay';
import { Router } from '@angular/router';
import { LoadingService } from 'src/app/shared/services/loading.service';
import { UserService } from 'src/app/shared/services/user.service';
import { AsciiBattleStorageService } from '../../services/ascii-battle-storage.service';
import { BattlePlayerStatusEnum } from 'src/app/shared/models/enum/player-battle-status.enum';
import { WebCommunicationService } from 'src/app/shared/services/web-communication.service';
import { UserStateEnum } from 'src/app/shared/models/enum/user-state.enum';

@Component({
  selector: 'app-menu-modal',
  templateUrl: './menu-modal.component.html',
  styleUrls: ['./menu-modal.component.scss']
})
export class MenuModalComponent implements OnInit, OnDestroy, IModal<any> {

  onClose = new Subject<any>();
  onCancel = new Subject<any>();

  loading: boolean;
  errors: string[];

  loadingSubscription: Subscription;

  componentSizeEnum = ComponentSizeEnum;

  get isPlaying() {
    return this.battleStorageService.players.find(x => x.userId === this.userService.user.id).status === BattlePlayerStatusEnum.Playing;
  }

  constructor(
    private overlay: OverlayRef,
    private router: Router,
    private loadingService: LoadingService,
    private userService: UserService,
    private battleStorageService: AsciiBattleStorageService,
    private webCommunicationService: WebCommunicationService
  ) {

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

  leave() {
    this.loading = true;
    this.webCommunicationService.delete(`api/battle/${this.battleStorageService.scene.id}`)
      .subscribe((result) => {
        if (result.success) {
          this.loadingService.startLoading({
            title: 'Loading...'
          }).subscribe(() => {
            this.onClose.next();
            this.onClose.complete();
            this.overlay.detach();
            this.overlay.dispose();
            this.userService.user.state = UserStateEnum.Lobby;
            this.userService.applyReward(this.battleStorageService.reward);
            this.router.navigate(['lobby']);
          });
        } else {
          if (!this.webCommunicationService.desync(result)) {
            this.errors = result.errors;
          }
        }
      });
  }

  closeOnClick(event) {
    if (event.target !== event.currentTarget) {
      return;
    }
    this.close();
  }

  close() {
    if (!this.loading) {
      this.onClose.next();
      this.overlay.detach();
      this.overlay.dispose();
    }
  }
}
