import { CharsService } from '../shared/services/chars.service';

export function fillBackground(backgrounds: Uint8Array, r: number, g: number, b: number, texturePosition: number) {
  backgrounds[texturePosition * 4] = r;
  backgrounds[texturePosition * 4 + 1] = g;
  backgrounds[texturePosition * 4 + 2] = b;
  backgrounds[texturePosition * 4 + 3] = 1;
}

export function fillColor(colors: Uint8Array, r: number, g: number, b: number, a: number, texturePosition: number) {
  colors[texturePosition * 4] = r;
  colors[texturePosition * 4 + 1] = g;
  colors[texturePosition * 4 + 2] = b;
  colors[texturePosition * 4 + 3] = a;
}

export function fillChar(charsService: CharsService, textureMapping: Float32Array, char: string, texturePosition: number) {
  const charPosition = charsService.getSpritePositionOnTexture(char);
  textureMapping[texturePosition * 12] = charPosition.x;
  textureMapping[texturePosition * 12 + 1] = charPosition.y;
  textureMapping[texturePosition * 12 + 2] = charPosition.x + charsService.spriteWidth;
  textureMapping[texturePosition * 12 + 3] = charPosition.y;
  textureMapping[texturePosition * 12 + 4] = charPosition.x;
  textureMapping[texturePosition * 12 + 5] = charPosition.y + charsService.spriteHeight;
  textureMapping[texturePosition * 12 + 6] = charPosition.x;
  textureMapping[texturePosition * 12 + 7] = charPosition.y + charsService.spriteHeight;
  textureMapping[texturePosition * 12 + 8] = charPosition.x + charsService.spriteWidth;
  textureMapping[texturePosition * 12 + 9] = charPosition.y;
  textureMapping[texturePosition * 12 + 10] = charPosition.x + charsService.spriteWidth;
  textureMapping[texturePosition * 12 + 11] = charPosition.y + charsService.spriteHeight;
}

export function fillVertexPosition(
  vertexPositions: Float32Array,
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

export function drawArrays(
  gl: WebGLRenderingContext,
  program: WebGLProgram,

  vertexPositions: Float32Array,
  colors: Uint8Array,
  backgrounds: Uint8Array,
  textureMapping: Float32Array,
  texture: WebGLTexture,
  cameraX: number,
  cameraY: number,
  width: number,
  height: number,
  colorsWidth: number,
  colorsHeight: number,
  textureWidth: number,
  textureHeight: number
) {

  const positionLocation = gl.getAttribLocation(program, 'a_position');
  const texcoordLocation = gl.getAttribLocation(program, 'a_texCoord');

  const positionBuffer = gl.createBuffer();
  gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
  gl.bufferData(gl.ARRAY_BUFFER, vertexPositions, gl.STATIC_DRAW);

  const texcoordBuffer = gl.createBuffer();
  gl.bindBuffer(gl.ARRAY_BUFFER, texcoordBuffer);
  gl.bufferData(gl.ARRAY_BUFFER, textureMapping, gl.STATIC_DRAW);

  const colorTexture = gl.createTexture();
  gl.bindTexture(gl.TEXTURE_2D, colorTexture);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, colorsWidth, colorsHeight, 0, gl.RGBA, gl.UNSIGNED_BYTE, colors);

  const backgroundTexture = gl.createTexture();
  gl.bindTexture(gl.TEXTURE_2D, backgroundTexture);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, colorsWidth, colorsHeight, 0, gl.RGBA, gl.UNSIGNED_BYTE, backgrounds);

  const positionResolutionLocation = gl.getUniformLocation(program, 'u_positionResolution');
  const textureResolutionLocation = gl.getUniformLocation(program, 'u_textureResolution');

  gl.viewport(cameraX, cameraY, width, height);
  gl.useProgram(program);

  gl.enableVertexAttribArray(positionLocation);
  gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
  gl.vertexAttribPointer(positionLocation, 2, gl.FLOAT, false, 0, 0);

  gl.enableVertexAttribArray(texcoordLocation);
  gl.bindBuffer(gl.ARRAY_BUFFER, texcoordBuffer);
  gl.vertexAttribPointer(texcoordLocation, 2, gl.FLOAT, false, 0, 0);

  gl.uniform2f(positionResolutionLocation, width, height);
  gl.uniform2f(textureResolutionLocation, textureWidth, textureHeight);

  const colorLocation = gl.getUniformLocation(program, 'u_color');
  const backgroundColorLocation = gl.getUniformLocation(program, 'u_backgroundColor');
  const textureLocation = gl.getUniformLocation(program, 'u_texture');

  gl.uniform1i(colorLocation, 0);
  gl.uniform1i(backgroundColorLocation, 1);
  gl.uniform1i(textureLocation, 2);

  gl.activeTexture(gl.TEXTURE0);
  gl.bindTexture(gl.TEXTURE_2D, colorTexture);
  gl.activeTexture(gl.TEXTURE1);
  gl.bindTexture(gl.TEXTURE_2D, backgroundTexture);
  gl.activeTexture(gl.TEXTURE2);
  gl.bindTexture(gl.TEXTURE_2D, texture);
  gl.drawArrays(gl.TRIANGLES, 0, vertexPositions.length / 2);
}
