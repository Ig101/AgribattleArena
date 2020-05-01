import { Injectable } from '@angular/core';
import { Scene } from '../models/scene/scene.model';
import { Actor } from '../models/scene/actor.model';
import { ActiveDecoration } from '../models/scene/active-decoration.model';
import { Player } from '../models/player.model';
import { ActionSquare } from '../models/actions/action-square.model';
import { InitiativePortrait } from '../models/gui/initiativePortrait.model';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class AsciiBattleStorageService {

  cameraX: number;
  cameraY: number;
  zoom: number;

  scene: Scene;
  currentActor: Actor;
  currentDecoration: ActiveDecoration;
  players: Player[];

  version: number;
  turnTime: number;

  availableActionSquares: ActionSquare[];
  defaultActionSquares: ActionSquare[];

  currentInitiativeList: BehaviorSubject<InitiativePortrait[]>;

  get isValidScene() {
    return !!this.scene;
  }

  constructor() {
    this.currentInitiativeList = new BehaviorSubject<InitiativePortrait[]>(undefined);
  }

  setTurnTime(time: number) {
    this.turnTime = time - 2;
  }

  clear() {
    this.scene = undefined;
    this.currentActor = undefined;
    this.currentDecoration = undefined;
    this.players = undefined;
    this.version = -1;
    this.availableActionSquares = undefined;
    this.defaultActionSquares = undefined;
    this.currentInitiativeList = undefined;
    this.turnTime = undefined;
  }
}
