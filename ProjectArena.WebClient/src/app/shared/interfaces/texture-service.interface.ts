export interface ITextureService {
  readonly spriteHeight: number;
  readonly spriteWidth: number;
  readonly width: number;
  getTexture(gl: WebGLRenderingContext): WebGLTexture;
  getSpritePositionOnTexture(id: string): { x: number; y: number; };
  getTileMask(left: boolean, right: boolean, top: boolean, bottom: boolean): { x: number; y: number; };
}
