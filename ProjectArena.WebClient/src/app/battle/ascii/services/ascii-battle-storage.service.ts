import { Injectable } from '@angular/core';
import { Scene } from '../models/scene/scene.model';
import { Actor } from '../models/scene/actor.model';
import { ActivaDecoration } from '../models/scene/active-decoration.model';
import { Player } from '../models/player.model';

@Injectable()
export class AsciiBattleStorageService {

  scene: Scene;
  currentActor: Actor;
  currentDecoration: ActivaDecoration;
  players: Player[];

  version: number;
  turnTime: number;

  constructor() { }

  clear() {
    this.scene = undefined;
    this.currentActor = undefined;
    this.currentDecoration = undefined;
    this.players = undefined;
    this.version = -1;
  }
}
