import { Component, OnInit, AfterViewInit, OnDestroy, ViewChild, ElementRef, HostListener } from '@angular/core';
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
import { LobbyTileActivator } from '../model/lobby-tile-activator.model';
import { asciiBioms } from 'src/app/shared/bioms/ascii-bioms.natives';
import { BiomEnum } from 'src/app/shared/models/enum/biom.enum';
import { actorNatives } from 'src/app/battle/ascii/natives';
import { MouseState } from 'src/app/shared/models/mouse-state.model';
import { rangeBetween, rangeBetweenShift } from 'src/app/helpers/math.helper';
import { Color } from 'src/app/shared/models/color.model';
import { getRandomBiom } from 'src/app/shared/bioms/biom.helper';
import { TavernModalComponent } from '../modals/tavern-modal/tavern-modal.component';

@Component({
  selector: 'app-ascii-lobby',
  templateUrl: './ascii-lobby.component.html',
  styleUrls: ['./ascii-lobby.component.scss']
})
export class AsciiLobbyComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('lobbyCanvas', { static: true }) lobbyCanvas: ElementRef<HTMLCanvasElement>;
  private canvasContext: CanvasRenderingContext2D;

  activators: LobbyTileActivator<Character>[];
  tiles: LobbyTile<Character>[][];
  cursor: string;

  finishLoadingSubscription: Subscription;
  onCloseSubscription: Subscription;
  hubBattleSubscription: Subscription;
  userChangedSubscription: Subscription;

  readonly defaultWidth = 1504;
  readonly defaultHeight = 1080;
  readonly defaultAspectRatio = this.defaultWidth / this.defaultHeight;

  tileHeight = 60;
  tileWidth = 0;
  campWidth = 27;
  campHeight = 15;
  campBiom = BiomEnum.Grass;
  charactersMaxCount = 6;

  zoom = 0;

  private updateFrequency = 30;
  private drawingTimer;
  changed = false;

  fullParty = false;

  componentSizeEnum = ComponentSizeEnum;

  mouseState: MouseState = {
    buttonsInfo: {},
    x: -1,
    y: -1,
    realX: -1,
    realY: -1
  };

  ground = {
    char: '·',
    color: {r: 225, g: 169, b: 95, a: 1} as Color,
    backgroundColor: {r: 30, g: 23, b: 13, a: 1} as Color
  };

  get userName() {
    return this.userService.user?.name;
  }

  get canvasWidth() {
    return this.lobbyCanvas.nativeElement.width;
  }

  get canvasHeight() {
    return this.lobbyCanvas.nativeElement.height;
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
    this.userChangedSubscription = this.userService.userChanged.subscribe(() => this.onUpdate() );
  }

  generateCamp() {
    const userRandom = new Random(this.lobbyStorageService.userHash);
    const biom = asciiBioms[this.campBiom];
    this.activators = new Array<LobbyTileActivator<Character>>(this.charactersMaxCount);
    for (let i = 0; i < this.charactersMaxCount; i++) {
      this.activators[i] = {
        xShift: 3,
        yShift: 1,
        object: this.userService.user.roster[i],
        x: Math.floor(this.campWidth / 2) + (i % 3 === 1 ? 7 : 4) * (i >= 3 ? 1 : -1),
        y: Math.floor(this.campHeight / 2) - 4 + ((i % 3) * 4)
      };
    }
    this.tiles = new Array<LobbyTile<Character>[]>(this.campWidth);
    for (let x = 0; x < this.campWidth; x++) {
      this.tiles[x] = new Array<LobbyTile<Character>>(this.campHeight);
      for (let y = 0; y < this.campHeight; y++) {
        const activator = this.activators.find(a =>
          a.x - a.xShift <= x &&
          a.x + a.xShift >= x &&
          a.y - a.yShift  <= y &&
          a.y >= y);
        const native = activator &&
          activator.object &&
          activator.x === x &&
          activator.y === y ?
          actorNatives[activator.object.nativeId] : undefined;
        const range = rangeBetweenShift(this.campWidth / 2 - 1 - x, this.campWidth / 2 - 1 - y * this.campWidth / this.campHeight);
        let tile = this.ground;
        if (range > this.campWidth / 2 - 4) {
          const probability = range - this.campWidth / 2 + 4;
          if (userRandom.nextDouble() * 4 < probability) {
            if ( range <= this.campWidth / 2 - 1) {
              tile = biom[0];
            } else {
              tile = getRandomBiom(userRandom, this.campBiom);
            }
          }
        }
        this.tiles[x][y] = {
          activator,
          char: native ? native.visualization.char : tile.char,
          color: native ? native.visualization.color : tile.color,
          backgroundColor: tile.backgroundColor
        };
      }
    }
  }

  ngOnInit(): void {
    this.lobbyStorageService.userHash = getHashFromString(this.userService.user.id);
    this.tileWidth = this.tileHeight * 0.6;
    this.setupAspectRatio(this.lobbyCanvas.nativeElement.offsetWidth, this.lobbyCanvas.nativeElement.offsetHeight);
    this.canvasContext = this.lobbyCanvas.nativeElement.getContext('2d');
    this.generateCamp();
    this.fullParty = this.userService.user.roster.length >= 6;
    this.changed = true;
    this.redrawScene();
    this.drawingTimer = setInterval(() => {
      this.redrawScene();
    }, 1000 / this.updateFrequency);
  }

  ngOnDestroy(): void {
    this.lobbyStorageService.clear();
    clearInterval(this.drawingTimer);
    this.queueService.dequeue(true);
    this.onCloseSubscription.unsubscribe();
    this.userChangedSubscription.unsubscribe();
  }

  ngAfterViewInit(): void {
    this.finishLoadingSubscription = this.loadingService.finishLoading()
    .subscribe(() => { this.finishLoadingSubscription.unsubscribe(); });
  }

  onUpdate() {
    for (const activator of this.activators) {
      activator.object = undefined;
      this.tiles[activator.x][activator.y].char = this.ground.char;
      this.tiles[activator.x][activator.y].color = this.ground.color;
    }
    for (let i = 0; i < this.userService.user.roster.length; i++) {
      const activator = this.activators[i];
      activator.object = this.userService.user.roster[i];
      const native = actorNatives[this.userService.user.roster[i].nativeId];
      this.tiles[activator.x][activator.y].char = native.visualization.char;
      this.tiles[activator.x][activator.y].color = native.visualization.color;
    }
    this.fullParty = this.userService.user.roster.length >= 6;
    this.changed = true;
  }

  onResize() {
    this.setupAspectRatio(this.lobbyCanvas.nativeElement.offsetWidth, this.lobbyCanvas.nativeElement.offsetHeight);
  }

  onMouseLeave() {
    for (const state of Object.values(this.mouseState.buttonsInfo)) {
      state.pressed = false;
      state.timeStamp = 0;
    }
    this.cursor = undefined;
    this.mouseState.realX = undefined;
    this.mouseState.realY = undefined;
  }

  onMouseUp(event: MouseEvent) {
    const x = Math.floor(this.mouseState.x);
    const y = Math.floor(this.mouseState.y);
    this.mouseState.buttonsInfo[event.button] = {pressed: false, timeStamp: 0};
    this.recalculateMouseMove(event.x, event.y, event.timeStamp);
    const newX = Math.floor(this.mouseState.x);
    const newY = Math.floor(this.mouseState.y);
    if (event.button === 0 && x >= 0 && x < this.campWidth && y >= 0 && y < this.campHeight) {
      const activator = this.tiles[x][y].activator;
      const newActivator = this.tiles[newX][newY].activator;
      if (activator && activator.object && activator === newActivator) {
        console.log(activator);
      }
    }
  }

  onMouseUpWindow(event: MouseEvent) {
    setTimeout(() => {
      this.mouseState.buttonsInfo[event.button] = {pressed: false, timeStamp: 0};
      this.changed = true;
      this.recalculateMouseMove(event.x, event.y, event.timeStamp);
    });
  }

  onMouseDown(event: MouseEvent) {
    this.changed = true;
    this.mouseState.buttonsInfo[event.button] = {pressed: true, timeStamp: 0};
  }

  onMouseMove(event: MouseEvent) {
    this.mouseState.realX = event.x;
    this.mouseState.realY = event.y;
    this.recalculateMouseMove(event.x, event.y, event.timeStamp);
  }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event) {
    event.preventDefault();
  }

  private recalculateMouseMove(x: number, y: number, timeStamp?: number) {
    const leftKey = this.mouseState.buttonsInfo[0];
    const rightKey = this.mouseState.buttonsInfo[2];
    const cameraLeft = this.campWidth / 2 - (this.canvasWidth - 374) / 2 / this.tileWidth + 0.5;
    const cameraTop = this.campHeight / 2 - this.canvasHeight / 2 / this.tileHeight + 0.5;
    const newX = x / this.zoom / this.tileWidth + cameraLeft;
    const newY = y / this.zoom / this.tileHeight + cameraTop;
    const mouseX = Math.floor(newX);
    const mouseY = Math.floor(newY);
    this.cursor = undefined;
    if (this.tiles && mouseX >= 0 && mouseX < this.campWidth && mouseY >= 0 && mouseY < this.campHeight) {
      if (this.tiles[mouseX][mouseY].activator && this.tiles[mouseX][mouseY].activator.object) {
        this.cursor = 'pointer';
      }
    }
    if (!rightKey?.pressed && !leftKey?.pressed) {
      this.mouseState.x = newX;
      this.mouseState.y = newY;
      this.changed = true;
    }
  }

  private setupAspectRatio(width: number, height: number) {
    const newAspectRatio = width / height;
    if (newAspectRatio < this.defaultAspectRatio) {
      const oldWidth = this.defaultWidth;
      this.lobbyCanvas.nativeElement.width = oldWidth;
      this.lobbyCanvas.nativeElement.height = oldWidth / newAspectRatio;
    } else {
      const oldHeight = this.defaultHeight;
      this.lobbyCanvas.nativeElement.width = oldHeight * newAspectRatio;
      this.lobbyCanvas.nativeElement.height = oldHeight;
    }
    this.zoom = this.lobbyCanvas.nativeElement.offsetWidth / this.canvasWidth;
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
    this.modalService.openModalWithoutArgs(TavernModalComponent);
  }

  private drawPoint(
    tile: LobbyTile<Character>,
    x: number, y: number,
    cameraLeft: number,
    cameraTop: number,
    active: boolean,
    clicked: boolean) {
    if (tile) {
      const canvasX = (x - cameraLeft) * this.tileWidth;
      const canvasY = (y - cameraTop) * this.tileHeight;
      const symbolY = canvasY + this.tileHeight * 0.75;
      if (tile.backgroundColor) {
        this.canvasContext.fillStyle = `rgb(${tile.backgroundColor.r}, ${tile.backgroundColor.g}, ${tile.backgroundColor.b})`;
        this.canvasContext.fillRect(canvasX, canvasY, this.tileWidth + 1, this.tileHeight + 1);
      }
      this.canvasContext.fillStyle = active ? (clicked ? `rgba(170, 170, 0, ${tile.color.a})` : `rgba(255, 255, 68, ${tile.color.a})`) :
        `rgba(${tile.color.r}, ${tile.color.g}, ${tile.color.b}, ${tile.color.a})`;
      this.canvasContext.fillText(tile.char, canvasX, symbolY);
    }
  }


  private redrawScene() {
    if (!this.changed) {
      return;
    }
    this.changed = false;
    const userRandom = new Random(this.lobbyStorageService.userHash);
    if (this.tiles) {
      const cameraLeft = this.campWidth / 2 - (this.canvasWidth - 374) / 2 / this.tileWidth + 0.5;
      const cameraTop = this.campHeight / 2 - this.canvasHeight / 2 / this.tileHeight + 0.8;
      this.canvasContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
      this.canvasContext.font = `${this.tileHeight}px PT Mono`;
      this.canvasContext.textAlign = 'left';
      const left = Math.floor(cameraLeft) - this.tileWidth;
      const right = Math.ceil(cameraLeft + this.canvasWidth / (this.tileWidth)) + this.tileWidth;
      const top = Math.floor(cameraTop) - this.tileHeight;
      const bottom = Math.ceil(cameraTop + this.canvasHeight / (this.tileHeight)) + this.tileHeight;
      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);
      const clicked = this.mouseState.buttonsInfo[0] && this.mouseState.buttonsInfo[0].pressed;
      for (let x = -40; x <= 80; x++) {
        for (let y = -20; y <= 60; y++) {
          let tile: LobbyTile<Character>;
          if (x >= 0 && y >= 0 && x < this.campWidth && y < this.campHeight) {
            tile = this.tiles[x][y];
          } else {
            const biom = getRandomBiom(userRandom, this.campBiom);
            tile = {
              char: biom.char,
              color: biom.color,
              backgroundColor: biom.backgroundColor,
              activator: undefined
            };
          }
          if (x >= left && x <= right && y >= top && y <= bottom) {
            this.drawPoint(tile, x, y, cameraLeft, cameraTop,
              tile.activator &&
              tile.activator.object &&
              tile.activator.x === x &&
              tile.activator.y === y &&
              mouseX >= x - tile.activator.xShift &&
              mouseX <= x + tile.activator.xShift &&
              mouseY >= y - tile.activator.yShift &&
              mouseY <= y,
              clicked);
          }
        }
      }
      for (const activator of this.activators) {
        if (activator.object) {
          const x = (activator.x + 0.5 - cameraLeft) * this.tileWidth;
          const y = (activator.y - cameraTop) * this.tileHeight;
          const activated = mouseX >= activator.x - activator.xShift &&
            mouseX <= activator.x + activator.xShift &&
            mouseY >= activator.y - activator.yShift &&
            mouseY <= activator.y;
          this.canvasContext.font = `${26}px PT Mono`;
          this.canvasContext.textAlign = 'center';
          this.canvasContext.fillStyle = activated ? (clicked ? '#aa0' : '#ff4') : '#ffffff';
          this.canvasContext.fillText(activator.object.name, x, y);
        }
      }
    }
  }
}
