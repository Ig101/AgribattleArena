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
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { TalentsModalComponent } from '../modals/talents-modal/talents-modal.component';
import { HintDeclaration } from '../model/hint-declaration.model';
import { CharsService } from 'src/app/shared/services/chars.service';
import { AssetsLoadingService } from 'src/app/shared/services/assets-loading.service';
import { fillVertexPosition, fillTileMask, fillBackground, fillColor, fillChar, drawArrays } from 'src/app/helpers/webgl.helper';

@Component({
  selector: 'app-ascii-lobby',
  templateUrl: './ascii-lobby.component.html',
  styleUrls: ['./ascii-lobby.component.scss']
})
export class AsciiLobbyComponent implements OnInit, OnDestroy {

  @ViewChild('lobbyCanvas', { static: true }) lobbyCanvas: ElementRef<HTMLCanvasElement>;
  private canvasWebGLContext: WebGLRenderingContext;
  @ViewChild('hudCanvas', { static: true }) hudCanvas: ElementRef<HTMLCanvasElement>;
  private canvas2DContext: CanvasRenderingContext2D;
  private shadersProgram: WebGLProgram;
  private charsTexture: WebGLTexture;

  private loaded;

  activators: LobbyTileActivator<Character>[];
  tiles: LobbyTile<Character>[][];
  cursor: string;

  finishLoadingSubscription: Subscription;
  onCloseSubscription: Subscription;
  hubBattleSubscription: Subscription;
  userChangedSubscription: Subscription;
  queueSubscription: Subscription;

  readonly defaultWidth = 1504;
  readonly defaultHeight = 1080;
  readonly defaultAspectRatio = this.defaultWidth / this.defaultHeight;

