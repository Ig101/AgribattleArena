/*
  For chars:
  r-channel - brightness,
  g-channel - saturation
*/

import { Injectable } from '@angular/core';
import { element } from 'protractor';
import { ITextureService } from '../interfaces/texture-service.interface';

@Injectable({
  providedIn: 'root'
})
export class CharsService implements ITextureService {

  private charsSubCanvas: HTMLCanvasElement;
  private context: CanvasRenderingContext2D;
  private chars: { [id: string]: { x: number, y: number } };
  private loaded = false;

  readonly spriteHeight = 30;
  readonly spriteWidth = 18;

  get width() {
    return this.charsSubCanvas.width;
  }

  constructor() {
    this.loaded = true;
    this.charsSubCanvas = document.createElement('canvas');
    document.body.appendChild(this.charsSubCanvas);
    this.charsSubCanvas.height = this.spriteHeight;
    this.chars = {};
    this.chars[' '] = { x: 0, y: 0 };
    const tileSpriteFunctions = this.drawTileSprites();
    const spriteFunctions = this.drawAllSprites();
    const elements = Object.keys(this.chars).length;
    this.charsSubCanvas.width = (this.spriteWidth + 2) * elements;
    this.context = this.charsSubCanvas.getContext('2d');
    this.context.clearRect(0, 0, (this.spriteWidth + 2) * elements, this.spriteHeight);
    this.context.fillStyle = 'rgba(150,0,0,255)';
    this.context.strokeStyle = 'rgba(255,0,0,255)';
    this.context.lineWidth = 2;
    for (const e of tileSpriteFunctions) {
      e();
    }
    this.context.fillStyle = 'rgba(255,0,0,255)';
    this.context.fillRect(0, 0, this.spriteWidth + 2, this.spriteHeight);
    this.context.font = `${this.spriteHeight}px PT Mono`;
    this.context.textAlign = 'left';
    this.context.fillStyle = 'rgba(255,0,0,255)';
    for (const e of spriteFunctions) {
      e();
    }
    this.loaded = true;
  }

