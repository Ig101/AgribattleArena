import { Component, OnInit, OnDestroy, ViewChild, ElementRef, HostListener } from '@angular/core';
import { LoadingService } from '../shared/services/loading.service';
import { SceneService } from '../engine/services/scene.service';
import { SynchronizationService } from '../engine/services/synchronization.service';
import { FullSynchronizationInfo } from '../shared/models/synchronization/full-synchronization-info.model';
import { ActorSynchronization } from '../shared/models/synchronization/objects/actor-synchronization.model';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ModalService } from '../shared/services/modal.service';
import { CharsService } from '../shared/services/chars.service';
import { AssetsLoadingService } from '../shared/services/assets-loading.service';
import { Random } from '../shared/random/random';
import { fillVertexPosition, drawArrays, fillTileMask, fillBackground, fillColor, fillChar } from '../helpers/webgl.helper';
import { ActionSquareTypeEnum } from '../battle/ascii/models/enum/action-square-type.enum';
import { getRandomBiom } from '../shared/bioms/biom.helper';
import { Color } from '../shared/models/color.model';
import { heightImpact, brightImpact } from '../battle/ascii/helpers/scene-draw.helper';
import { Scene } from '../engine/scene/scene.object';
import { BiomEnum } from '../shared/models/enum/biom.enum';
import { Actor } from '../engine/scene/actor.object';
import { Tile } from '../engine/scene/tile.object';
import { DEFAULT_HEIGHT, RANGED_RANGE, VISIBILITY_AMPLIFICATION } from '../content/content.helper';
import { angleBetween, rangeBetween } from '../helpers/math.helper';
import { MouseState } from '../shared/models/mouse-state.model';

@Component({
  selector: 'app-fight',
  templateUrl: './fight.component.html',
  styleUrls: ['./fight.component.scss']
})
export class FightComponent implements OnInit, OnDestroy {

  @ViewChild('battleCanvas', { static: true }) battleCanvas: ElementRef<HTMLCanvasElement>;
  @ViewChild('hudCanvas', { static: true }) hudCanvas: ElementRef<HTMLCanvasElement>;
  private canvas2DContext: CanvasRenderingContext2D;
  private canvasWebGLContext: WebGLRenderingContext;
  private charsTexture: WebGLTexture;
  private shadersProgram: WebGLProgram;

  mouseState: MouseState = {
    buttonsInfo: {},
    x: -1,
    y: -1,
    realX: -1,
    realY: -1
  };

  blocked = false;

  private tileWidthInternal = 0;
  private tileHeightInternal = 60;
  readonly defaultWidth = 1600;
  readonly defaultHeight = 1080;
  readonly defaultAspectRatio = this.defaultWidth / this.defaultHeight;

  zoom = 0;

  tickerState = false;
  tickerTime: number;
  tickerPeriod = (1000 / 10);

  cameraX: number;
  cameraY: number;
  battleZoom: number;

  updateSubscription: Subscription;

  rangeMapIsActive: boolean;
  rangeMap: boolean[][]; // undefined for nothing, false for red, true for yellow
  textureMapping: Float32Array;
  colors: Uint8Array;
  backgroundTextureMapping: Float32Array;
  backgrounds: Uint8Array;
  mainTextureVertexes: Float32Array;

  redPath: Path2D;
  yellowPath: Path2D;
  greenPath: Path2D;

  cursorVertexes: {
    position: number;
    textureMapping: Float32Array;
    colors: Uint8Array;
  };

  get canvasWidth() {
    return this.battleCanvas.nativeElement.width;
  }

  get canvasHeight() {
    return this.battleCanvas.nativeElement.height;
  }

  get turnTime() {
    return this.scene.turnTime;
  }

  get tileWidth() {
    return this.tileWidthInternal * this.battleZoom;
  }

  get tileHeight() {
    return this.tileHeightInternal * this.battleZoom;
  }

  get interfaceShift() {
    return 362 / this.zoom;
  }

  private get scene() {
    return this.sceneService.scene;
  }

