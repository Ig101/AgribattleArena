import { Component, OnInit, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { LoadingService } from '../shared/services/loading.service';
import { LoadingScene } from '../shared/models/loading/loading-scene.model';
import { CharsService } from '../shared/services/chars.service';
import { AssetsLoadingService } from '../shared/services/assets-loading.service';
import { drawArrays, fillTileMask, fillBackground, fillColor, fillChar, fillVertexPosition } from '../helpers/webgl.helper';

@Component({
  selector: 'app-loading-screen',
  templateUrl: './loading-screen.component.html',
  styleUrls: ['./loading-screen.component.scss']
})
export class LoadingScreenComponent implements OnInit, OnDestroy {

  @ViewChild('loadingCanvas', { static: true }) battleCanvas: ElementRef<HTMLCanvasElement>;
  private canvasWebGLContext: WebGLRenderingContext;

  @ViewChild('hudCanvas', { static: true }) hudCanvas: ElementRef<HTMLCanvasElement>;
  private canvas2DContext: CanvasRenderingContext2D;
  private charsTexture: WebGLTexture;
  private shadersProgram: WebGLProgram;

  readonly defaultWidth = 1600;
  readonly defaultHeight = 1080;
  readonly defaultAspectRatio = this.defaultWidth / this.defaultHeight;

  private updateTimer;
  private updateFrequency = 30;

  get canvasWidth() {
    return this.battleCanvas.nativeElement.width;
  }

  get canvasHeight() {
    return this.battleCanvas.nativeElement.height;
  }

  zoom = 0;

  get alpha() {
    return this.loadingService.alpha;
  }

  constructor(
    private loadingService: LoadingService,
    private charsService: CharsService,
    private assetsLoadingService: AssetsLoadingService
  ) { }
  ngOnInit(): void {
    this.canvas2DContext = this.hudCanvas.nativeElement.getContext('2d');
    this.canvasWebGLContext = this.battleCanvas.nativeElement.getContext('webgl');
    this.setupAspectRatio(this.battleCanvas.nativeElement.offsetWidth, this.battleCanvas.nativeElement.offsetHeight);
    this.loadingService.setupTime();
    this.updateTimer = setInterval(() => {
      this.loadingService.loadingUpdate();
      this.redraw();
    }, 1000 / this.updateFrequency);
    this.assetsLoadingService.loadShadersAndCreateProgram(
      this.canvasWebGLContext,
      'vertex-shader-2d.fx',
      'fragment-shader-2d.fx'
    )
      .subscribe((result) => {
        this.charsTexture = this.charsService.getTexture(this.canvasWebGLContext);
        this.shadersProgram = result;
        this.loadingService.changed = true;
        this.redraw();
      });
  }

  ngOnDestroy(): void {
    clearInterval(this.updateTimer);
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
    this.loadingService.changed = true;
    this.redraw();
  }

  private drawPoint(
    scene: LoadingScene,
    x: number,
    y: number,
    tileHeight: number,
    tileWidth: number,
    texturePosition: number,
    colors: Uint8Array,
    textureMapping: Float32Array,
    backgrounds: Uint8Array,
    backgroundTextureMapping: Float32Array) {

    const tile = scene.tiles[x][y];
    if (tile) {
      fillTileMask(
        this.charsService,
        backgroundTextureMapping,
        x > 0 && tile.height - scene.tiles[x - 1][y].height >= 10,
        x < scene.width - 1 && tile.height - scene.tiles[x + 1][y].height >= 10,
        y > 0 && tile.height - scene.tiles[x][y - 1].height >= 10,
        y < scene.height - 1 && tile.height - scene.tiles[x][y + 1].height >= 10,
        texturePosition);
      if (tile.backgroundColor) {
        fillBackground(backgrounds, tile.backgroundColor.r, tile.backgroundColor.g, tile.backgroundColor.b, texturePosition);
      } else {
        fillBackground(backgrounds, 0, 0, 0, texturePosition);
      }
      fillColor(colors, tile.color.r, tile.color.g, tile.color.b, tile.color.a, texturePosition);
      fillChar(this.charsService, textureMapping, tile.char, texturePosition);
    }
  }


  redraw() {
    if (!this.loadingService.changed) {
      return;
    }
    const definition = this.loadingService.definition;
    const titleHeight = 30;
    let titlePosition = this.canvasHeight / 2 + titleHeight * 0.375;
    if (definition.loadingScene) {
      titlePosition = this.canvasHeight / 2 - 220;
    }
    this.canvas2DContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
    if (definition.loadingScene && this.shadersProgram) {
      this.canvasWebGLContext.clearColor(0, 0, 0, 0);
      const sceneWidth = 1300;
      const sceneHeight = 680;
      let tileHeight = 30;
      let tileWidth = 0.6 * tileHeight;
      const sceneWidthCoefficient = sceneWidth / (tileWidth * definition.loadingScene.width);
      const sceneHeightCoefficient = sceneHeight / (tileHeight * definition.loadingScene.height);
      if (sceneHeightCoefficient <= sceneWidthCoefficient && sceneHeightCoefficient < 1) {
        tileWidth *= sceneHeightCoefficient;
        tileHeight *= sceneHeightCoefficient;
      }
      if (sceneWidthCoefficient < sceneHeightCoefficient && sceneWidthCoefficient < 1) {
        tileWidth *= sceneWidthCoefficient;
        tileHeight *= sceneWidthCoefficient;
      }
      const cameraLeft = definition.loadingScene.width / 2 - this.canvasWidth / 2 / tileWidth;
      const cameraTop = definition.loadingScene.height / 2 - this.canvasHeight / 2 / tileHeight;
      const textureMapping: Float32Array = new Float32Array(definition.loadingScene.width * definition.loadingScene.height * 12);
      const colors: Uint8Array = new Uint8Array(definition.loadingScene.width * definition.loadingScene.height * 4);
      const backgroundTextureMapping: Float32Array = new Float32Array(definition.loadingScene.width * definition.loadingScene.height * 12);
      const backgrounds: Uint8Array = new Uint8Array(definition.loadingScene.width * definition.loadingScene.height * 4);
      const mainTextureVertexes: Float32Array = new Float32Array(definition.loadingScene.width * definition.loadingScene.height * 12);
      let texturePosition = 0;
      for (let y = 0; y < definition.loadingScene.height; y++ ) {
        for (let x = 0; x < definition.loadingScene.width; x++) {
          fillVertexPosition(mainTextureVertexes, x, y, 0, 0, tileWidth, tileHeight, texturePosition);
          this.drawPoint(definition.loadingScene, x, y, tileHeight, tileWidth, texturePosition,
            colors, textureMapping, backgrounds, backgroundTextureMapping);
          texturePosition++;
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
        Math.round((0 - cameraLeft) * tileWidth),
        Math.round((0 - cameraTop) * tileHeight) - 140,
        definition.loadingScene.width * tileWidth,
        definition.loadingScene.height * tileHeight,
        definition.loadingScene.width,
        definition.loadingScene.height,
        this.charsService.width,
        this.charsService.spriteHeight);
    }
    if (definition.title) {
      this.canvas2DContext.font = `${titleHeight}px PT Mono`;
      this.canvas2DContext.fillStyle = `#ffffff`;
      this.canvas2DContext.textAlign = 'center';
      this.canvas2DContext.fillText(definition.title, this.canvasWidth / 2, titlePosition);
    }
  }

}
