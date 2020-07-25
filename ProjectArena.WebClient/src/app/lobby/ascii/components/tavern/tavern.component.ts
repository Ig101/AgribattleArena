import { Component, OnInit, Output, EventEmitter, ViewChild, ElementRef, HostListener, OnDestroy } from '@angular/core';
import { CharacterForSale } from '../../model/character-for-sale.model';
import { LobbyTileActivator } from '../../model/lobby-tile-activator.model';
import { LobbyTile } from '../../model/lobby-tile.model';
import { UserService } from 'src/app/shared/services/user.service';
import { AsciiLobbyStorageService } from '../../services/ascii-lobby-storage.service';
import { MouseState } from 'src/app/shared/models/mouse-state.model';
import { Random } from 'src/app/shared/random/random';
import { Character } from '../../model/character.model';
import { getRandomBiom } from 'src/app/shared/bioms/biom.helper';
import { Subscription } from 'rxjs';
import { actorNatives } from 'src/app/battle/ascii/natives';
import { fillTileMask, fillBackground, fillColor, fillChar, drawArrays, fillVertexPosition } from 'src/app/helpers/webgl.helper';
import { CharsService } from 'src/app/shared/services/chars.service';
import { AssetsLoadingService } from 'src/app/shared/services/assets-loading.service';
@Component({
  selector: 'app-tavern',
  templateUrl: './tavern.component.html',
  styleUrls: ['./tavern.component.scss']
})
export class TavernComponent implements OnInit, OnDestroy {

  @ViewChild('tavernCanvas', { static: true }) tavernCanvas: ElementRef<HTMLCanvasElement>;
  private canvasContext: WebGLRenderingContext;
  private shadersProgram: WebGLProgram;
  private charsTexture: WebGLTexture;

  @Output() chosenPatron = new EventEmitter<CharacterForSale>();
  @Output() finishLoading = new EventEmitter<any>();
  private loaded;

  userChangedSubscription: Subscription;

  patrons: LobbyTileActivator<CharacterForSale>[];
  tiles: LobbyTile<CharacterForSale>[][];
  cursor: string;

  readonly defaultWidth = 510;
  readonly defaultHeight = 450;
  readonly defaultAspectRatio = this.defaultWidth / this.defaultHeight;

  tileHeight = 50;
  tileWidth = 0;
  tavernHeight = 9;
  tavernWidth = 17;
  changed = false;

  updateFrequency = 30;

  drawingTimer;

  zoom = 0;

  mouseState: MouseState = {
    buttonsInfo: {},
    x: -1,
    y: -1,
    realX: -1,
    realY: -1
  };

  get canvasWidth() {
    return this.tavernCanvas.nativeElement.width;
  }

  get canvasHeight() {
    return this.tavernCanvas.nativeElement.height;
  }

  constructor(
    private userService: UserService,
    private lobbyStorageService: AsciiLobbyStorageService,
    private charsService: CharsService,
    private assetsLoadingService: AssetsLoadingService
  ) {
    this.userChangedSubscription = this.userService.userChanged.subscribe(() => this.onUpdate() );
  }

