export interface ITextureService {
  readonly spriteHeight;
  readonly spriteWidth;
  getTexture(gl: WebGLRenderingContext);
  getSpritePositionOnTexture(id: string);
  getTileMask(left: boolean, right: boolean, top: boolean, bottom: boolean);
}