  getTexture(gl: WebGLRenderingContext) {
    while (!this.loaded) {

    }
    const texture = gl.createTexture();
    gl.bindTexture(gl.TEXTURE_2D, texture);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, this.charsSubCanvas);
    return texture;
  }

  getTileMask(left: boolean, right: boolean, top: boolean, bottom: boolean) {
    while (!this.loaded) {

    }
    return this.chars[this.getTileName(left, right, top, bottom)];
  }

  getSpritePositionOnTexture(id: string) {
    while (!this.loaded) {

    }
    return this.chars[id];
  }

  private getTileName(left: boolean, right: boolean, top: boolean, bottom: boolean) {
    if (!left && !right && !top && !bottom) { return 'tile-no'; }
    if (left && !right && !top && !bottom) { return 'tile-lef'; }
    if (!left && right && !top && !bottom) { return 'tile-rig'; }
    if (!left && !right && top && !bottom) { return 'tile-top'; }
    if (!left && !right && !top && bottom) { return 'tile-bot'; }
    if (left && !right && top && !bottom) { return 'tile-top-lef'; }
    if (!left && right && top && !bottom) { return 'tile-top-rig'; }
    if (left && !right && !top && bottom) { return 'tile-bot-lef'; }
    if (!left && right && !top && bottom) { return 'tile-bot-rig'; }
    if (left && right && !top && !bottom) { return 'tile-lef-rig'; }
    if (!left && !right && top && bottom) { return 'tile-top-bot'; }
    if (left && !right && top && bottom) { return 'tile-top-lef-bot'; }
    if (!left && right && top && bottom) { return 'tile-top-rig-bot'; }
    if (left && right && top && !bottom) { return 'tile-lef-bot-rig'; }
    if (left && right && top && !bottom) { return 'tile-lef-top-rig'; }
    if (left && right && top && bottom) { return 'tile-all'; }
  }

  private drawTileBound(left: boolean, right: boolean, top: boolean, bottom: boolean) {
    const elementNumber = Object.keys(this.chars).length ;
    this.chars[this.getTileName(left, right, top, bottom)] = { x: (this.spriteWidth + 2) * elementNumber, y: 0 };
    return () => {
      this.context.fillRect((this.spriteWidth + 2) * elementNumber, 0, this.spriteWidth + 2, this.spriteHeight);
      const path = new Path2D();
      if (left) {
        path.moveTo((this.spriteWidth + 2) * elementNumber + 1, 0);
        path.lineTo((this.spriteWidth + 2) * elementNumber + 1, this.spriteHeight);
      }
      if (right) {
        path.moveTo((this.spriteWidth + 2) * (elementNumber + 1) - 3, 0);
        path.lineTo((this.spriteWidth + 2) * (elementNumber + 1) - 3, this.spriteHeight);
      }
      if (top) {
        path.moveTo((this.spriteWidth + 2) * elementNumber - 1, 1);
        path.lineTo((this.spriteWidth + 2) * (elementNumber + 1) - 1, 1);
      }
      if (bottom) {
        path.moveTo((this.spriteWidth + 2) * elementNumber - 1, this.spriteHeight - 1);
        path.lineTo((this.spriteWidth + 2) * (elementNumber + 1) - 1, this.spriteHeight - 1);
      }
      this.context.stroke(path);
    };
  }

  private drawTileSprites(): (() => void)[] {
    return [
      this.drawTileBound(false, false, false, false),
      this.drawTileBound(true, false, false, false),
      this.drawTileBound(false, true, false, false),
      this.drawTileBound(false, false, true, false),
      this.drawTileBound(false, false, false, true),
      this.drawTileBound(true, true, false, false),
      this.drawTileBound(false, true, true, false),
      this.drawTileBound(false, false, true, true),
      this.drawTileBound(false, true, false, true),
      this.drawTileBound(true, false, true, false),
      this.drawTileBound(true, false, false, true),
      this.drawTileBound(false, true, true, true),
      this.drawTileBound(true, false, true, true),
      this.drawTileBound(true, true, false, true),
      this.drawTileBound(true, true, true, false),
      this.drawTileBound(true, true, true, true),
    ];
  }

  private drawImageChar(id: string, src: string) {
    const elementNumber = Object.keys(this.chars).length ;
    this.chars[id] = { x: (this.spriteWidth + 2) * elementNumber, y: 0 };
    return () => {
      const img = new Image();
      img.onload = () => {
          this.context.drawImage(img, (this.spriteWidth + 2) * elementNumber, 0);
      };
      img.src = src;
    };
  }

  private drawTextChar(id: string, text: string) {
    const elementNumber = Object.keys(this.chars).length ;
    const symbolY = this.spriteHeight * 0.75;
    this.chars[id] = { x: (this.spriteWidth + 2) * elementNumber, y: 0 };
    return () => {
      this.context.fillText(text, (this.spriteWidth + 2) * elementNumber, symbolY);
    };
  }

  private drawAllSprites() {
    return [
      this.drawImageChar('adventurer', 'assets/actors/adventurer.png'),

      this.drawImageChar('grass', 'assets/objects/grass.png'),
      this.drawImageChar('ground', 'assets/objects/ground.png'),
      this.drawImageChar('rock', 'assets/objects/rock.png'),
      this.drawImageChar('tree', 'assets/objects/tree.png'),

      this.drawImageChar('floor', 'assets/objects/floor.png'),
      this.drawImageChar('table', 'assets/objects/table.png'),
      this.drawImageChar('wood-wall', 'assets/objects/wood-wall.png'),
      this.drawImageChar('bar-corner', 'assets/objects/bar-corner.png'),
      this.drawImageChar('bar-vertical', 'assets/objects/bar-vertical.png'),
      this.drawImageChar('bar-horizontal', 'assets/objects/bar-horizontal.png'),
      this.drawTextChar('x', 'x'),
      this.drawTextChar('.', '.'),
      this.drawTextChar('·', '·'),
      this.drawTextChar('-', '-'),
      this.drawTextChar('!', '!'),
      this.drawTextChar('s', 's'),
      this.drawTextChar('o', 'o'),
      this.drawTextChar('#', '#'),
      this.drawTextChar('*', '*'),
      this.drawTextChar('&', '&'),
      this.drawTextChar('\\', '\\'),
      this.drawTextChar('/', '/'),
      this.drawTextChar('X', 'X'),
      this.drawTextChar('+', '+'),
      this.drawTextChar('|', '|')
    ];
  }
}