  generateTavern() {
    this.patrons = new Array<LobbyTileActivator<CharacterForSale>>(6);
    this.patrons[0] = {
      xShift: 0,
      yShift: 0,
      object: this.userService.user.tavern.find(x => x.id === 1),
      x: 3,
      y: 6
    };
    this.patrons[1] = {
      xShift: 0,
      yShift: 0,
      object: this.userService.user.tavern.find(x => x.id === 2),
      x: 5,
      y: 6
    };
    this.patrons[2] = {
      xShift: 0,
      yShift: 0,
      object: this.userService.user.tavern.find(x => x.id === 3),
      x: 11,
      y: 5
    };
    this.patrons[3] = {
      xShift: 0,
      yShift: 0,
      object: this.userService.user.tavern.find(x => x.id === 4),
      x: 13,
      y: 5
    };
    this.patrons[4] = {
      xShift: 0,
      yShift: 0,
      object: this.userService.user.tavern.find(x => x.id === 5),
      x: 6,
      y: 3
    };
    this.patrons[5] = {
      xShift: 0,
      yShift: 0,
      object: this.userService.user.tavern.find(x => x.id === 6),
      x: 8,
      y: 3
    };
    this.tiles = new Array<LobbyTile<CharacterForSale>[]>(this.tavernWidth);
    for (let x = 0; x < this.tavernWidth; x++) {
      this.tiles[x] = new Array<LobbyTile<CharacterForSale>>(this.tavernHeight);
      for (let y = 0; y < this.tavernWidth; y++) {
        if ((x === 0 || y === 0 || x === this.tavernWidth - 1 || y === this.tavernHeight - 1) &&
          (y !== 0 || x < 3 || x > 5)) {
          this.tiles[x][y] = {
            char: '#',
            color: (x >= 2 && x <= 4 && y > 5) || (x >= 7 && x <= 9 && y > 5) ||
              (x >= 12 && x <= 14 && y > 5) || (y >= 2 && y <= 4 && x < 5) ?
              { r: 71, g: 45, b: 21, a: 1 } :  { r: 101, g: 67, b: 33, a: 1},
            backgroundColor: (x >= 2 && x <= 4 && y > 5) || (x >= 7 && x <= 9 && y > 5) ||
              (x >= 12 && x <= 14 && y > 5) || (y >= 2 && y <= 4 && x < 5) ?
              { r: 30, g: 20, b: 10}  : { r: 40, g: 26, b: 14},
            activator: undefined
          };
        } else {
          const activator = this.patrons.find(a =>
            a.x - a.xShift <= x &&
            a.x + a.xShift >= x &&
            a.y - a.yShift <= y &&
            a.y + a.yShift >= y);
          if (x === 4 && y === 6 || x === 12 && y === 5 || x === 7 && y === 3) {
            this.tiles[x][y] = {
              char: '■',
              color: { r: 101, g: 67, b: 33, a: 1},
              backgroundColor: { r: 22, g: 20, b: 18},
              activator
            };
            continue;
          }
          if ((y === 3 && x >= 11 || x === 11 && y <= 3) && x !== 15) {
            this.tiles[x][y] = {
              char: '+',
              color: { r: 101, g: 67, b: 33, a: 1},
              backgroundColor: { r: 22, g: 20, b: 18},
              activator
            };
            continue;
          }
          if (y === 2 && x === 12) {
            this.tiles[x][y] = {
              char: '&',
              color: { r: 255, g: 200, b: 200, a: 1},
              backgroundColor: { r: 22, g: 20, b: 18},
              activator
            };
            continue;
          }
          this.tiles[x][y] = {
            char: activator?.object && activator?.x === x && activator?.y === y ? '@' : '.',
            color: activator?.object && activator?.x === x && activator?.y === y ?
              { r: 160, g: 160, b: 160, a: 1 } :
              { r: 173, g: 165, b: 135, a: 1},
            backgroundColor: { r: 22, g: 20, b: 18},
            activator
          };
        }
      }
    }
  }

  ngOnInit(): void {
    this.tileWidth = this.tileHeight * 0.6;
    this.setupAspectRatio(this.tavernCanvas.nativeElement.offsetWidth, this.tavernCanvas.nativeElement.offsetHeight);
    this.canvasContext = this.tavernCanvas.nativeElement.getContext('webgl');
    this.generateTavern();
    this.changed = true;
    this.redrawScene();
    this.drawingTimer = setInterval(() => {
      this.redrawScene();
    }, 1000 / this.updateFrequency);
    this.assetsLoadingService.loadShadersAndCreateProgram(
      this.canvasContext,
      'vertex-shader-2d.vert',
      'fragment-shader-2d.frag'
    )
      .subscribe((result) => {
        this.charsTexture = this.charsService.getTexture(this.canvasContext);
        this.shadersProgram = result;
        this.changed = true;
      });
  }

