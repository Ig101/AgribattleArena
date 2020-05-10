import { Component, OnInit, AfterViewInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { LoadingService } from 'src/app/shared/services/loading.service';
import { Subscription } from 'rxjs';
import { Random } from 'src/app/shared/random/random';
import { AsciiLobbyStorageService } from '../services/ascii-lobby-storage.service';
import { getHashFromString } from 'src/app/helpers/extensions/hash.extension';
import { UserService } from 'src/app/shared/services/user.service';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';
import { ComponentSizeEnum } from 'src/app/shared/models/enum/component-size.enum';
import { ModalService } from 'src/app/shared/services/modal.service';
import { UserStateEnum } from 'src/app/shared/models/enum/user-state.enum';
import { Router } from '@angular/router';
import { QueueService } from '../../services/queue.service';
import { SettingsModalComponent } from '../modals/settings-modal/settings-modal.component';
import { LobbyTile } from '../model/lobby-tile.model';
import { Character } from '../model/character.model';

@Component({
  selector: 'app-ascii-lobby',
  templateUrl: './ascii-lobby.component.html',
  styleUrls: ['./ascii-lobby.component.scss']
})
export class AsciiLobbyComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('lobbyCanvas', { static: true }) battleCanvas: ElementRef<HTMLCanvasElement>;
  private canvasContext: CanvasRenderingContext2D;

  tiles: LobbyTile<Character>

  finishLoadingSubscription: Subscription;
  onCloseSubscription: Subscription;
  hubBattleSubscription: Subscription;

  readonly defaultWidth = 1480;
  readonly defaultHeight = 1080;
  readonly defaultAspectRatio = this.defaultWidth / this.defaultHeight;

  tileHeight = 60;
  tileWidth = 0;

  zoom = 0;

  private updateFrequency = 30;
  private drawingTimer;
  changed = false;

  componentSizeEnum = ComponentSizeEnum;

  get userName() {
    return this.userService.user?.name;
  }

  get canvasWidth() {
    return this.battleCanvas.nativeElement.width;
  }

  get canvasHeight() {
    return this.battleCanvas.nativeElement.height;
  }

  get inQueue() {
    return this.queueService.inQueue;
  }

  get queueTimePassed() {
    return this.queueService.timePassed;
  }

  get exitingFromQueue() {
    return this.queueService.exiting;
  }

  constructor(
    private loadingService: LoadingService,
    private lobbyStorageService: AsciiLobbyStorageService,
    private userService: UserService,
    private arenaHub: ArenaHubService,
    private modalService: ModalService,
    private router: Router,
    private queueService: QueueService
  ) {
    this.onCloseSubscription = arenaHub.onClose.subscribe(() => {
      this.loadingService.startLoading({
        title: 'Server connection is lost. Refresh the page or try again later...'
      }, 0, true);
      console.error('Hub connection is lost');
    });
    this.hubBattleSubscription = this.arenaHub.prepareForBattleNotifier.subscribe((value) => {
      if (value) {
        this.userService.user.state = UserStateEnum.Battle;
        this.loadingService.startLoading({
          title: 'Encountered enemy. Prepare for battle!',
          loadingScene: value
        }, 3000).subscribe(() => {
          this.queueService.inQueue = false;
          this.router.navigate(['battle']);
        });
      }
    });
  }

  ngOnInit(): void {
    this.lobbyStorageService.userHash = getHashFromString(this.userService.user.id);
    const userRandom = new Random(this.lobbyStorageService.userHash);
    this.tileWidth = this.tileHeight * 0.6;
    this.setupAspectRatio(this.battleCanvas.nativeElement.offsetWidth, this.battleCanvas.nativeElement.offsetHeight);
    this.canvasContext = this.battleCanvas.nativeElement.getContext('2d');
    // TODO Roster generation
    this.changed = true;
    this.drawingTimer = setInterval(() => {
      this.redrawScene();
    }, 1000 / this.updateFrequency);
  }

  ngOnDestroy(): void {
    this.lobbyStorageService.clear();
    clearInterval(this.drawingTimer);
    this.queueService.dequeue(true);
    this.onCloseSubscription.unsubscribe();
  }

  ngAfterViewInit(): void {
    this.finishLoadingSubscription = this.loadingService.finishLoading()
    .subscribe(() => { this.finishLoadingSubscription.unsubscribe(); });
  }

  setupAspectRatio(width: number, height: number) {
    const newAspectRatio = width / height;
    if (newAspectRatio < this.defaultAspectRatio) {
      const oldWidth = this.defaultWidth;
      this.battleCanvas.nativeElement.width = oldWidth;
      this.battleCanvas.nativeElement.height = oldWidth / newAspectRatio;
    } else {
      const oldHeight = this.defaultHeight;
      this.battleCanvas.nativeElement.width = oldHeight * newAspectRatio;
      this.battleCanvas.nativeElement.height = oldHeight;
    }
    this.zoom = this.battleCanvas.nativeElement.offsetWidth / this.canvasWidth;
    this.changed = true;
    this.redrawScene();
  }

  openSettings() {
    this.modalService.openModalWithoutArgs(SettingsModalComponent);
  }

  patrol() {
    this.queueService.enqueue();
  }

  cancelPatrol() {
    this.queueService.dequeue();
  }

  toTavern() {

  }

  private redrawScene() {
    if (!this.changed) {
      return;
    }
    this.changed = false;
    /*const scene = this.battleStorageService.scene;
    if (scene) {
      const cameraLeft = this.battleStorageService.cameraX - this.canvasWidth / 2 / this.tileWidth;
      const cameraTop = this.battleStorageService.cameraY - this.canvasHeight / 2 / this.tileHeight;
      this.canvasContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
      this.canvasContext.font = `${this.tileHeight}px PT Mono`;
      this.canvasContext.textAlign = 'left';
      const left = Math.max(0, Math.floor(cameraLeft));
      const right = Math.min(scene.width - 1, Math.ceil(cameraLeft + this.canvasWidth / (this.tileWidth)));
      const top = Math.max(0, Math.floor(cameraTop));
      const bottom = Math.min(scene.height - 1, Math.ceil(cameraTop + this.canvasHeight / (this.tileHeight)));

      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);
      const currentActionSquare = this.canAct ? this.battleStorageService.availableActionSquares
        ?.find(s => s.x === mouseX && s.y === mouseY && s.type !== ActionSquareTypeEnum.Actor) : undefined;
      for (let x = left; x <= right; x++) {
        for (let y = top; y <= bottom; y++ ) {
          let drawChar;
          if (currentActionSquare?.x === x && currentActionSquare?.y === y) {
            drawChar = {
              char: 'x',
              color: currentActionSquare.type === ActionSquareTypeEnum.Act ? {r: 255, g: 0, b: 0, a: 1} : {r: 255, g: 255, b: 0, a: 1}
            };
          } else if (currentActionSquare?.type === ActionSquareTypeEnum.Act && currentActionSquare.parentSquares.length > 0 &&
            currentActionSquare?.parentSquares[0].x === x && currentActionSquare?.parentSquares[0].y === y &&
            currentActionSquare?.parentSquares[0].type !== ActionSquareTypeEnum.Actor) {
            drawChar = {
              char: 'x',
              color: {r: 255, g: 255, b: 0, a: 1}
            };
          } else if (currentActionSquare?.parentSquares.some(s => s.x === x && s.y === y && s.type !== ActionSquareTypeEnum.Actor)) {
            drawChar = {
              char: '.',
              color: {r: 255, g: 255, b: 0, a: 1}
            };
          }
          this.drawPoint(scene, x, y, cameraLeft, cameraTop, drawChar);
        }
      }
    }*/
  }
}
