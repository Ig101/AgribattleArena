import { Component, OnInit, Input, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { IconDefinition } from '../models/icon-definition.model';
import { drawArrays, fillBackground, fillTileMask, fillVertexPosition, fillColor, fillChar } from 'src/app/helpers/webgl.helper';
import { Color } from '../models/color.model';
import { AssetsLoadingService } from '../services/assets-loading.service';
import { ITextureService } from '../interfaces/texture-service.interface';

@Component({
  selector: 'app-texture-icon',
  templateUrl: './texture-icon.component.html',
  styleUrls: ['./texture-icon.component.scss']
})
export class TextureIconComponent implements OnInit, OnDestroy {

  @Input() set definition(value: IconDefinition) {
    if (this.context && value) {
      this.redrawIcon(value);
    }
    this.tempDefinition = value;
  }
  @Input() textureService: ITextureService;

  private texture: WebGLTexture;
  private program: WebGLProgram;

  tempDefinition: IconDefinition;

  @ViewChild('iconCanvas', { static: true }) iconCanvas: ElementRef<HTMLCanvasElement>;
  context: WebGLRenderingContext;

  constructor(
    private assetsLoadingService: AssetsLoadingService
  ) { }

  ngOnDestroy() {
    this.context.getExtension('WEBGL_lose_context').loseContext();
  }

  ngOnInit(): void {
    this.context = this.iconCanvas.nativeElement.getContext('webgl');
    this.assetsLoadingService.loadShadersAndCreateProgram(
      this.context,
      'vertex-shader-2d.vert',
      'fragment-shader-2d.frag'
    )
      .subscribe((result) => {
        this.texture = this.textureService.getTexture(this.context);
        this.program = result;
        if (this.tempDefinition) {
          this.redrawIcon(this.tempDefinition);
        }
      });
  }

  redrawIcon(definition: IconDefinition) {
    const width = this.iconCanvas.nativeElement.width;
    const height = this.iconCanvas.nativeElement.height;
    const textureMapping: Float32Array = new Float32Array(12);
    const colors: Uint8Array = new Uint8Array(4);
    const activities: Uint8Array = new Uint8Array(1);
    const backgroundTextureMapping: Float32Array = new Float32Array(12);
    const backgrounds: Uint8Array = new Uint8Array(4);
    const mainTextureVertexes: Float32Array = new Float32Array(12);
    fillBackground(backgrounds, 0, 8, 24, 0);
    fillChar(this.textureService, backgroundTextureMapping, ' ', 0);
    fillVertexPosition(mainTextureVertexes, 0, 0, 0, 0, width, height, 0);
    fillColor(colors, activities, definition.color.r, definition.color.g, definition.color.b, definition.color.a, false, 0);
    fillChar(this.textureService, textureMapping, definition.char, 0);
    this.context.clearColor(0, 0, 0, 0);
    drawArrays(
      this.context,
      this.program,
      mainTextureVertexes,
      colors,
      backgrounds,
      textureMapping,
      backgroundTextureMapping,
      activities,
      this.texture,
      0,
      0,
      width,
      height,
      1,
      1,
      this.textureService.width,
      this.textureService.spriteHeight);
  }
}
