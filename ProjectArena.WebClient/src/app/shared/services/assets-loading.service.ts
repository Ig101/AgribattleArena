import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { switchMap, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AssetsLoadingService {

  constructor(
    private http: HttpClient
  ) { }

  private createShader(gl: WebGLRenderingContext, type: number, source: string) {
    const shader = gl.createShader(type);
    gl.shaderSource(shader, source);
    gl.compileShader(shader);
    if (gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
      return shader;
    }
    console.error(gl.getShaderInfoLog(shader));
    gl.deleteShader(shader);
  }

  loadShadersAndCreateProgram(gl: WebGLRenderingContext, vertexShaderPath: string, fragmentShaderPath: string) {
    return this.http.get(`assets/${vertexShaderPath}`, {responseType: 'text'})
      .pipe(switchMap((vertex: string) => {
        return this.http.get(`assets/${fragmentShaderPath}`, {responseType: 'text'})
          .pipe(map((fragment: string) => {
            return {
              vertex,
              fragment
            };
          }));
      }))
      .pipe(map((result) => {
        const program = gl.createProgram();
        const vertexShader = this.createShader(gl, gl.VERTEX_SHADER, result.vertex);
        const fragmentShader = this.createShader(gl, gl.FRAGMENT_SHADER, result.fragment);
        gl.attachShader(program, vertexShader);
        gl.attachShader(program, fragmentShader);
        gl.linkProgram(program);
        if (gl.getProgramParameter(program, gl.LINK_STATUS)) {
          return program;
        }
        console.error(gl.getProgramInfoLog(program));
        gl.deleteProgram(program);
      }));
  }
}
