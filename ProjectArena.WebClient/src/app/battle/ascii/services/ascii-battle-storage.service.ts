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
import { BiomEnum } from 'src/app/shared/models/enum/biom.enum';
import { Reward } from 'src/app/shared/models/battle/reward.model';

@Injectable()
export class AsciiBattleStorageService {

  openedModal: IModal<unknown>;

  endDeclaration: EndGameDeclaration;
  ended: boolean;
  shownEnd: boolean;

  floatingTextTime = 1000;
  floatingTextSpeed = 0.07;
  floatingTextDelay = 400;

  movePenalty = 2;

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

  currentAnimations: AnimationTile[][];
  floatingTexts: FloatingText[] = [];

  currentInitiativeList: BehaviorSubject<InitiativePortrait[]>;
  currentActionId: number;

  skillHotkeys = [
      { code: 'Digit1', key: '1' },
      { code: 'Digit2', key: '2' },
      { code: 'Digit3', key: '3' },
      { code: 'Digit4', key: '4' },
      { code: 'Digit5', key: '5' },
      { code: 'Digit6', key: '6' },
      { code: 'Digit7', key: '7' },
      { code: 'Digit8', key: '8' },
      { code: 'Digit9', key: '9' },
      { code: 'Digit0', key: '0' },
  ];

  idle = false;

  get isValidScene() {
    return !!this.scene;
  }

  reward: Reward;
  constructor() {
    this.currentInitiativeList = new BehaviorSubject<InitiativePortrait[]>(undefined);
  }

  setTurnTime(time: number) {
    this.turnTime = time;
  }

  clear() {
    this.reward = undefined;
    this.scene = undefined;
    this.currentActor = undefined;
    this.currentDecoration = undefined;
    this.players = undefined;
    this.version = -1;
    this.availableActionSquares = undefined;
    this.currentInitiativeList.next(undefined);
    this.turnTime = undefined;
    this.currentAnimations = undefined;
    this.floatingTexts = [];
    this.ended = false;
    this.shownEnd = false;
    this.endDeclaration = undefined;
    this.currentActionId = undefined;
    this.idle = false;
  }
}
