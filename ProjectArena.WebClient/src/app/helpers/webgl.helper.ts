export function fillBackground(backgrounds: number[], r: number, g: number, b: number, texturePosition: number) {
  backgrounds[texturePosition * 4] = r;
  backgrounds[texturePosition * 4 + 1] = g;
  backgrounds[texturePosition * 4 + 2] = b;
  backgrounds[texturePosition * 4 + 3] = 1.0;
}

export function fillColor(colors: number[], r: number, g: number, b: number, a: number, texturePosition: number) {
  colors[texturePosition * 4] = r;
  colors[texturePosition * 4 + 1] = g;
  colors[texturePosition * 4 + 2] = b;
  colors[texturePosition * 4 + 3] = a;
}

export function fillChar(textureMapping: number[], char: string, texturePosition: number) {
  const charPosition = this.charsService.getSpritePositionOnTexture(char);
  textureMapping[texturePosition * 12] = charPosition.x;
  textureMapping[texturePosition * 12 + 1] = charPosition.y;
  textureMapping[texturePosition * 12 + 2] = charPosition.x + this.charsService.spriteWidth;
  textureMapping[texturePosition * 12 + 3] = charPosition.y;
  textureMapping[texturePosition * 12 + 4] = charPosition.x;
  textureMapping[texturePosition * 12 + 5] = charPosition.y + this.charsService.spriteHeight;
  textureMapping[texturePosition * 12 + 6] = charPosition.x;
  textureMapping[texturePosition * 12 + 7] = charPosition.y + this.charsService.spriteHeight;
  textureMapping[texturePosition * 12 + 8] = charPosition.x + this.charsService.spriteWidth;
  textureMapping[texturePosition * 12 + 9] = charPosition.y;
  textureMapping[texturePosition * 12 + 10] = charPosition.x + this.charsService.spriteWidth;
  textureMapping[texturePosition * 12 + 11] = charPosition.y + this.charsService.spriteHeight;
}

export function fillVertexPosition(
  vertexPositions: number[],
  x: number,
  y: number,
  cameraLeft: number,
  cameraTop: number,
  tileWidth: number,
  tileHeight: number,
  texturePosition: number
) {
  const canvasX = Math.round((x - cameraLeft) * tileWidth);
  const canvasY = Math.round((y - cameraTop) * tileHeight);
  vertexPositions[texturePosition * 12] = canvasX;
  vertexPositions[texturePosition * 12 + 1] = canvasY;
  vertexPositions[texturePosition * 12 + 2] = canvasX + tileWidth;
  vertexPositions[texturePosition * 12 + 3] = canvasY;
  vertexPositions[texturePosition * 12 + 4] = canvasX;
  vertexPositions[texturePosition * 12 + 5] = canvasY + tileHeight;
  vertexPositions[texturePosition * 12 + 6] = canvasX;
  vertexPositions[texturePosition * 12 + 7] = canvasY + tileHeight;
  vertexPositions[texturePosition * 12 + 8] = canvasX + tileWidth;
  vertexPositions[texturePosition * 12 + 9] = canvasY;
  vertexPositions[texturePosition * 12 + 10] = canvasX + tileWidth;
  vertexPositions[texturePosition * 12 + 11] = canvasY + tileHeight;
}