  constructor(
    private loadingService: LoadingService,
    private sceneService: SceneService,
    private activatedRoute: ActivatedRoute,
    private modalService: ModalService,
    private charsService: CharsService,
    private assetsLoadingService: AssetsLoadingService
  ) {
    this.mouseState.buttonsInfo[0] = {
      pressed: false,
      timeStamp: 0
    };
    this.mouseState.buttonsInfo[2] = {
      pressed: false,
      timeStamp: 0
    };
  }

  ngOnInit(): void {
    this.tileWidthInternal = this.tileHeightInternal * 0.6;
    this.setupAspectRatio(this.battleCanvas.nativeElement.offsetWidth, this.battleCanvas.nativeElement.offsetHeight);
    this.canvasWebGLContext = this.battleCanvas.nativeElement.getContext('webgl');
    this.canvas2DContext = this.hudCanvas.nativeElement.getContext('2d');
    this.canvas2DContext.font = `${26}px PT Mono`;
    this.canvas2DContext.textAlign = 'center';
    this.canvas2DContext.globalAlpha = 1.0;
    this.createSampleScene();
    this.cameraX = this.scene.width / 2;
    this.cameraY = this.scene.height / 2;
    this.battleZoom = 1;
    this.updateSubscription = this.sceneService.updateSub.subscribe((shift) => {
      this.redraw(shift);
    });
    this.rangeMapIsActive = true;
    const mapSize = RANGED_RANGE * 2 + 1;
    this.rangeMap = new Array<boolean[]>(mapSize);
    for (let i = 0; i < mapSize; i++) {
      this.rangeMap[i] = new Array<boolean>(mapSize);
    }

    this.assetsLoadingService.loadShadersAndCreateProgram(
      this.canvasWebGLContext,
      'vertex-shader-2d.vert',
      'fragment-shader-2d.frag'
    )
      .subscribe((result) => {
        this.charsTexture = this.charsService.getTexture(this.canvasWebGLContext);
        this.shadersProgram = result;
        this.redraw();
        setTimeout(() => {
          this.loadingService.finishLoading()
          .subscribe(() => {
            this.sceneService.startUpdates();
          });
        }, 200);
      });
  }

  ngOnDestroy(): void {
    this.updateSubscription.unsubscribe();
    this.sceneService.clearScene();
  }

  createSampleScene() {
    let idCounterPosition = 1000;
    const actors: ActorSynchronization[] = [];
    let tilesCounter = 1;
    for (let x = 0; x < 28; x++) {
      for (let y = 0; y < 16; y++) {
        actors.push({
          reference: {
            id: idCounterPosition++,
            x,
            y
          },
          left: false,
          name: 'Ground',
          char: 'ground',
          color: {r: 60, g: 61, b: 95, a: 1},
          ownerId: undefined,
          tags: ['tile'],
          parentId: tilesCounter,
          durability: 100000,
          maxDurability: 100000,
          turnCost: 1,
          initiativePosition: 0,
          height: (x > 10 || x < 6) && y > 5 ? 900 : 500,
          volume: 10000,
          freeVolume: 9000,
          preparationReactions: [],
          activeReactions: [],
          clearReactions: [],
          actions: [],
          actors: [],
          buffs: [],
        });
        actors.push({
          reference: {
            id: idCounterPosition++,
            x,
            y
          },
          left: false,
          name: 'Grass',
          char: 'grass',
          color: { r: 45, g: 60, b: 150, a: 1 },
          ownerId: undefined,
          tags: ['tile'],
          parentId: tilesCounter,
          durability: 100,
          maxDurability: 100,
          turnCost: 1,
          initiativePosition: 0,
          height: 5,
          volume: 250,
          freeVolume: 0,
          preparationReactions: [],
          activeReactions: [],
          clearReactions: [],
          actions: [],
          actors: [],
          buffs: [],
        });
        tilesCounter++;
      }
    }
    actors.push({
      reference: {
        id: 5000,
        x: 9,
        y: 7
      },
      left: false,
      name: 'Actor',
      char: 'adventurer',
      color: { r: 255, g: 155, b: 55, a: 1 },
      ownerId: 'sampleP',
      tags: ['active'],
      parentId: 1 + 9 * 16 + 7,
      durability: 100,
      maxDurability: 100,
      turnCost: 1,
      initiativePosition: 0,
      height: 180,
      volume: 120,
      freeVolume: 40,
      preparationReactions: [],
      activeReactions: [],
      clearReactions: [],
      actions: [],
      actors: [],
      buffs: [],
    });
    this.sceneService.setupGame(
      {
        id: 'sampleS',
        timeLine: 0,
        idCounterPosition,
        currentPlayerId: 'sampleP',
        actors,
        players: [
          {
            id: 'sampleP'
          }
        ],
        width: 28,
        height: 16,
        biom: BiomEnum.Grass,
        waitingActions: []
      },
      undefined,
      {
        time: 8000000000,
        tempActor: {
          id: 5000,
          x: 9,
          y: 7
        }
      }
    );
  }

