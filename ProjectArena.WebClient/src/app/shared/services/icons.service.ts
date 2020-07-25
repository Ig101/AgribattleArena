import { Injectable } from '@angular/core';
import { ITextureService } from '../interfaces/texture-service.interface';

@Injectable({
  providedIn: 'root'
})
export class IconsService implements ITextureService {

  private iconsSubCanvas: HTMLCanvasElement;
  private context: CanvasRenderingContext2D;
  private icons: { [id: string]: { x: number, y: number } };
  private loaded = false;

  readonly spriteHeight = 30;
  readonly spriteWidth = 18;

  get width() {
    return this.iconsSubCanvas.width;
  }

  constructor() { }

  private loadIfNotLoaded() {
    if (!this.loaded) {
      this.loaded = true;
      this.iconsSubCanvas = document.createElement('canvas');
      this.iconsSubCanvas.height = this.spriteHeight;
      this.icons = {};
      this.icons[' '] = { x: 0, y: 0 };
      const spriteFunctions = this.drawAllSprites();
      const elements = Object.keys(this.icons).length;
      this.iconsSubCanvas.width = (this.spriteWidth + 2) * elements;
      this.context = this.iconsSubCanvas.getContext('2d');
      this.context.clearRect(0, 0, (this.spriteWidth + 2) * elements, this.spriteHeight);
      this.context.fillStyle = 'rgba(255,0,0,255)';
      this.context.fillRect(0, 0, this.spriteWidth + 2, this.spriteHeight);
      this.context.font = `${this.spriteHeight}px PT Mono`;
      this.context.textAlign = 'left';
      this.context.fillStyle = 'rgba(255,0,0,255)';
      spriteFunctions.forEach(e => {
        e();
      });
    }
  }

  getTexture(gl: WebGLRenderingContext) {
    this.loadIfNotLoaded();
    const texture = gl.createTexture();
    gl.bindTexture(gl.TEXTURE_2D, texture);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, this.iconsSubCanvas);
    return texture;
  }

  getSpritePositionOnTexture(id: string) {
    this.loadIfNotLoaded();
    return this.icons[id];
  }

  getTileMask(left: boolean, right: boolean, top: boolean, bottom: boolean) {
    this.loadIfNotLoaded();
    return this.icons[' '];
  }

  private drawTalentIcon(id: string, text: string) {
    const elementNumber = Object.keys(this.icons).length ;
    const symbolY = (this.spriteHeight - 4) * 0.75 + 2;
    this.icons['t' + id] = { x: (this.spriteWidth + 2) * elementNumber, y: 0 };
    return () => {
      this.context.font = `${this.spriteHeight - 4}px PT Mono`;
      this.context.fillText(text, (this.spriteWidth + 2) * elementNumber + 1, symbolY);
    };
  }

  private drawAllSprites() {
    return [
      this.drawTalentIcon('c', 'c'),
      this.drawTalentIcon('p', 'p'),
      this.drawTalentIcon('m', 'm'),
      this.drawTalentIcon('f', 'f'),
      this.drawTalentIcon('w', 'w'),
      this.drawTalentIcon('z', 'z'),
      this.drawTalentIcon('q', 'q'),
      this.drawTalentIcon('d', 'd'),
      this.drawTalentIcon('a', 'a'),
      this.drawTalentIcon('e', 'e'),
      this.drawTalentIcon('t', 't'),
      this.drawTalentIcon('r', 'r'),
      this.drawTalentIcon('C', 'C'),
      this.drawTalentIcon('S', 'S'),
      this.drawTalentIcon('W', 'W'),
      this.drawTalentIcon('I', 'I'),
      this.drawTalentIcon('E', 'E'),
      this.drawTalentIcon('B', 'B'),
      this.drawTalentIcon('R', 'R'),
      this.drawTalentIcon('A', 'A'),
      this.drawTalentIcon('F', 'F'),
      this.drawTalentIcon('M', 'M'),
      this.drawTalentIcon('P', 'P'),
      this.drawTalentIcon('O', 'O'),
    ];
  }
}
