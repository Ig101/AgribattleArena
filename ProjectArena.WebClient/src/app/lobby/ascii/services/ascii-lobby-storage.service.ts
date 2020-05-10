import { Injectable } from '@angular/core';

@Injectable()
export class AsciiLobbyStorageService {

  userHash: number;

  patrolling: number;

  constructor() { }

  clear() {
    this.userHash = undefined;
    this.patrolling = undefined;
  }
}
