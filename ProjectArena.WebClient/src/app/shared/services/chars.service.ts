/*
  For chars:
  r-channel - brightness,
  g-channel - saturation
*/

import { Injectable } from '@angular/core';
import { element } from 'protractor';

@Injectable({
  providedIn: 'root'
})
export class CharsService {

  private charsSubCanvas: HTMLCanvasElement;
  private context: CanvasRenderingContext2D;
  private chars: { [id: string]: { x: number, y: number } };
  private loaded = false;

  readonly spriteHeight = 30;
  readonly spriteWidth = 18;

  get width() {
    return this.charsSubCanvas.width;
  }

  constructor() { }

  loadIfNotLoaded() {
    if (!this.loaded) {
      this.loaded = true;
      this.charsSubCanvas = document.createElement('canvas');
      this.charsSubCanvas.height = this.spriteHeight;
      this.chars = {};
      const tileSpriteFunctions = this.drawTileSprites();
      const spriteFunctions = this.drawAllSprites();
      const elements = Object.keys(this.chars).length;
      this.charsSubCanvas.width = (this.spriteWidth + 2) * elements;
      this.context = this.charsSubCanvas.getContext('2d');
      this.context.clearRect(0, 0, (this.spriteWidth + 2) * elements, this.spriteHeight);
      this.context.fillStyle = 'rgba(150,0,0,255)';
      this.context.strokeStyle = 'rgba(255,0,0,255)';
      this.context.lineWidth = 2;
      tileSpriteFunctions.forEach(e => {
        e();
      });
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
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, this.charsSubCanvas);
    return texture;
  }

  getTileMask(left: boolean, right: boolean, top: boolean, bottom: boolean) {
    this.loadIfNotLoaded();
    return this.chars[this.getTileName(left, right, top, bottom)];
  }

  getSpritePositionOnTexture(id: string) {
    this.loadIfNotLoaded();
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

  private drawTextChar(id: string, text: string) {
    const elementNumber = Object.keys(this.chars).length ;
    const symbolY = this.spriteHeight * 0.75;
    this.chars[id] = { x: (this.spriteWidth + 2) * elementNumber, y: 0 };
    return () => {
      this.context.fillText(text, (this.spriteWidth + 2) * elementNumber, symbolY);
    };
  }

  private drawTalentChar(id: string, text: string) {
    const elementNumber = Object.keys(this.chars).length ;
    const symbolY = (this.spriteHeight - 4) * 0.75 + 2;
    this.chars['t' + id] = { x: (this.spriteWidth + 2) * elementNumber, y: 0 };
    return () => {
      this.context.font = `${this.spriteHeight - 4}px PT Mono`;
      this.context.fillText(text, (this.spriteWidth + 2) * elementNumber + 1, symbolY);
    };
  }

  private drawAllSprites() {
    return [
      this.drawTextChar('x', 'x'),
      this.drawTextChar('.', '.'),
      this.drawTextChar('·', '·'),
      this.drawTextChar('-', '-'),
      this.drawTextChar('!', '!'),
      this.drawTextChar('@', '@'),
      this.drawTextChar('S', 'S'),
      this.drawTextChar('C', 'C'),
      this.drawTextChar('R', 'R'),
      this.drawTextChar('B', 'B'),
      this.drawTextChar('F', 'F'),
      this.drawTextChar('M', 'M'),
      this.drawTextChar('I', 'I'),
      this.drawTextChar('s', 's'),
      this.drawTextChar('o', 'o'),
      this.drawTextChar('#', '#'),
      this.drawTextChar('Y', 'Y'),
      this.drawTextChar('*', '*'),
      this.drawTextChar('■', '■'),
      this.drawTextChar('&', '&'),
      this.drawTextChar('\\', '\\'),
      this.drawTextChar('/', '/'),
      this.drawTextChar('X', 'X'),
      this.drawTextChar('+', '+'),
      this.drawTextChar('|', '|'),

      this.drawTalentChar('c', 'c'),
      this.drawTalentChar('p', 'p'),
      this.drawTalentChar('m', 'm'),
      this.drawTalentChar('f', 'f'),
      this.drawTalentChar('w', 'w'),
      this.drawTalentChar('z', 'z'),
      this.drawTalentChar('q', 'q'),
      this.drawTalentChar('d', 'd'),
      this.drawTalentChar('a', 'a'),
      this.drawTalentChar('e', 'e'),
      this.drawTalentChar('t', 't'),
      this.drawTalentChar('r', 'r'),
      this.drawTalentChar('C', 'C'),
      this.drawTalentChar('S', 'S'),
      this.drawTalentChar('W', 'W'),
      this.drawTalentChar('I', 'I'),
      this.drawTalentChar('E', 'E'),
      this.drawTalentChar('B', 'B'),
      this.drawTalentChar('R', 'R'),
      this.drawTalentChar('A', 'A'),
      this.drawTalentChar('F', 'F'),
      this.drawTalentChar('M', 'M'),
      this.drawTalentChar('P', 'P'),
      this.drawTalentChar('O', 'O'),
    ];
  }
}