  ngOnDestroy(): void {
    clearInterval(this.drawingTimer);
    this.canvasContext.getExtension('WEBGL_lose_context').loseContext();
    this.userChangedSubscription.unsubscribe();
  }

  onUpdate() {
    for (const space of this.patrons) {
      space.object = undefined;
      this.tiles[space.x][space.y].char = '.';
      this.tiles[space.x][space.y].color = { r: 173, g: 165, b: 135, a: 1};
    }
    for (const patron of this.userService.user.tavern) {
      const space = this.patrons[patron.id - 1];
      space.object = patron;
      this.tiles[space.x][space.y].char = '@';
      this.tiles[space.x][space.y].color = { r: 160, g: 160, b: 160, a: 1 };
    }
    this.changed = true;
  }

  onResize() {
    this.setupAspectRatio(this.tavernCanvas.nativeElement.offsetWidth, this.tavernCanvas.nativeElement.offsetHeight);
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
    this.recalculateMouseMove(event.offsetX, event.offsetY, event.timeStamp);
    const newX = Math.floor(this.mouseState.x);
    const newY = Math.floor(this.mouseState.y);
    if (event.button === 0 && x >= 0 && x < this.tavernWidth && y >= 0 && y < this.tavernHeight) {
      const activator = this.tiles[x][y].activator;
      const newActivator = this.tiles[newX][newY].activator;
      if (activator && activator.object && activator === newActivator) {
        this.chosenPatron.emit(activator.object);
      }
    }
  }

  onMouseUpWindow(event: MouseEvent) {
    setTimeout(() => {
      this.mouseState.buttonsInfo[event.button] = {pressed: false, timeStamp: 0};
      this.changed = true;
      if (this.tavernCanvas.nativeElement !== event.target) {
        this.mouseState.x = undefined;
        this.mouseState.y = undefined;
      }
    });
  }

  onMouseDown(event: MouseEvent) {
    this.changed = true;
    this.mouseState.buttonsInfo[event.button] = {pressed: true, timeStamp: 0};
  }

