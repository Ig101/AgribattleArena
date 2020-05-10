import { Injectable } from '@angular/core';
import { Scene } from '../models/scene/scene.model';
import { Actor } from '../models/scene/actor.model';
import { ActiveDecoration } from '../models/scene/active-decoration.model';
import { Player } from '../models/player.model';
import { ActionSquare } from '../models/actions/action-square.model';
import { InitiativePortrait } from '../models/gui/initiativePortrait.model';
import { BehaviorSubject } from 'rxjs';
import { AnimationDeclaration } from '../models/animations/animation-declaration.model';
import { AnimationTile } from '../models/animations/animation-tile.model';
import { FloatingText } from '../models/animations/floating-text.model';
import { EndGameDeclaration } from '../models/modals/end-game-declaration.model';
import { IModal } from 'src/app/shared/interfaces/modal.interface';

@Injectable()
export class AsciiBattleStorageService {

  openedModal: IModal<unknown>;

  endDeclaration: EndGameDeclaration;
  ended: boolean;

  floatingTextTime = 1000;
  floatingTextSpeed = 0.07;
  floatingTextDelay = 400;

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

  currentAnimations: AnimationTile[][];
  floatingTexts: FloatingText[] = [];

  currentInitiativeList: BehaviorSubject<InitiativePortrait[]>;

  skillHotkeys = [
    '1',
    '2',
    '3',
    '4',
    '5',
    '6',
    '7',
    '8',
    '9',
    '0'
  ];

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
    this.currentAnimations = undefined;
    this.floatingTexts = [];
    this.ended = false;
    this.endDeclaration = undefined;
  }
}
