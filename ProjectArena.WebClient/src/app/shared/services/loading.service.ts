import { Injectable } from '@angular/core';
import { Observable, of, Subject } from 'rxjs';
import { LoadingDefinition } from '../models/loading/loading-definition.model';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {

  changed = false;

  alphaSpeed = 0.005;
  alpha = 1;

  definition: LoadingDefinition = {
    title: 'Loading...'
  };

  on = true;

  private stuck = false;
  private onInternal = true;
  private finishSubject: Subject<any>;
  private time: number;
  private requiredTime = 0;

  constructor() { }

  startLoading(loadingDefinition: LoadingDefinition, requiredTime?: number, stuck: boolean = false): Observable<any> {
    this.changed = true;
    if (stuck) {
      this.stuck = stuck;
    }
    this.definition = loadingDefinition;
    if (!this.onInternal) {
      this.requiredTime = requiredTime || 0;
      this.on = true;
      this.onInternal = true;
      if (this.finishSubject) {
        this.finishSubject.unsubscribe();
      }
      this.finishSubject = new Subject<any>();
      return this.finishSubject.asObservable();
    } else {
      return this.finishSubject.asObservable();
    }
  }

  finishLoading(): Observable<any> {
    if (this.onInternal) {
      this.onInternal = false;
      if (this.finishSubject) {
        this.finishSubject.unsubscribe();
      }
      this.finishSubject = new Subject<any>();
      return this.finishSubject.asObservable();
    } else {
      return of();
    }
  }

  setupTime() {
    this.time = performance.now();
  }

  loadingUpdate() {
    const lastTime = this.time;
    this.time = performance.now();
    const shift = this.time - lastTime;
    if (this.requiredTime > 0) {
      this.requiredTime -= shift;
    }
    if (this.onInternal && this.alpha < 1) {
      this.alpha = Math.min(1, this.alpha + this.alphaSpeed * shift);
      if (this.alpha >= 1 && this.finishSubject && !this.stuck) {
        this.finishSubject.next();
        this.finishSubject.complete();
        this.finishSubject.unsubscribe();
        this.finishSubject = undefined;
      }
    }
    if (!this.onInternal && this.alpha > 0 && this.requiredTime <= 0) {
      this.alpha = Math.max(0, this.alpha - this.alphaSpeed * shift);
      if (this.alpha <= 0 && this.finishSubject && !this.stuck) {
        this.on = false;
        this.finishSubject.next();
        this.finishSubject.complete();
        this.finishSubject.unsubscribe();
        this.finishSubject = undefined;
      }
    }
  }
}