  private isOnClickablePosition() {
    if (!this.scene || !this.scene.currentActor) {
      return false;
    }
    const mouseX = Math.floor(this.mouseState.x);
    const mouseY = Math.floor(this.mouseState.y);
    const rangeMapPositionX = mouseX - this.scene.currentActor.x + RANGED_RANGE;
    const rangeMapPositionY = mouseY - this.scene.currentActor.y + RANGED_RANGE;
    return this.rangeMapIsActive &&
      rangeMapPositionX >= 0 &&
      rangeMapPositionY >= 0 &&
      rangeMapPositionX < RANGED_RANGE * 2 + 1 &&
      rangeMapPositionY < RANGED_RANGE * 2 + 1 &&
      this.rangeMap[rangeMapPositionX][rangeMapPositionY] !== undefined;
  }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event) {
    event.preventDefault();
  }

  onResize() {
    this.setupAspectRatio(this.battleCanvas.nativeElement.offsetWidth, this.battleCanvas.nativeElement.offsetHeight);
  }

  private recalculateMouseMove(x: number, y: number, timeStamp?: number) {
    const leftKey = this.mouseState.buttonsInfo[0];
    const rightKey = this.mouseState.buttonsInfo[2];
    if (!rightKey.pressed && !leftKey.pressed) {
      const cameraLeft = this.cameraX - (this.canvasWidth - this.interfaceShift) / 2 / this.tileWidth;
      const cameraTop = this.cameraY - this.canvasHeight / 2 / this.tileHeight;
      const newX = x / this.zoom / this.tileWidth + cameraLeft;
      const newY = y / this.zoom / this.tileHeight + cameraTop;
      this.mouseState.x = newX;
      this.mouseState.y = newY;
      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);
    }
  }

  onMouseMove(event: MouseEvent) {
    if (!this.blocked) {
      this.mouseState.realX = event.x;
      this.mouseState.realY = event.y;
      this.recalculateMouseMove(event.x, event.y, event.timeStamp);
    }
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
    this.hudCanvas.nativeElement.width = this.battleCanvas.nativeElement.width;
    this.hudCanvas.nativeElement.height = this.battleCanvas.nativeElement.height;
    this.zoom = this.battleCanvas.nativeElement.offsetWidth / this.canvasWidth;
    if (this.scene) {
      this.scene.visualizationChanged = true;
    }
    this.redraw();
  }

  private fillRangeMapPart(
    actorZ: number,
    actorX: number,
    actorY: number,
    startPointX: number,
    startPointY: number,
  ) {
    let nextX = actorX;
    let nextY = actorY;
    let visible = true;
    const angle = angleBetween(actorX, actorY, startPointX, startPointY);
    const sin = Math.sin(angle) * 0.8;
    const cos = Math.cos(angle) * 0.8;
    while (Math.floor(nextX) !== startPointX || Math.floor(nextY) !== startPointY) {
      let currentX = Math.floor(nextX);
      let currentY = Math.floor(nextY);
      while (Math.floor(nextX) === currentX && Math.floor(nextY) === currentY) {
        nextX += cos;
        nextY += sin;
      }
      currentX = Math.floor(nextX);
      currentY = Math.floor(nextY);
      if (currentX < 0 || currentY < 0 || currentX >= this.scene.width || currentY >= this.scene.height) {
        return;
      }
      const range = rangeBetween(actorX, actorY, currentX, currentY);
      if (range > RANGED_RANGE) {
        return;
      }
      if (this.rangeMap[currentX - actorX + RANGED_RANGE][currentY - actorY + RANGED_RANGE] !== true) {
        this.rangeMap[currentX - actorX + RANGED_RANGE][currentY - actorY + RANGED_RANGE] = visible;
      }
      if (this.scene.tiles[currentX][currentY].height > actorZ + VISIBILITY_AMPLIFICATION * range) {
        visible = false;
      }
    }
  }

  private fillRangeMapAndPathes(
    actorZ: number,
    actorX: number,
    actorY: number,
    yellowPath: Path2D,
    redPath: Path2D,
    cameraLeft: number,
    cameraTop: number
  ) {
    for (let x = 0; x < RANGED_RANGE * 2 + 1; x++) {
      for (let y = 0; y < RANGED_RANGE * 2 + 1; y++) {
        this.rangeMap[x][y] = undefined;
      }
    }
    this.rangeMap[RANGED_RANGE][RANGED_RANGE] = true;
    for (let i = -RANGED_RANGE; i <= RANGED_RANGE; i++) {
      let newX = actorX + i;
      let newY = actorY - RANGED_RANGE;
      this.fillRangeMapPart(actorZ, actorX, actorY, newX, newY);
      newX = actorX - RANGED_RANGE;
      newY = actorY + i;
      this.fillRangeMapPart(actorZ, actorX, actorY, newX, newY);
      newX = actorX + i;
      newY = actorY + RANGED_RANGE;
      this.fillRangeMapPart(actorZ, actorX, actorY, newX, newY);
      newX = actorX + RANGED_RANGE;
      newY = actorY + i;
      this.fillRangeMapPart(actorZ, actorX, actorY, newX, newY);
    }
    for (let x = 0; x < RANGED_RANGE * 2 + 1; x++) {
      for (let y = 0; y < RANGED_RANGE * 2 + 1; y++) {
        const value = this.rangeMap[x][y];
        if (value !== undefined) {
          const canvasX = (actorX + x - RANGED_RANGE - cameraLeft) * this.tileWidth;
          const canvasY = (actorY + y - RANGED_RANGE - cameraTop) * this.tileHeight;
          // left
          if (x === 0 || this.rangeMap[x - 1][y] === undefined) {
            const path = value ? yellowPath : redPath;
            path.moveTo(canvasX, canvasY - 1);
            path.lineTo(canvasX, canvasY + this.tileHeight + 1);
          } else if (this.rangeMap[x - 1][y] === false && value === true) {
            yellowPath.moveTo(canvasX, canvasY - 1);
            yellowPath.lineTo(canvasX, canvasY + this.tileHeight + 1);
          }
          // right
          if (x === RANGED_RANGE * 2 || this.rangeMap[x + 1][y] === undefined) {
            const path = value ? yellowPath : redPath;
            path.moveTo(canvasX + this.tileWidth, canvasY - 1);
            path.lineTo(canvasX + this.tileWidth, canvasY + this.tileHeight + 1);
          } else if (this.rangeMap[x + 1][y] === false && value === true) {
            yellowPath.moveTo(canvasX + this.tileWidth, canvasY - 1);
            yellowPath.lineTo(canvasX + this.tileWidth, canvasY + this.tileHeight + 1);
          }
          // top
          if (y === 0 || this.rangeMap[x][y - 1] === undefined) {
            const path = value ? yellowPath : redPath;
            path.moveTo(canvasX - 1, canvasY);
            path.lineTo(canvasX + this.tileWidth + 1, canvasY);
          } else if (this.rangeMap[x][y - 1] === false && value === true) {
            yellowPath.moveTo(canvasX - 1, canvasY);
            yellowPath.lineTo(canvasX + this.tileWidth + 1, canvasY);
          }
          // bottom
          if (y === RANGED_RANGE * 2 || this.rangeMap[x][y + 1] === undefined) {
            const path = value ? yellowPath : redPath;
            path.moveTo(canvasX - 1, canvasY + this.tileHeight);
            path.lineTo(canvasX + this.tileWidth + 1, canvasY + this.tileHeight);
          } else if (this.rangeMap[x][y + 1] === false && value === true) {
            yellowPath.moveTo(canvasX - 1, canvasY + this.tileHeight);
            yellowPath.lineTo(canvasX + this.tileWidth + 1, canvasY + this.tileHeight);
          }
        }
      }
    }
  }

  private drawDummyPoint(
    char: string,
    color: Color,
    texturePosition: number,

    colors: Uint8Array,
    textureMapping: Float32Array,
    backgrounds: Uint8Array,
    backgroundTextureMapping: Float32Array) {

    const dim = 0.3;
    fillTileMask(this.charsService, backgroundTextureMapping, false, false, false, false, texturePosition);
    fillBackground(backgrounds, color.r * dim / 5, color.g * dim / 5, color.b * dim / 5, texturePosition);
    fillColor(colors, color.r * dim, color.g * dim, color.b * dim, color.a, texturePosition);
    fillChar(this.charsService, textureMapping, char, texturePosition);
  }

  private getTileActorAndVisibleActors(tile: Tile) {
    let backgroundActor: Actor;
    let visibleActor: Actor;
    let multiActor = false;
    for (let i = tile.actors.length - 1; i >= 0; i--) {
      const actor = tile.actors[i];
      if (actor.tags.includes('tile')) {
        if (!visibleActor) {
          visibleActor = actor;
        }
        backgroundActor = actor;
        break;
      }
      if (!visibleActor) {
        visibleActor = actor;
        continue;
      }
      if (visibleActor.tags.includes('small')) {
        if (!actor.tags.includes('small')) {
          visibleActor = actor;
        }
        multiActor = true;
      }
    }
    return { backgroundActor, visibleActor: (visibleActor.tags.includes('small') && multiActor) ? undefined : visibleActor , multiActor };
  }

  private getTileHeight(x: number, y: number) {
    if (x < 0 || y < 0 || x >= this.scene.width || y >= this.scene.height) {
      return DEFAULT_HEIGHT;
    }
    const tile = this.scene.tiles[x][y];
    for (let i = tile.actors.length - 1; i >= 0; i--) {
      const actor = tile.actors[i];
      if (actor.tags.includes('tile')) {
        return actor.z + actor.height;
      }
    }
    return 0;
  }

  private drawPoint(
    x: number,
    y: number,
    texturePosition: number,

    cameraLeft: number,
    cameraTop: number,

    greenPath: Path2D,
    yellowPath: Path2D,
    redPath: Path2D,
    colors: Uint8Array,
    textureMapping: Float32Array,
    backgrounds: Uint8Array,
    backgroundTextureMapping: Float32Array) {

    const tile = this.scene.tiles[x][y];
    if (tile) {
      const canvasX = Math.round((x - cameraLeft) * this.tileWidth);
      const canvasY = Math.round((y - cameraTop) * this.tileHeight);
      const info = this.getTileActorAndVisibleActors(tile);
      const currentTileHeight = info.backgroundActor ? info.backgroundActor.z + info.backgroundActor.height : 0;
      fillTileMask(
        this.charsService,
        backgroundTextureMapping,
        currentTileHeight - this.getTileHeight(x - 1, y) >= 120,
        currentTileHeight - this.getTileHeight(x + 1, y) >= 120,
        currentTileHeight - this.getTileHeight(x, y - 1) >= 120,
        currentTileHeight - this.getTileHeight(x, y + 1) >= 120,
        texturePosition);
      if (info.backgroundActor) {
        const background = heightImpact(currentTileHeight, info.backgroundActor.color);
        fillBackground(
          backgrounds,
          background.r / 5,
          background.g / 5,
          background.b / 5,
          texturePosition);
      } else {
        fillBackground(backgrounds, 0, 0, 0, texturePosition);
      }
      let color: Color;
      let char: string;
      let mirrored: boolean;
      if (info.multiActor && (!info.visibleActor || this.tickerState)) {
        color = { r: 255, g: 255, b: 0, a: 1 };
        char = '*';
        mirrored = false;
      } else if (!info.visibleActor) {
        color = { r: 0, g: 0, b: 0, a: 0 };
        char = ' ';
        mirrored = false;
      } else {
        color = info.visibleActor === info.backgroundActor ?
        heightImpact(currentTileHeight, info.visibleActor.color) :
        info.visibleActor.color;
        char = info.visibleActor.char;
        mirrored = info.visibleActor.left;
      }
      // TODO TileStubs
      fillColor(colors, color.r, color.g, color.b, color.a, texturePosition);
      fillChar(
        this.charsService, textureMapping, info.visibleActor.char, texturePosition, mirrored);
      if (info.visibleActor && info.visibleActor.tags.includes('active')) {
        if (info.visibleActor.maxDurability) {
          const percentOfHealth = Math.max(0, Math.min(info.visibleActor.durability / info.visibleActor.maxDurability, 1));
          let path: Path2D;
          if (percentOfHealth > 0.65) {
            path = greenPath;
          } else if (percentOfHealth > 0.25) {
            path = yellowPath;
          } else {
            path = redPath;
          }
          const zoomMultiplier = Math.floor(this.battleZoom);
          path.moveTo(canvasX + 1 + zoomMultiplier, canvasY + 1 + zoomMultiplier);
          path.lineTo(
            canvasX + percentOfHealth * (this.tileWidth - 2 * 1 - zoomMultiplier) + 1 + zoomMultiplier,
            canvasY + 1 + zoomMultiplier);
        }
      }
    }
  }

  redraw(shift?: number) {
    if (this.scene && this.shadersProgram) {

      if (shift) {
        this.tickerTime -= shift;
        if (this.tickerTime <= 0) {
          this.tickerState = !this.tickerState;
          this.tickerTime += this.tickerPeriod;
        }
      }

      const time = performance.now();
      const sceneRandom = new Random(this.scene.hash);
      const cameraLeft = this.cameraX - (this.canvasWidth - this.interfaceShift) / 2 / this.tileWidth;
      const cameraTop = this.cameraY - this.canvasHeight / 2 / this.tileHeight;
      const left = Math.floor(cameraLeft) - 1;
      const right = Math.ceil(cameraLeft + this.canvasWidth / (this.tileWidth)) + 1;
      const top = Math.floor(cameraTop) - 1;
      const bottom = Math.ceil(cameraTop + this.canvasHeight / (this.tileHeight)) + 1;
      const width = right - left + 1;
      const height = bottom - top + 1;
      if (this.scene.visualizationChanged) {
        this.redPath = new Path2D();
        this.yellowPath = new Path2D();
        this.greenPath = new Path2D();

        // TODO Disable on enemy turn or when cannot cast ranged skills
        if (this.rangeMapIsActive) {
          this.fillRangeMapAndPathes(
            this.scene.currentActor.z + this.scene.currentActor.height,
            this.scene.currentActor.x,
            this.scene.currentActor.y,
            this.yellowPath,
            this.redPath,
            cameraLeft,
            cameraTop);
        }
        this.textureMapping = new Float32Array(width * height * 12);
        this.colors = new Uint8Array(width * height * 4);
        this.backgroundTextureMapping = new Float32Array(width * height * 12);
        this.backgrounds = new Uint8Array(width * height * 4);
        this.mainTextureVertexes = new Float32Array(width * height * 12);
        let texturePosition = 0;
        for (let y = -20; y <= 60; y++) {
          for (let x = -40; x <= 80; x++) {
            if (x >= left && y >= top && x <= right && y <= bottom) {
              fillVertexPosition(this.mainTextureVertexes, x, y, left, top, this.tileWidth, this.tileHeight, texturePosition);
              if (x >= 0 && y >= 0 && x < this.scene.width && y < this.scene.height) {
                sceneRandom.next();
                this.drawPoint(x, y, texturePosition, cameraLeft, cameraTop,
                  this.greenPath, this.yellowPath, this.redPath, this.colors, this.textureMapping,
                  this.backgrounds, this.backgroundTextureMapping);
              } else {
                const biom = getRandomBiom(sceneRandom, this.scene.biom);
                this.drawDummyPoint(biom.char, biom.color, texturePosition,
                  this.colors, this.textureMapping, this.backgrounds, this.backgroundTextureMapping);
              }
              texturePosition++;
            } else {
              sceneRandom.next();
            }
          }
        }
        this.cursorVertexes = undefined;
      }

      if (this.cursorVertexes) {
        for (let i = 0; i < 12; i++) {
          this.textureMapping[this.cursorVertexes.position * 12 + i] = this.cursorVertexes.textureMapping[i];
        }
        for (let i = 0; i < 4; i++) {
          this.colors[this.cursorVertexes.position * 4 + i] = this.cursorVertexes.colors[i];
        }
        this.cursorVertexes = undefined;
      }

      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);

      const rangeMapPositionX = mouseX - this.scene.currentActor.x + RANGED_RANGE;
      const rangeMapPositionY = mouseY - this.scene.currentActor.y + RANGED_RANGE;
      if (this.rangeMapIsActive &&
        rangeMapPositionX >= 0 &&
        rangeMapPositionY >= 0 &&
        rangeMapPositionX < RANGED_RANGE * 2 + 1 &&
        rangeMapPositionY < RANGED_RANGE * 2 + 1 &&
        this.rangeMap[rangeMapPositionX][rangeMapPositionY] !== undefined) {

        const value = this.rangeMap[rangeMapPositionX][rangeMapPositionY];
        if (value !== undefined) {
          const position = (mouseY - top) * width + (mouseX - left);
          this.cursorVertexes = {
            position,
            textureMapping: this.textureMapping.slice(position * 12, position * 12 + 12),
            colors: this.colors.slice(position * 4, position * 4 + 4)
          };
          fillColor(this.colors, 255, value ? 255 : 0, 0, 1, position);
          fillChar(
            this.charsService, this.textureMapping, 'x', position, false);
        }
      }

      drawArrays(
        this.canvasWebGLContext,
        this.shadersProgram,
        this.mainTextureVertexes,
        this.colors,
        this.backgrounds,
        this.textureMapping,
        this.backgroundTextureMapping,
        this.charsTexture,
        Math.round((left - cameraLeft) * this.tileWidth),
        Math.round((top - cameraTop - 1) * this.tileHeight),
        (right - left + 1) * this.tileWidth,
        (bottom - top + 1) * this.tileHeight,
        (right - left + 1),
        (bottom - top + 1),
        this.charsService.width,
        this.charsService.spriteHeight);
      this.canvas2DContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
      this.canvas2DContext.lineWidth = 2;
      this.canvas2DContext.strokeStyle = 'rgba(200, 0, 0, 1.0)';
      this.canvas2DContext.stroke(this.redPath);
      this.canvas2DContext.strokeStyle = 'rgba(255, 255, 0, 1.0)';
      this.canvas2DContext.stroke(this.yellowPath);
      this.canvas2DContext.strokeStyle = 'rgba(0, 255, 0, 1.0)';
      this.canvas2DContext.stroke(this.greenPath);

      this.scene.visualizationChanged = false;

      // console.log(performance.now() - time);

      /* if (this.battleStorageService.availableActionSquares?.length > 0) {
        this.generateActionSquareGrid(this.battleStorageService.currentActionId ? redPath : yellowPath, cameraLeft, cameraTop);
      }

      this.canvas2DContext.lineWidth = 1;
      for (const text of this.battleStorageService.floatingTexts) {
        if (text.time >= 0) {
          const x = (text.x + 0.5 - cameraLeft) * this.tileWidth;
          const y = (text.y - cameraTop) * this.tileHeight - text.height;
          this.canvas2DContext.fillStyle = `rgba(${text.color.r}, ${text.color.g},
            ${text.color.b}, ${text.color.a})`;
          this.canvas2DContext.strokeStyle = `rgba(0, 8, 24, ${text.color.a})`;
          this.canvas2DContext.fillText(text.text, x, y);
          this.canvas2DContext.strokeText(text.text, x, y);
        }
      }

      if (!this.loaded) {
        this.loaded = true;
        if (this.battleStorageService.version > 1) {
          this.finishLoadingSubscription = this.loadingService.finishLoading()
            .subscribe(() => {
              this.loadingFinished = true;
              this.processNextActionFromQueue();
            });
         }
      }*/
    }
  }

}
