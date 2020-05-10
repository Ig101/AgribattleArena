import { Component, OnInit, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { LoadingService } from '../shared/services/loading.service';
import { LoadingScene } from '../shared/models/loading/loading-scene.model';

@Component({
  selector: 'app-loading-screen',
  templateUrl: './loading-screen.component.html',
  styleUrls: ['./loading-screen.component.scss']
})
export class LoadingScreenComponent implements OnInit, OnDestroy {

  @ViewChild('loadingCanvas', { static: true }) battleCanvas: ElementRef<HTMLCanvasElement>;
  private canvasContext: CanvasRenderingContext2D;

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
    private loadingService: LoadingService
  ) { }
  ngOnInit(): void {
    this.canvasContext = this.battleCanvas.nativeElement.getContext('2d');
    this.setupAspectRatio(this.battleCanvas.nativeElement.offsetWidth, this.battleCanvas.nativeElement.offsetHeight);
    this.loadingService.setupTime();
    this.loadingService.changed = true;
    this.redraw();
    this.updateTimer = setInterval(() => {
      this.loadingService.loadingUpdate();
      this.redraw();
    }, 1000 / this.updateFrequency);
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
    this.canvasContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
    this.loadingService.changed = true;
    this.redraw();
  }

  private drawPoint(
    scene: LoadingScene,
    x: number, y: number,
    cameraLeft: number,
    cameraTop: number,
    tileHeight: number,
    tileWidth: number) {
    const tile = scene.tiles[x][y];
    if (tile) {
      const canvasX = (x - cameraLeft) * tileWidth;
      const canvasY = (y - cameraTop) * tileHeight + 140;
      const symbolY = canvasY + tileHeight * 0.75;
      if (tile.backgroundColor) {
        this.canvasContext.fillStyle = `rgb(${tile.backgroundColor.r}, ${tile.backgroundColor.g}, ${tile.backgroundColor.b})`;
        this.canvasContext.fillRect(canvasX, canvasY, tileWidth + 1, tileHeight + 1);
      }
      this.canvasContext.fillStyle = `rgba(${tile.color.r}, ${tile.color.g},
        ${tile.color.b}, ${tile.color.a})`;
      this.canvasContext.fillText(tile.char, canvasX, symbolY);
    }
  }


  redraw() {
    if (!this.loadingService.changed) {
      return;
    }
    this.loadingService.changed = false;
    this.canvasContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
    const definition = this.loadingService.definition;
    const titleHeight = 30;
    let titlePosition = this.canvasHeight / 2 + titleHeight * 0.375;
    if (definition.loadingScene) {
      titlePosition = this.canvasHeight / 2 - 220;
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
      this.canvasContext.font = `${tileHeight}px PT Mono`;
      this.canvasContext.fillStyle = `#ffffff`;
      this.canvasContext.textAlign = 'left';
      for (let x = 0; x < definition.loadingScene.width; x++) {
        for (let y = 0; y < definition.loadingScene.height; y++ ) {
          this.drawPoint(definition.loadingScene, x, y, cameraLeft, cameraTop, tileHeight, tileWidth);
        }
      }
    }
    if (definition.title) {
      this.canvasContext.font = `${titleHeight}px PT Mono`;
      this.canvasContext.fillStyle = `#ffffff`;
      this.canvasContext.textAlign = 'center';
      this.canvasContext.fillText(definition.title, this.canvasWidth / 2, titlePosition);
    }
  }

}
