import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CharsService {

  private charsSubCanvas: HTMLCanvasElement;
  private context: CanvasRenderingContext2D;
  private chars: { [id: string]: { x: number, y: number } };

  readonly spriteHeight = 60;
  readonly spriteWidth = 36;

  get width() {
    return this.charsSubCanvas.width;
  }

  constructor() {
    this.charsSubCanvas = document.createElement('canvas');
    this.charsSubCanvas.height = this.spriteHeight;
    this.chars = {};
    const spriteFunctions = this.drawAllSprites();
    const elements = Object.keys(this.chars).length;
    this.charsSubCanvas.width = this.spriteWidth * elements;
    this.context = this.charsSubCanvas.getContext('2d');
    this.context.clearRect(0, 0, this.spriteWidth * elements, this.spriteHeight);
    this.context.font = `${this.spriteHeight}px PT Mono`;
    this.context.textAlign = 'left';
    this.context.fillStyle = 'rgba(255,0,0,255)';
    spriteFunctions.forEach(element => {
      element();
    });
  }

  private drawTextChar(id: string, text: string) {
    const elementNumber = Object.keys(this.chars).length ;
    const symbolY = this.spriteHeight * 0.75;
    this.chars[id] = { x: this.spriteWidth * elementNumber, y: 0 };
    return () => {
      this.context.fillText(text, this.spriteWidth * elementNumber, symbolY);
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
      this.drawTextChar('|', '|')
    ];
  }

  getTexture(gl: WebGLRenderingContext) {
    const texture = gl.createTexture();
    gl.bindTexture(gl.TEXTURE_2D, texture);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, this.charsSubCanvas);
    return texture;
  }

  getSpritePositionOnTexture(id: string) {
    return this.chars[id];
  }
}
