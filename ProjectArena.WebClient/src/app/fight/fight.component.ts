import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
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
import { Visualization } from './models/visualization.model';
import { DEFAULT_HEIGHT } from '../content/content.helper';

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

  blocked = false;

  private tileWidthInternal = 0;
  private tileHeightInternal = 60;
  readonly defaultWidth = 1600;
  readonly defaultHeight = 1080;
  readonly defaultAspectRatio = this.defaultWidth / this.defaultHeight;

  zoom = 0;

  cameraX: number;
  cameraY: number;
  battleZoom: number;

  updateSubscription: Subscription;

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
  ) { }

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
    this.updateSubscription = this.sceneService.updateSub.subscribe(() => this.redraw());
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
          height: x > 4 && y > 4 ? 900 : 500,
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
      tags: ['active', 'intelligent'],
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

  onResize() {
    this.setupAspectRatio(this.battleCanvas.nativeElement.offsetWidth, this.battleCanvas.nativeElement.offsetHeight);
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
    this.redraw();
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
        if (actor.tags.includes('small')) {
          multiActor = true;
        } else {
          visibleActor = actor;
          multiActor = false;
        }
      }
    }
    return { backgroundActor, visibleActor, multiActor };
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
    drawChar: Visualization,

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
        const color = heightImpact(currentTileHeight, info.backgroundActor.color);
        fillBackground(
          backgrounds,
          color.r / 5,
          color.g / 5,
          color.b / 5,
          texturePosition);
      } else {
        fillBackground(backgrounds, 0, 0, 0, texturePosition);
      }

      if (drawChar) {
        fillColor(colors, drawChar.color.r, drawChar.color.g, drawChar.color.b, drawChar.color.a, texturePosition);
        fillChar(this.charsService, textureMapping, drawChar.char, texturePosition);
      } /*else if (info.multiActor) {

      }*/ else {
        let color: Color;
        let char: string;
        let mirrored: boolean;
        if (!info.visibleActor) {
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
      }
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

  redraw() {
    if (this.scene && this.shadersProgram) {
      const time = performance.now();
      const sceneRandom = new Random(this.scene.hash);
      const cameraLeft = this.cameraX - (this.canvasWidth - this.interfaceShift) / 2 / this.tileWidth;
      const cameraTop = this.cameraY - this.canvasHeight / 2 / this.tileHeight;
      const left = Math.floor(cameraLeft) - 1;
      const right = Math.ceil(cameraLeft + this.canvasWidth / (this.tileWidth)) + 1;
      const top = Math.floor(cameraTop) - 1;
      const bottom = Math.ceil(cameraTop + this.canvasHeight / (this.tileHeight)) + 1;
      /*const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);
      const currentActionSquare = this.canAct ? this.battleStorageService.availableActionSquares
        ?.find(s => s.x === mouseX && s.y === mouseY && s.type) : undefined;*/
      const redPath = new Path2D();
      const yellowPath = new Path2D();
      const greenPath = new Path2D();
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
          if (x >= left && y >= top && x <= right && y <= bottom) {
            fillVertexPosition(mainTextureVertexes, x, y, left, top, this.tileWidth, this.tileHeight, texturePosition);
            if (x >= 0 && y >= 0 && x < this.scene.width && y < this.scene.height) {
              sceneRandom.next();
              this.drawPoint(x, y, texturePosition, undefined, cameraLeft, cameraTop,
                greenPath, yellowPath, redPath, colors, textureMapping, backgrounds, backgroundTextureMapping);
            } else {
              const biom = getRandomBiom(sceneRandom, this.scene.biom);
              this.drawDummyPoint(biom.char, biom.color, texturePosition,
                colors, textureMapping, backgrounds, backgroundTextureMapping);
            }
            texturePosition++;
          } else {
            sceneRandom.next();
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
        (right - left + 1) * this.tileWidth,
        (bottom - top + 1) * this.tileHeight,
        (right - left + 1),
        (bottom - top + 1),
        this.charsService.width,
        this.charsService.spriteHeight);

      this.canvas2DContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
      this.canvas2DContext.lineWidth = 2;
      this.canvas2DContext.strokeStyle = 'rgba(255, 0, 0, 1.0)';
      this.canvas2DContext.stroke(redPath);
      this.canvas2DContext.strokeStyle = 'rgba(255, 255, 0, 1.0)';
      this.canvas2DContext.stroke(yellowPath);
      this.canvas2DContext.strokeStyle = 'rgba(0, 255, 0, 1.0)';
      this.canvas2DContext.stroke(greenPath);

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