  onMouseMove(event: MouseEvent) {
    this.mouseState.realX = event.offsetX;
    this.mouseState.realY = event.offsetY;
    this.recalculateMouseMove(event.offsetX, event.offsetY, event.timeStamp);
  }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event) {
    event.preventDefault();
  }

  private recalculateMouseMove(x: number, y: number, timeStamp?: number) {
    const leftKey = this.mouseState.buttonsInfo[0];
    const rightKey = this.mouseState.buttonsInfo[2];
    const newX = x / this.zoom / this.tileWidth;
    const newY = y / this.zoom / this.tileHeight;
    const mouseX = Math.floor(newX);
    const mouseY = Math.floor(newY);
    this.cursor = undefined;
    if (this.tiles && mouseX >= 0 && mouseX < this.tavernWidth && mouseY >= 0 && mouseY < this.tavernHeight) {
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
      this.tavernCanvas.nativeElement.width = oldWidth;
      this.tavernCanvas.nativeElement.height = oldWidth / newAspectRatio;
    } else {
      const oldHeight = this.defaultHeight;
      this.tavernCanvas.nativeElement.width = oldHeight * newAspectRatio;
      this.tavernCanvas.nativeElement.height = oldHeight;
    }
    this.zoom = this.tavernCanvas.nativeElement.offsetWidth / this.canvasWidth;
    this.changed = true;
    this.redrawScene();
  }

  private drawPoint(
    tile: LobbyTile<CharacterForSale>,
    x: number,
    y: number,
    tiles: LobbyTile<CharacterForSale>[][],
    active: boolean,
    clicked: boolean,

    texturePosition: number,
    colors: Uint8Array,
    textureMapping: Float32Array,
    backgrounds: Uint8Array,
    backgroundTextureMapping: Float32Array) {

    fillTileMask(
      this.charsService,
      backgroundTextureMapping,
      tile.char === '#' && (x > 0 && (tiles[x - 1][y].char !== tile.char || tiles[x - 1][y].color.r < tile.color.r) || x === 0),
      tile.char === '#' && (x < this.tavernWidth - 1 &&
        (tiles[x + 1][y].char !== tile.char || tiles[x + 1][y].color.r < tile.color.r) || x === this.tavernWidth - 1),
      tile.char === '#' && (y > 0 && (tiles[x][y - 1].char !== tile.char || tiles[x][y - 1].color.r < tile.color.r) || y === 0),
      tile.char === '#' && (y < this.tavernHeight - 1 &&
        (tiles[x][y + 1].char !== tile.char || tiles[x][y + 1].color.r < tile.color.r) || y === this.tavernHeight - 1),
      texturePosition);
    if (tile.backgroundColor) {
      fillBackground(backgrounds, tile.backgroundColor.r, tile.backgroundColor.g, tile.backgroundColor.b, texturePosition);
    } else {
      fillBackground(backgrounds, 0, 0, 0, texturePosition);
    }
    const color = active ?
      (clicked ? { r: 170, g: 170, b: 0, a: tile.color.a } : { r: 255, g: 255, b: 68, a: tile.color.a }) : tile.color;
    fillColor(colors, color.r, color.g, color.b, color.a, texturePosition);
    fillChar(this.charsService, textureMapping, tile.char, texturePosition);
  }

  private redrawScene() {
    if (!this.changed) {
      return;
    }
    this.changed = false;
    if (this.tiles && this.shadersProgram) {
      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);
      const clicked = this.mouseState.buttonsInfo[0] && this.mouseState.buttonsInfo[0].pressed;
      const textureMapping: Float32Array = new Float32Array(this.tavernWidth * this.tavernHeight * 12);
      const colors: Uint8Array = new Uint8Array(this.tavernWidth * this.tavernHeight * 4);
      const backgroundTextureMapping: Float32Array = new Float32Array(this.tavernWidth * this.tavernHeight * 12);
      const backgrounds: Uint8Array = new Uint8Array(this.tavernWidth * this.tavernHeight * 4);
      const mainTextureVertexes: Float32Array = new Float32Array(this.tavernWidth * this.tavernHeight * 12);
      let texturePosition = 0;
      for (let y = 0; y < this.tavernHeight; y++) {
        for (let x = 0; x < this.tavernWidth; x++) {
          fillVertexPosition(mainTextureVertexes, x, y, 0, 0, this.tileWidth, this.tileHeight, texturePosition);
          const tile = this.tiles[x][y];
          if (tile) {
            this.drawPoint(tile, x, y, this.tiles,
              tile.activator &&
              tile.activator.object &&
              tile.activator.x === x &&
              tile.activator.y === y &&
              mouseX >= x - tile.activator.xShift &&
              mouseX <= x + tile.activator.xShift &&
              mouseY >= y - tile.activator.yShift &&
              mouseY <= y + tile.activator.yShift,
              clicked,
              texturePosition,
              colors,
              textureMapping,
              backgrounds,
              backgroundTextureMapping);
            texturePosition++;
          }
        }
      }
      drawArrays(
        this.canvasContext,
        this.shadersProgram,
        mainTextureVertexes,
        colors,
        backgrounds,
        textureMapping,
        backgroundTextureMapping,
        this.charsTexture,
        0,
        0,
        this.tavernWidth * this.tileWidth,
        this.tavernHeight * this.tileHeight,
        this.tavernWidth,
        this.tavernHeight,
        this.charsService.width,
        this.charsService.spriteHeight);

      if (!this.loaded) {
        this.loaded = true;
        this.finishLoading.emit();
      }
    }
  }
}
