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
@Component({
  selector: 'app-tavern',
  templateUrl: './tavern.component.html',
  styleUrls: ['./tavern.component.scss']
})
export class TavernComponent implements OnInit, OnDestroy {

  @ViewChild('tavernCanvas', { static: true }) tavernCanvas: ElementRef<HTMLCanvasElement>;
  private canvasContext: CanvasRenderingContext2D;

  @Output() chosenPatron = new EventEmitter<CharacterForSale>();

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
    private lobbyStorageService: AsciiLobbyStorageService
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
            color: { r: 101, g: 67, b: 33, a: 1},
            backgroundColor: { r: 24, g: 16, b: 8},
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
              backgroundColor: { r: 14, g: 13, b: 11},
              activator
            };
            continue;
          }
          if ((y === 3 && x >= 11 || x === 11 && y <= 3) && x !== 15) {
            this.tiles[x][y] = {
              char: '+',
              color: { r: 101, g: 67, b: 33, a: 1},
              backgroundColor: { r: 14, g: 13, b: 11},
              activator
            };
            continue;
          }
          if (y === 2 && x === 12) {
            this.tiles[x][y] = {
              char: '&',
              color: { r: 255, g: 200, b: 200, a: 1},
              backgroundColor: { r: 14, g: 13, b: 11},
              activator
            };
            continue;
          }
          this.tiles[x][y] = {
            char: activator?.object && activator?.x === x && activator?.y === y ? '@' : '.',
            color: activator?.object && activator?.x === x && activator?.y === y ?
              { r: 160, g: 160, b: 160, a: 1 } :
              { r: 173, g: 165, b: 135, a: 1},
            backgroundColor: { r: 14, g: 13, b: 11},
            activator
          };
        }
      }
    }
  }

  ngOnInit(): void {
    this.tileWidth = this.tileHeight * 0.6;
    this.setupAspectRatio(this.tavernCanvas.nativeElement.offsetWidth, this.tavernCanvas.nativeElement.offsetHeight);
    this.canvasContext = this.tavernCanvas.nativeElement.getContext('2d');
    this.generateTavern();
    this.changed = true;
    this.redrawScene();
    this.drawingTimer = setInterval(() => {
      this.redrawScene();
    }, 1000 / this.updateFrequency);
  }

  ngOnDestroy(): void {
    clearInterval(this.drawingTimer);
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
    x: number, y: number,
    active: boolean,
    clicked: boolean) {
    if (tile) {
      const canvasX = (x) * this.tileWidth;
      const canvasY = (y) * this.tileHeight;
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
      this.canvasContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
      this.canvasContext.font = `${this.tileHeight}px PT Mono`;
      this.canvasContext.textAlign = 'left';
      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);
      const clicked = this.mouseState.buttonsInfo[0] && this.mouseState.buttonsInfo[0].pressed;
      for (let x = 0; x < this.tavernWidth; x++) {
        for (let y = 0; y <= this.tavernHeight; y++) {
          const tile = this.tiles[x][y];
          this.drawPoint(tile, x, y,
            tile.activator &&
            tile.activator.object &&
            tile.activator.x === x &&
            tile.activator.y === y &&
            mouseX >= x - tile.activator.xShift &&
            mouseX <= x + tile.activator.xShift &&
            mouseY >= y - tile.activator.yShift &&
            mouseY <= y + tile.activator.yShift,
            clicked);
        }
      }
    }
  }
}
