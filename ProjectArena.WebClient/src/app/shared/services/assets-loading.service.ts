import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { switchMap, map } from 'rxjs/operators';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AssetsLoadingService {

  shaders: {
    vertex: string,
    fragment: string
  };

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

  private processShaders(gl: WebGLRenderingContext, shaders: {
    vertex: string,
    fragment: string
  }) {
    const program = gl.createProgram();
    const vertexShader = this.createShader(gl, gl.VERTEX_SHADER, shaders.vertex);
    const fragmentShader = this.createShader(gl, gl.FRAGMENT_SHADER, shaders.fragment);
    gl.attachShader(program, vertexShader);
    gl.attachShader(program, fragmentShader);
    gl.linkProgram(program);
    if (gl.getProgramParameter(program, gl.LINK_STATUS)) {
      return program;
    }
    console.error(gl.getProgramInfoLog(program));
    gl.deleteProgram(program);
  }

  loadShadersAndCreateProgram(gl: WebGLRenderingContext, vertexShaderPath: string, fragmentShaderPath: string) {
    if (this.shaders) {
      return of(this.processShaders(gl, this.shaders));
    } else {
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
          this.shaders = result;
          return this.processShaders(gl, result);
        }));
    }
  }
}