  tileHeight = 60;
  tileWidth = 0;
  campWidth = 27;
  campHeight = 15;
  campBiom = BiomEnum.Grass;
  charactersMaxCount = 3;

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
    char: 'ground',
    color: {r: 139, g: 133, b: 175, a: 1} as Color,
    backgroundColor: {r: 13, g: 13, b: 17, a: 1} as Color
  };

  openedModal: IModal<unknown>;

  currentActor: Character;
  hintDeclaration: HintDeclaration;

  get userName() {
    return this.userService.user?.name;
  }

  get userExperience() {
    return this.userService.user?.experience;
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

  get interfaceShift() {
    return 362 / this.zoom;
  }

  constructor(
    private loadingService: LoadingService,
    private lobbyStorageService: AsciiLobbyStorageService,
    private userService: UserService,
    private arenaHub: ArenaHubService,
    private modalService: ModalService,
    private router: Router,
    private queueService: QueueService,
    private charsService: CharsService,
    private assetsLoadingService: AssetsLoadingService
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
    this.queueSubscription = this.queueService.queueUpdate.subscribe(() => {
      this.changed = true;
    });
  }

  generateCamp() {
    const userRandom = new Random(this.lobbyStorageService.userHash);
    const biom = asciiBioms[this.campBiom];
    this.activators = new Array<LobbyTileActivator<Character>>(this.charactersMaxCount);
    for (let i = 0; i < this.charactersMaxCount; i++) {
      this.activators[i] = {
        xShift: 4,
        yShift: 1,
        object: this.userService.user.roster[i],
        x: Math.floor(this.campWidth / 2) + (i === 0 ? 5 : -4),
        y: Math.floor(this.campHeight / 2) + ((i % 2) * 3 - (i === 2 ? 3 : 0))
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
          char: tile.char,
          color: tile.color,
          backgroundColor: tile.backgroundColor
        };
      }
    }
  }

  ngOnInit(): void {
    this.lobbyStorageService.userHash = getHashFromString(this.userService.user.id);
    this.tileWidth = this.tileHeight * 0.6;
    this.setupAspectRatio(this.lobbyCanvas.nativeElement.offsetWidth, this.lobbyCanvas.nativeElement.offsetHeight);
    this.canvasWebGLContext = this.lobbyCanvas.nativeElement.getContext('webgl');
    this.canvas2DContext = this.hudCanvas.nativeElement.getContext('2d');
    this.generateCamp();
    this.fullParty = this.userService.user.roster.length >= this.charactersMaxCount;
    this.changed = true;
    this.redrawScene();
    this.drawingTimer = setInterval(() => {
      this.redrawScene();
    }, 1000 / this.updateFrequency);
    this.assetsLoadingService.loadShadersAndCreateProgram(
      this.canvasWebGLContext,
      'vertex-shader-2d.vert',
      'fragment-shader-2d.frag'
    )
      .subscribe((result) => {
        this.charsTexture = this.charsService.getTexture(this.canvasWebGLContext);
        this.shadersProgram = result;
        this.changed = true;
      });
  }

  ngOnDestroy(): void {
    this.lobbyStorageService.clear();
    clearInterval(this.drawingTimer);
    this.canvasWebGLContext.getExtension('WEBGL_lose_context').loseContext();
    this.queueService.dequeue(true);
    this.onCloseSubscription.unsubscribe();
    this.userChangedSubscription.unsubscribe();
  }

  onUpdate() {
    for (const activator of this.activators) {
      activator.object = undefined;
    }
    for (let i = 0; i < this.userService.user.roster.length; i++) {
      const activator = this.activators[i];
      activator.object = this.userService.user.roster[i];
    }
    this.fullParty = this.userService.user.roster.length >= this.charactersMaxCount;
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
        this.currentActor = activator.object;
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
    const cameraLeft = this.campWidth / 2 - (this.canvasWidth - this.interfaceShift) / 2 / this.tileWidth;
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
    this.hudCanvas.nativeElement.width = this.lobbyCanvas.nativeElement.width;
    this.hudCanvas.nativeElement.height = this.lobbyCanvas.nativeElement.height;
    this.zoom = this.lobbyCanvas.nativeElement.offsetWidth / this.canvasWidth;
    this.changed = true;
    this.redrawScene();
  }

  openSettings() {
    if (this.openedModal) {
      this.openedModal.close();
    }
    this.openedModal = this.modalService.openModalWithoutArgs(SettingsModalComponent);
    this.openedModal.onClose.subscribe(() => this.openedModal = undefined);
  }

  patrol() {
    this.queueService.enqueue();
  }

  cancelPatrol() {
    this.queueService.dequeue();
  }

  toTavern() {
    if (this.openedModal) {
      this.openedModal.close();
    }
    this.openedModal = this.modalService.openModalWithoutArgs(TavernModalComponent);
    this.openedModal.onClose.subscribe(() => this.openedModal = undefined);
  }

  private drawPoint(
    tile: LobbyTile<Character>,
    x: number, y: number,
    active: boolean,
    clicked: boolean,

    texturePosition: number,
    colors: Uint8Array,
    textureMapping: Float32Array,
    backgrounds: Uint8Array,
    backgroundTextureMapping: Float32Array) {

    if (tile) {
      const native = tile.activator?.x === x && tile.activator?.y === y && tile.activator.object ?
        actorNatives[tile.activator.object.nativeId] : undefined;
      fillTileMask(this.charsService, backgroundTextureMapping, false, false, false, false, texturePosition);
      if (tile.backgroundColor) {
        fillBackground(backgrounds, tile.backgroundColor.r, tile.backgroundColor.g, tile.backgroundColor.b, texturePosition);
      } else {
        fillBackground(backgrounds, 0, 0, 0, texturePosition);
      }
      let color = native ? native.visualization.color : tile.color;
      color = active ? (clicked ? { r: 170, g: 170, b: 0, a: color.a } : { r: 255, g: 255, b: 68, a: color.a }) : color;
      fillColor(colors, color.r, color.g, color.b, color.a, texturePosition);
      fillChar(this.charsService, textureMapping, (native ? native.visualization.char : tile.char), texturePosition);
    }
  }


  private redrawScene() {
    if (!this.changed) {
      return;
    }
    this.changed = false;
    const userRandom = new Random(this.lobbyStorageService.userHash + (this.queueService.inQueue ? this.queueService.queueSeed : 0));
    if (this.tiles && this.shadersProgram) {
      const cameraLeft = this.campWidth / 2 - (this.canvasWidth - this.interfaceShift) / 2 / this.tileWidth;
      const cameraTop = this.campHeight / 2 - this.canvasHeight / 2 / this.tileHeight + 1;
      this.canvas2DContext.font = `${this.tileHeight}px PT Mono`;
      this.canvas2DContext.textAlign = 'left';
      const left = Math.floor(cameraLeft) - 1;
      const right = Math.ceil(cameraLeft + this.canvasWidth / (this.tileWidth)) + 1;
      const top = Math.floor(cameraTop) - 1;
      const bottom = Math.ceil(cameraTop + this.canvasHeight / (this.tileHeight)) + 1;
      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);
      const clicked = this.mouseState.buttonsInfo[0] && this.mouseState.buttonsInfo[0].pressed;
      const width = right - left + 1;
      const height = bottom - top + 1;
      const textureMapping: Float32Array = new Float32Array(width * height * 12);
      const colors: Uint8Array = new Uint8Array(width * height * 4);
      const backgroundTextureMapping: Float32Array = new Float32Array(width * height * 12);
      const backgrounds: Uint8Array = new Uint8Array(width * height * 4);
      const mainTextureVertexes: Float32Array = new Float32Array(width * height * 12);
      let texturePosition = 0;
      for (let y = -20; y <= 60; y++) {
        for (let x = -40; x <= 80; x++) {
          let tile: LobbyTile<Character>;
          if (x >= left && x <= right && y >= top && y <= bottom) {
            fillVertexPosition(mainTextureVertexes, x, y, left, top, this.tileWidth, this.tileHeight, texturePosition);
            if (x >= 0 && y >= 0 && x < this.campWidth && y < this.campHeight) {
              tile = this.tiles[x][y];
              if (this.queueService.inQueue && this.queueService.queueSeed) {
                const biom = getRandomBiom(userRandom, this.campBiom);
                tile = {
                  char: biom.char,
                  color: biom.color,
                  backgroundColor: biom.backgroundColor,
                  activator: tile.activator
                };
              }
            } else {
              const biom = getRandomBiom(userRandom, this.campBiom);
              tile = {
                char: biom.char,
                color: biom.color,
                backgroundColor: biom.backgroundColor,
                activator: undefined
              };
            }
            this.drawPoint(tile, x, y,
              tile.activator &&
              tile.activator.object &&
              tile.activator.x === x &&
              tile.activator.y === y &&
              mouseX >= x - tile.activator.xShift &&
              mouseX <= x + tile.activator.xShift &&
              mouseY >= y - tile.activator.yShift &&
              mouseY <= y,
              clicked,
              texturePosition,
              colors,
              textureMapping,
              backgrounds,
              backgroundTextureMapping);
            texturePosition++;
          } else {
            userRandom.nextDouble();
          }
        }
      }

      drawArrays(
        this.canvasWebGLContext,
        this.shadersProgram,
        mainTextureVertexes,
        colors,
        backgrounds,
        textureMapping,
        backgroundTextureMapping,
        this.charsTexture,
        Math.round((left - cameraLeft) * this.tileWidth),
        Math.round((top - cameraTop - 1) * this.tileHeight),
        width * this.tileWidth,
        height * this.tileHeight,
        width,
        height,
        this.charsService.width,
        this.charsService.spriteHeight);

      this.canvas2DContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
      this.canvas2DContext.font = `${26}px PT Mono`;
      this.canvas2DContext.textAlign = 'center';
      for (const activator of this.activators) {
        if (activator.object) {
          const x = (activator.x + 0.5 - cameraLeft) * this.tileWidth;
          const y = (activator.y - cameraTop) * this.tileHeight;
          const activated = mouseX >= activator.x - activator.xShift &&
            mouseX <= activator.x + activator.xShift &&
            mouseY >= activator.y - activator.yShift &&
            mouseY <= activator.y;
          this.canvas2DContext.fillStyle = activated ? (clicked ? '#aa0' : '#ff4') : '#ffffff';
          this.canvas2DContext.fillText(activator.object.name, x, y);
        }
      }

      if (!this.loaded) {
        this.loaded = true;
        this.finishLoadingSubscription = this.loadingService.finishLoading()
        .subscribe(() => { this.finishLoadingSubscription.unsubscribe(); });
      }
    }
  }
}
