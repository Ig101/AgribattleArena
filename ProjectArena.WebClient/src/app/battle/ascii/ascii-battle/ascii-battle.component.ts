import { Component, OnInit, OnDestroy, ViewChild, ElementRef, HostListener, AfterViewInit } from '@angular/core';
import { ActivatedRoute, ActivationEnd } from '@angular/router';
import { BattleResolverService } from '../../resolvers/battle-resolver.service';
import { AsciiBattleStorageService } from '../services/ascii-battle-storage.service';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';
import { Subscription, Subscribable } from 'rxjs';
import { Synchronizer } from 'src/app/shared/models/battle/synchronizer.model';
import { BattleSynchronizationActionEnum } from 'src/app/shared/models/enum/battle-synchronization-action.enum';
import { UserService } from 'src/app/shared/services/user.service';
import { BattlePlayerStatusEnum } from 'src/app/shared/models/enum/player-battle-status.enum';
import { Scene } from '../models/scene/scene.model';
import { newArray } from '@angular/compiler/src/util';
import { Tile } from '../models/scene/tile.model';
import { tileNatives, actorNatives, skillNatives, buffNatives } from '../natives';
import { Player } from '../models/player.model';
import { convertTile, convertActor, convertDecoration, convertEffect } from '../helpers/scene-create.helper';
import { Actor } from '../models/scene/actor.model';
import { ActiveDecoration } from '../models/scene/active-decoration.model';
import { SpecEffect } from '../models/scene/spec-effect.model';
import { removeFromArray } from 'src/app/helpers/extensions/array.extension';
import { synchronizeActor, synchronizeTile, synchronizeDecoration, synchronizeEffect } from '../helpers/scene-update.helper';
import { AsciiBattleSynchronizerService } from '../services/ascii-battle-synchronizer.service';
import { MouseState } from 'src/app/shared/models/mouse-state.model';
import { SmartAction } from '../models/actions/smart-action.model';
import { Color } from 'src/app/shared/models/color.model';
import { ActionSquare } from '../models/actions/action-square.model';
import { Visualization } from '../models/visualization.model';
import { ActionSquareTypeEnum } from '../models/enum/action-square-type.enum';
import { InitiativePortrait } from '../models/gui/initiativePortrait.model';
import { SmartActionTypeEnum } from '../models/enum/smart-action-type.enum';
import { AsciiBattlePathCreatorService } from '../services/ascii-battle-path-creator.service';
import { AsciiBattleAnimationsService } from '../services/ascii-battle-animations.service';
import { AnimationTile } from '../models/animations/animation-tile.model';
import { heightImpact, brightImpact } from '../helpers/scene-draw.helper';
import { LoadingService } from 'src/app/shared/services/loading.service';
import { ModalService } from 'src/app/shared/services/modal.service';
import { EndGameDeclaration } from '../models/modals/end-game-declaration.model';
import { VictoryModalComponent } from '../modals/victory-modal/victory-modal.component';
import { ActorModalComponent } from '../modals/actor-modal/actor-modal.component';
import { SkillModalComponent } from '../modals/skill-modal/skill-modal.component';
import { Skill } from '../models/scene/skill.model';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { DecorationModalComponent } from '../modals/decoration-modal/decoration-modal.component';
import { checkMilliness, checkSkillTargets } from '../helpers/scene-actions.helper';
import { Random } from 'src/app/shared/random/random';
import { getRandomBiom } from 'src/app/shared/bioms/biom.helper';

@Component({
  selector: 'app-ascii-battle',
  templateUrl: './ascii-battle.component.html',
  styleUrls: ['./ascii-battle.component.scss']
})
export class AsciiBattleComponent implements OnInit, OnDestroy, AfterViewInit {

  @ViewChild('battleCanvas', { static: true }) battleCanvas: ElementRef<HTMLCanvasElement>;
  private canvasContext: CanvasRenderingContext2D;

  lastChange: number;

  drawingTimer;
  updatingFrequency = 30;
  changed = false;
  blocked = false;

  private tileWidthInternal = 0;
  private tileHeightInternal = 30;
  readonly defaultWidth = 1530;
  readonly defaultHeight = 1080;
  readonly defaultAspectRatio = this.defaultWidth / this.defaultHeight;

  smartActions: SmartAction[];
  mouseState: MouseState = {
    buttonsInfo: {},
    x: -1,
    y: -1,
    realX: -1,
    realY: -1
  };

  selectedTile: { x: number, y: number, duration: number, forced: boolean };
  selectedAlwaysTile: { x: number, y: number };

  tickingFrequency = 2;
  tickState = 0;

  onCloseSubscription: Subscription;
  arenaActionsSubscription: Subscription;
  synchronizationErrorSubscription: Subscription;
  animationSubscription: Subscription;
  finishLoadingSubscription: Subscription;
  victorySubscription: Subscription;

  receivingMessagesFromHubAllowed = false;
  loadingFinished = false;
  specificActionResponseForWait: {
    actorId: number,
    action: BattleSynchronizationActionEnum
  };
  actionsQueue: {
    actorId: number,
    x: number,
    y: number,
    action: BattleSynchronizationActionEnum
  }[] = [];

  skillList: SmartAction[] = [];
  endTurn: SmartAction;
  currentSkillId: number;
  pressedKey: string;

  zoom = 0;

  animationTicker = false;

  get canAct() {
    return !this.blocked && !this.specificActionResponseForWait && this.actionsQueue.length === 0 &&
      this.battleStorageService.turnTime > 0 &&
      this.battleStorageService.currentActor?.owner?.id === this.userService.user.id &&
      this.receivingMessagesFromHubAllowed &&
      !this.specificActionResponseForWait;
  }

  get canvasWidth() {
    return this.battleCanvas.nativeElement.width;
  }

  get canvasHeight() {
    return this.battleCanvas.nativeElement.height;
  }

  get cameraX() {
    return this.battleStorageService.cameraX;
  }

  set cameraX(value: number) {
    // TODO camera boundaries
    this.battleStorageService.cameraX = value;
  }

  get cameraY() {
    return this.battleStorageService.cameraY;
  }

  set cameraY(value: number) {
    // TODO camera boundaries
    this.battleStorageService.cameraY = value;
  }

  get battleZoom() {
    return this.battleStorageService.zoom;
  }

  get turnTime() {
    return this.battleStorageService.turnTime;
  }

  get tileWidth() {
    return this.tileWidthInternal * this.battleZoom;
  }

  get tileHeight() {
    return this.tileHeightInternal * this.battleZoom;
  }

  get isMyTurn() {
    return this.battleStorageService?.currentActor?.owner?.id === this.userService?.user.id;
  }

  get currentActorColor() {
    return this.battleStorageService.currentActor?.visualization.color;
  }

  get maxActionPoints() {
    return 8;
  }

  get currentActionPoints() {
    return this.battleStorageService.currentActor?.actionPoints;
  }

  get actionPointsAfterSpend() {
    if (!this.canAct) {
      return this.battleStorageService.currentActor?.actionPoints;
    }
    const mouseX = Math.floor(this.mouseState.x);
    const mouseY = Math.floor(this.mouseState.y);
    const currentActionSquare = this.battleStorageService.availableActionSquares
      ?.find(s => s.x === mouseX && s.y === mouseY && s.type);
    return ((currentActionSquare?.remainedPoints + 1) || (this.battleStorageService.currentActor?.actionPoints + 1)) - 1;
  }

  get interfaceShift() {
    return 374 / this.zoom;
  }

  get userName() {
    return this.userService.user.name;
  }

  constructor(
    private activatedRoute: ActivatedRoute,
    private battleResolver: BattleResolverService,
    private battleStorageService: AsciiBattleStorageService,
    private battleSynchronizerService: AsciiBattleSynchronizerService,
    private battlePathCreator: AsciiBattlePathCreatorService,
    private arenaHub: ArenaHubService,
    private userService: UserService,
    private battleAnimationsService: AsciiBattleAnimationsService,
    private loadingService: LoadingService,
    private modalService: ModalService,
  ) {
    this.endTurn = {
      hotKey: ' ',
      type: SmartActionTypeEnum.Hold,
      smartValue: 0,
      pressed: false,
      actions: [() => {
        this.specificActionResponseForWait = {
          actorId: this.battleStorageService.currentActor.id,
          action: BattleSynchronizationActionEnum.Wait
        };
        this.arenaHub.orderWait(this.battleStorageService.scene.id, this.battleStorageService.currentActor.id);
        this.battleAnimationsService
          .generateAnimationsFromIssue(BattleSynchronizationActionEnum.Wait, this.battleStorageService.currentActor);
      }],
      title: 'End turn',
      disabled: undefined
    };
    this.mouseState.buttonsInfo[0] = {
      pressed: false,
      timeStamp: 0
    };
    this.mouseState.buttonsInfo[2] = {
      pressed: false,
      timeStamp: 0
    };
    this.onCloseSubscription = arenaHub.onClose.subscribe(() => {
      this.loadingService.startLoading({
        title: 'Server connection is lost. Refresh the page or try again later...'
      }, 0, true);
      console.error('Hub connection is lost');
    });
    this.synchronizationErrorSubscription = arenaHub.synchronizationErrorState.subscribe((value) => {
      if (value) {
        this.loadingService.startLoading({
          title: 'Desynchronization. Page will be refreshed in 2 seconds.'
        }, 0, true);
        console.error('Unexpected synchronization error');
        setTimeout(() => {
       //   location.reload();
        }, 2000);
      }
    });
    this.animationSubscription = battleAnimationsService.generationConclusion.subscribe((pending) => {
      this.processNextActionFromQueueWithChecks(pending);
    });
    this.victorySubscription = battleAnimationsService.victoryAnimationPlayed.subscribe((declaration) => {
      this.endGame(declaration);
    });
    this.arenaActionsSubscription = arenaHub.battleSynchronizationActionsNotifier.subscribe(() => {
      if (this.receivingMessagesFromHubAllowed) {
        this.processNextActionFromQueue();
      }
    });
  }

  ngOnDestroy(): void {
    this.battleStorageService.clear();
    this.onCloseSubscription.unsubscribe();
    this.arenaActionsSubscription.unsubscribe();
    this.synchronizationErrorSubscription.unsubscribe();
    this.animationSubscription.unsubscribe();
    this.finishLoadingSubscription.unsubscribe();
    this.victorySubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.tileWidthInternal = this.tileHeightInternal * 0.6;
    this.setupAspectRatio(this.battleCanvas.nativeElement.offsetWidth, this.battleCanvas.nativeElement.offsetHeight);
    this.canvasContext = this.battleCanvas.nativeElement.getContext('2d');
    this.battleStorageService.version = 0;
    const loadBattle = this.activatedRoute.snapshot.data.battle;
    this.drawingTimer = setInterval(this.updateCycle, 1000 / this.updatingFrequency, this);
    if (loadBattle) {
      const snapshot = this.battleResolver.popBattleSnapshot();
      this.restoreScene(snapshot);
      if (!this.loadingFinished) {
        return;
      }
    }
    this.processNextActionFromQueue();
  }

  ngAfterViewInit(): void {
    this.finishLoadingSubscription = this.loadingService.finishLoading()
      .subscribe(() => {
        this.loadingFinished = true;
        this.processNextActionFromQueue();
      });
  }

  onResize() {
    this.setupAspectRatio(this.battleCanvas.nativeElement.offsetWidth, this.battleCanvas.nativeElement.offsetHeight);
  }

  setupAspectRatio(width: number, height: number) {
    const newAspectRatio = width / height;
    if (newAspectRatio < this.defaultAspectRatio) {
      const oldWidth = this.defaultWidth;
      this.battleCanvas.nativeElement.width = oldWidth;
      this.battleCanvas.nativeElement.height = oldWidth / newAspectRatio;
    } else {
      const oldHeight = this.defaultHeight;
      this.battleCanvas.nativeElement.width = oldHeight * newAspectRatio;
      this.battleCanvas.nativeElement.height = oldHeight;
    }
    this.zoom = this.battleCanvas.nativeElement.offsetWidth / this.canvasWidth;
    this.changed = true;
    this.redrawScene();
  }

  onPositionSelect(x: number, y: number) {
    if (!x || !y) {
      this.selectedTile = undefined;
    }
    if ((!this.selectedTile || this.selectedTile.x !== x || this.selectedTile.y !== y)) {
      this.selectedTile = {x, y, duration: 0, forced: true};
    }
  }

  onMouseDown(event: MouseEvent) {
    if (!this.blocked) {
      this.mouseState.buttonsInfo[event.button] = {pressed: true, timeStamp: event.timeStamp};
    }
  }

  openSettingsl() {

  }

  openModalFromPosition(x: number, y: number) {
    if (x >= 0 && y >= 0 && x < this.battleStorageService.scene.width && y < this.battleStorageService.scene.height) {
      const tile = this.battleStorageService.scene.tiles[x][y];
      if (tile.actor) {
        this.blocked = true;
        this.selectedAlwaysTile = {x, y};
        this.battleStorageService.openedModal = this.modalService.openModal(ActorModalComponent, tile.actor);
        this.battleStorageService.openedModal.onClose.subscribe(() => {
          this.blocked = false;
          this.selectedAlwaysTile = undefined;
          this.battleStorageService.openedModal = undefined;
        });
      }
      if (tile.decoration) {
        this.blocked = true;
        const oldTile = this.selectedAlwaysTile;
        this.selectedAlwaysTile = {x, y};
        this.battleStorageService.openedModal = this.modalService.openModal(DecorationModalComponent, tile.decoration);
        this.battleStorageService.openedModal.onClose.subscribe(() => {
          this.blocked = false;
          this.selectedAlwaysTile = undefined;
          this.battleStorageService.openedModal = undefined;
        });
      }
    }
  }

  openSkillModal(skill: Skill) {
    this.blocked = true;
    this.battleStorageService.openedModal = this.modalService.openModal(SkillModalComponent, skill);
    this.battleStorageService.openedModal.onClose.subscribe(() => {
      this.blocked = false;
      this.battleStorageService.openedModal = undefined;
    });
  }

  onMouseUp(event: MouseEvent) {
    this.recalculateMouseMove(event.x, event.y, event.timeStamp);
    if (!this.blocked) {
      if (!this.skillList.find(x => x.pressed) && !this.endTurn.pressed) {
        this.mouseState.buttonsInfo[event.button] = {pressed: false, timeStamp: 0};
        const x = Math.floor(this.mouseState.x);
        const y = Math.floor(this.mouseState.y);
        this.recalculateMouseMove(event.x, event.y, event.timeStamp);
        if (event.button === 2 && Math.floor(this.mouseState.x) === x && Math.floor(this.mouseState.y) === y) {
          // Context
          this.openModalFromPosition(x, y);
        }
        if (event.button === 0 && this.canAct && Math.floor(this.mouseState.x) === x && Math.floor(this.mouseState.y) === y) {
          // Action
          const currentActionSquare = this.battleStorageService.availableActionSquares
          ?.find(s => s.x === x && s.y === y && s.type);
          if (currentActionSquare) {
            // TODO cast spells
            if (this.currentSkillId) {
              this.arenaHub.orderCast(
                this.battleStorageService.scene.id,
                this.battleStorageService.currentActor.id,
                this.currentSkillId,
                currentActionSquare.x,
                currentActionSquare.y);
              this.specificActionResponseForWait = {
                actorId: this.battleStorageService.currentActor.id,
                action: BattleSynchronizationActionEnum.Cast
              };
              this.battleAnimationsService
                .generateAnimationsFromIssue(BattleSynchronizationActionEnum.Cast, this.battleStorageService.currentActor,
                currentActionSquare.x, currentActionSquare.y, this.currentSkillId);
              this.resetSkillActions();
            } else {
              this.actionsQueue = [
                ...currentActionSquare.parentSquares.filter(s => !s.isActor)
                  .reverse().map(s => {
                    return {
                    actorId: this.battleStorageService.currentActor.id,
                    x: s.x,
                    y: s.y,
                    action: s.type === ActionSquareTypeEnum.Move ?
                      BattleSynchronizationActionEnum.Move :
                      BattleSynchronizationActionEnum.Attack
                    };
                  }), {
                    actorId: this.battleStorageService.currentActor.id,
                    x: currentActionSquare.x,
                    y: currentActionSquare.y,
                    action: currentActionSquare.type === ActionSquareTypeEnum.Move ?
                      BattleSynchronizationActionEnum.Move :
                      BattleSynchronizationActionEnum.Attack
                  }];
              this.sendActionFromQueue();
            }
          }
        }
      } else {
        this.mouseState.buttonsInfo[event.button] = {pressed: false, timeStamp: 0};
      }
      this.resetButtonsPressedState();
    }
  }

  onMouseLeave() {
    for (const state of Object.values(this.mouseState.buttonsInfo)) {
      state.pressed = false;
      state.timeStamp = 0;
    }
  }

  onMouseMove(event: MouseEvent) {
    if (!this.blocked) {
      this.mouseState.realX = event.x;
      this.mouseState.realY = event.y;
      this.recalculateMouseMove(event.x, event.y, event.timeStamp);
    }
  }

  resetButtonsPressedState() {
    this.endTurn.pressed = false;
    for (const skill of this.skillList) {
      skill.pressed = false;
    }
  }

  onKeyDown(event: KeyboardEvent) {
    if (!this.blocked) {
      this.pressedKey = event.key;
      this.resetButtonsPressedState();
      const action = this.endTurn.hotKey === event.key ? this.endTurn : this.skillList.find(x => x.hotKey === event.key);
      if (action && this.canAct && !action?.disabled) {
        action.pressed = true;
      } else if (event.key !== 'Escape') {
        this.pressedKey = undefined;
      }
    }
  }

  private resetSkillActions() {
    for (const action of this.skillList) {
      if  (action.smartValue > 0) {
        action.actions[action.smartValue]();
        this.changed = true;
      }
    }
  }

  onKeyUp(event: KeyboardEvent) {
    if (!this.blocked) {
      if (this.pressedKey === event.key && event.key === 'Escape') {
        this.resetSkillActions();
      }
      if (this.pressedKey === event.key && this.canAct) {
        const action = this.endTurn.hotKey === event.key ? this.endTurn : this.skillList.find(x => x.hotKey === event.key);
        if (action?.pressed && !action?.disabled) {
          if (action.type === SmartActionTypeEnum.Toggle) {
            action.actions[action.smartValue]();
          } else if (action.type !== SmartActionTypeEnum.Hold || action.smartValue >= 0.95) {
            action.actions[0]();
          }
          action.pressed = false;
          this.changed = true;
        }
        this.pressedKey = undefined;
      }
    }
  }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event) {
    event.preventDefault();
  }

  cameraShift(xShift: number, yShift: number) {
    this.mouseState.x += xShift;
    this.mouseState.y += yShift;
    this.cameraX += xShift;
    this.cameraY += yShift;
    this.changed = true;
  }

  private recalculateMouseMove(x: number, y: number, timeStamp?: number) {
    const leftKey = this.mouseState.buttonsInfo[0];
    const rightKey = this.mouseState.buttonsInfo[2];
    if (!rightKey.pressed && !leftKey.pressed) {
      const cameraLeft = this.battleStorageService.cameraX - (this.canvasWidth - this.interfaceShift) / 2 / this.tileWidth;
      const cameraTop = this.battleStorageService.cameraY - this.canvasHeight / 2 / this.tileHeight;
      const newX = x / this.zoom / this.tileWidth + cameraLeft;
      const newY = y / this.zoom / this.tileHeight + cameraTop;
      this.mouseState.x = newX;
      this.mouseState.y = newY;
      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);
      /*if (this.battleStorageService.scene && mouseX >= 0 && mouseY >= 0 &&
        mouseX < this.battleStorageService.scene.width && mouseY < this.battleStorageService.scene.height) {
        if ((!this.selectedTile || this.selectedTile.x !== mouseX || this.selectedTile.y !== mouseY)) {
          this.selectedTile = {x: mouseX, y: mouseY, duration: 0, forced: false};
        }
      } else if (this.selectedTile && !this.selectedTile.forced) {
        this.selectedTile = undefined;
      }*/
      this.changed = true;
    }
  }

   // Drawing

  private mixColorWithReplacement(color: Color, replacement: AnimationTile, z: number): Color {
    const newColor = { r: color.r, g: color.g, b: color.b, a: color.a };
    let mainColor: Color = replacement ?
    { r : replacement.color.r * (1 - replacement.unitColorMultiplier),
      g : replacement.color.g * (1 - replacement.unitColorMultiplier),
      b : replacement.color.b * (1 - replacement.unitColorMultiplier),
      a : replacement.unitAlpha ? 0 : replacement.color.a }
    : { r: 0, g: 0, b: 0, a: 0 };
    if (!(replacement?.ignoreHeight)) {
      mainColor = heightImpact(z, mainColor);
    }
    if (replacement && replacement.unitColorMultiplier < 1) {
      newColor.r *= replacement.unitColorMultiplier;
      newColor.g *= replacement.unitColorMultiplier;
      newColor.b *= replacement.unitColorMultiplier;
    }
    if (replacement && !replacement.unitAlpha) {
      newColor.a = 0;
    }
    newColor.r += mainColor.r;
    newColor.g += mainColor.g;
    newColor.b += mainColor.b;
    newColor.a += mainColor.a;
    return newColor;
  }

  private drawDummyPoint(
    char: string,
    color: Color,
    backgroundColor: Color,
    x: number, y: number,
    cameraLeft: number,
    cameraTop: number) {
    const dim = 0.5;
    const canvasX = (x - cameraLeft) * this.tileWidth;
    const canvasY = (y - cameraTop) * this.tileHeight;
    const symbolY = canvasY + this.tileHeight * 0.75;
    if (backgroundColor) {
      this.canvasContext.fillStyle = `rgb(${backgroundColor.r * dim}, ${backgroundColor.g * dim}, ${backgroundColor.b * dim})`;
      this.canvasContext.fillRect(canvasX, canvasY, this.tileWidth + 1, this.tileHeight + 1);
    }
    this.canvasContext.fillStyle = `rgba(${color.r * dim}, ${color.g * dim}, ${color.b * dim}, ${color.a})`;
    this.canvasContext.fillText(char, canvasX, symbolY);
  }

  private drawPoint(scene: Scene, x: number, y: number, cameraLeft: number, cameraTop: number, drawChar: Visualization) {
    const tile = scene.tiles[x][y];
    if (tile) {
      const canvasX = (x - cameraLeft) * this.tileWidth;
      const canvasY = (y - cameraTop) * this.tileHeight;
      const symbolY = canvasY + this.tileHeight * 0.75;
      if (tile.backgroundColor) {
        const color = heightImpact(tile.height, tile.backgroundColor);
        this.canvasContext.fillStyle = `rgb(${color.r}, ${color.g}, ${color.b})`;
        this.canvasContext.fillRect(canvasX, canvasY, this.tileWidth + 1, this.tileHeight + 1);
      }
      const selected = (this.selectedTile && this.selectedTile.duration > 500 && this.selectedTile.x === x && this.selectedTile.y === y) ||
      (this.selectedAlwaysTile && this.selectedAlwaysTile.x === x && this.selectedAlwaysTile.y === y);
      const replacement = this.battleStorageService.currentAnimations ? this.battleStorageService.currentAnimations[x][y] : undefined;
      if (drawChar) {
        const color = brightImpact(tile.bright, drawChar.color);
        this.canvasContext.fillStyle = `rgba(${color.r},${color.g},${color.b},${color.a})`;
        this.canvasContext.fillText(drawChar.char, canvasX, symbolY);
      } else
      if ((tile.actor || tile.decoration) && ((tile.specEffects.length === 0 && !selected) || Math.floor(this.tickState) % 2 === 1)) {
        if (tile.actor) {
          let color = heightImpact(tile.actor.z, tile.actor === this.battleStorageService.currentActor &&
            this.battleStorageService.currentActor.owner?.id === this.userService.user.id ?
            {r: 255, g: 255, b: 0, a: tile.actor.visualization.color.a} :
            tile.actor.visualization.color);
          color = brightImpact(tile.bright, replacement ? this.mixColorWithReplacement(color, replacement, tile.actor.z) : color);
          this.canvasContext.fillStyle = `rgba(${color.r},${color.g},${color.b},${color.a})`;
          this.canvasContext.fillText(replacement?.char ? replacement.char : tile.actor.visualization.char, canvasX, symbolY);
        } else if (tile.decoration) {
          let color = heightImpact(tile.decoration.z, tile.decoration.visualization.color);
          color = brightImpact(tile.bright, replacement ? this.mixColorWithReplacement(color, replacement, tile.decoration.z) : color);
          this.canvasContext.fillStyle = `rgba(${color.r},${color.g},${color.b},${color.a})`;
          this.canvasContext.fillText(replacement?.char ? replacement.char : tile.decoration.visualization.char, canvasX, symbolY);
        }
      } else if (tile.specEffects.length > 0 && !selected) {
        const firstEffect = tile.specEffects[0];
        let color = heightImpact(firstEffect.z, firstEffect.visualization.color);
        color =  brightImpact(tile.bright, replacement?.workingOnSpecEffects ?
          this.mixColorWithReplacement(color, replacement, firstEffect.z) :
          color);
        this.canvasContext.fillStyle = `rgba(${color.r},${color.g},${color.b},${color.a})`;
        this.canvasContext.fillText( replacement?.workingOnSpecEffects ?
          replacement.char :
          firstEffect.visualization.char, canvasX, symbolY);
      } else {
        const color =  replacement?.workingOnSpecEffects && replacement?.char ?
          brightImpact(tile.bright, replacement.color) :
          heightImpact(tile.height, tile.visualization.color);
        this.canvasContext.fillStyle = `rgba(${color.r}, ${color.g},
          ${color.b}, ${color.a})`;
        this.canvasContext.fillText(replacement?.char ? replacement.char : tile.visualization.char, canvasX, symbolY);
      }
      if ((tile.actor || tile.decoration) && !(replacement?.overflowHealth)) {
        const healthObject = tile.actor || tile.decoration;
        const percentOfHealth = Math.max(0, Math.min(healthObject.health / healthObject.maxHealth, 1));
        let color: Color;
        if (percentOfHealth > 0.65) {
          color = {r: 0, g: 255, b: 0};
        } else if (percentOfHealth > 0.25) {
          color = {r: 255, g: 255, b: 0};
        } else {
          color = {r: 255, g: 0, b: 0};
        }
        color = brightImpact(tile.bright, color);
        const zoomMultiplier = Math.floor(this.battleZoom);
        this.canvasContext.lineWidth = zoomMultiplier * 2;
        this.canvasContext.strokeStyle = `rgb(${color.r}, ${color.g}, ${color.b})`;
        const path = new Path2D();
        path.moveTo(canvasX + 1 + zoomMultiplier, canvasY + 1 + zoomMultiplier);
        path.lineTo(
          canvasX + percentOfHealth * (this.tileWidth - 2 * 1 - zoomMultiplier) + 1 + zoomMultiplier,
          canvasY + 1 + zoomMultiplier);
        this.canvasContext.stroke(path);
      }
    }
  }

  private generateActionSquareGrid(cameraLeft: number, cameraTop: number): Path2D {
    const path = new Path2D();
    for (const square of this.battleStorageService.availableActionSquares) {
      const canvasX = (square.x - cameraLeft) * this.tileWidth;
      const canvasY = (square.y - cameraTop) * this.tileHeight;
      if (!square.bottomSquare) {
        path.moveTo(canvasX - 1, canvasY + this.tileHeight);
        path.lineTo(canvasX + this.tileWidth + 1, canvasY + this.tileHeight);
      }
      if (!square.topSquare) {
        path.moveTo(canvasX - 1, canvasY);
        path.lineTo(canvasX + this.tileWidth + 1, canvasY);
      }
      if (!square.leftSquare) {
        path.moveTo(canvasX, canvasY - 1);
        path.lineTo(canvasX, canvasY + this.tileHeight + 1);
      }
      if (!square.rightSquare) {
        path.moveTo(canvasX + this.tileWidth, canvasY - 1);
        path.lineTo(canvasX + this.tileWidth, canvasY + this.tileHeight + 1);
      }
    }
    return path;
  }

  private redrawScene() {
    if (!this.changed && !this.loadingFinished) {
      return;
    }
    this.changed = false;
    const scene = this.battleStorageService.scene;
    if (scene) {
      const sceneRandom = new Random(this.battleStorageService.scene.hash);
      const cameraLeft = this.battleStorageService.cameraX - (this.canvasWidth - this.interfaceShift) / 2 / this.tileWidth;
      const cameraTop = this.battleStorageService.cameraY - this.canvasHeight / 2 / this.tileHeight;
      this.canvasContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
      this.canvasContext.font = `${this.tileHeight}px PT Mono`;
      this.canvasContext.textAlign = 'left';
      const left = Math.floor(cameraLeft) - 1;
      const right = Math.ceil(cameraLeft + this.canvasWidth / (this.tileWidth)) + 1;
      const top = Math.floor(cameraTop) - 1;
      const bottom = Math.ceil(cameraTop + this.canvasHeight / (this.tileHeight)) + 1;
      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);
      const currentActionSquare = this.canAct ? this.battleStorageService.availableActionSquares
        ?.find(s => s.x === mouseX && s.y === mouseY && s.type) : undefined;
      for (let x = -40; x <= 80; x++) {
        for (let y = -20; y <= 60; y++) {
          if (x >= left && y >= top && x <= right && y <= bottom) {
            if (x >= 0 && y >= 0 && x < this.battleStorageService.scene.width && y < this.battleStorageService.scene.height) {
              let drawChar;
              if (currentActionSquare?.x === x && currentActionSquare?.y === y) {
                drawChar = {
                  char: 'x',
                  color: currentActionSquare.type === ActionSquareTypeEnum.Act ? {r: 255, g: 0, b: 0, a: 1} : {r: 255, g: 255, b: 0, a: 1}
                };
              } else if (currentActionSquare?.type === ActionSquareTypeEnum.Act && currentActionSquare.parentSquares.length > 0 &&
                currentActionSquare?.parentSquares[0].x === x && currentActionSquare?.parentSquares[0].y === y &&
                !currentActionSquare?.parentSquares[0].isActor) {
                drawChar = {
                  char: 'x',
                  color: {r: 255, g: 255, b: 0, a: 1}
                };
              } else if (currentActionSquare?.parentSquares.some(s => s.x === x && s.y === y && !s.isActor)) {
                drawChar = {
                  char: '.',
                  color: {r: 255, g: 255, b: 0, a: 1}
                };
              }
              sceneRandom.next();
              this.drawPoint(scene, x, y, cameraLeft, cameraTop, drawChar);
            } else {
              const biom = getRandomBiom(sceneRandom, this.battleStorageService.scene.biom);
              this.drawDummyPoint(biom.char, biom.color, biom.backgroundColor, x, y, cameraLeft, cameraTop);
            }
          } else {
            sceneRandom.next();
          }
        }
      }
      if (this.battleStorageService.availableActionSquares?.length > 0) {
        const path = this.generateActionSquareGrid(cameraLeft, cameraTop);
        this.canvasContext.strokeStyle = this.currentSkillId ? 'rgba(255, 0, 0, 0.8)' : 'rgba(255, 255, 0, 0.8)';
        this.canvasContext.lineWidth = 2;
        this.canvasContext.stroke(path);
      }
      for (const text of this.battleStorageService.floatingTexts) {
        if (text.time >= 0) {
          const x = (text.x + 0.5 - cameraLeft) * this.tileWidth;
          const y = (text.y - cameraTop) * this.tileHeight - text.height;
          this.canvasContext.font = `${26}px PT Mono`;
          this.canvasContext.textAlign = 'center';
          this.canvasContext.fillStyle = `rgba(${text.color.r}, ${text.color.g},
            ${text.color.b}, ${text.color.a})`;
          this.canvasContext.fillText(text.text, x, y);
          this.canvasContext.lineWidth = 1;
          this.canvasContext.strokeStyle = `rgba(0, 8, 24, ${text.color.a})`;
          this.canvasContext.strokeText(text.text, x, y);
        }
      }
    }
  }

  // Updating
  private sendActionFromQueue() {
    const newAction = this.actionsQueue.shift();
    if (!this.battleStorageService.currentActor) {
      this.specificActionResponseForWait = undefined;
      return;
    }
    this.specificActionResponseForWait = {
      actorId: newAction.actorId,
      action: newAction.action
    };
    const sX = Math.abs(this.battleStorageService.currentActor.x - newAction.x);
    const sY = Math.abs(this.battleStorageService.currentActor.y - newAction.y);
    const tile = this.battleStorageService.scene.tiles[newAction.x][newAction.y];
    const initialTile = this.battleStorageService.scene
      .tiles[this.battleStorageService.currentActor.x][this.battleStorageService.currentActor.y];
    switch (newAction.action) {
      case BattleSynchronizationActionEnum.Move:
        if (newAction.actorId === this.battleStorageService.currentActor.id &&
          (sX === 1 && sY === 0 || sX === 0 && sY === 1) && this.battleStorageService.currentActor.canMove &&
          !tile.unbearable && !tile.actor && !tile.decoration &&
          Math.abs(tile.height - initialTile.height) < 10) {
            this.arenaHub.orderMove(this.battleStorageService.scene.id, newAction.actorId, newAction.x, newAction.y);
            this.battleAnimationsService
              .generateAnimationsFromIssue(BattleSynchronizationActionEnum.Move, this.battleStorageService.currentActor,
              newAction.x, newAction.y);
        } else {
          this.specificActionResponseForWait = undefined;
        }
        break;
      case BattleSynchronizationActionEnum.Attack:
        const skill = this.battleStorageService.currentActor.attackingSkill;
        if (newAction.actorId === this.battleStorageService.currentActor.id &&
          this.battleStorageService.currentActor.canAct &&
          checkSkillTargets(tile, this.battleStorageService.currentActor, skill.availableTargets) &&
          (!skill.onlyVisibleTargets || checkMilliness(initialTile, tile, this.battleStorageService.scene.tiles)) &&
          (tile.actor || tile.decoration) && tile.actor !== this.battleStorageService.currentActor) {
          this.arenaHub.orderAttack(this.battleStorageService.scene.id, newAction.actorId, newAction.x, newAction.y);
          this.battleAnimationsService
            .generateAnimationsFromIssue(BattleSynchronizationActionEnum.Attack, this.battleStorageService.currentActor,
            newAction.x, newAction.y);
        } else {
          this.specificActionResponseForWait = undefined;
        }
        break;
      default:
        this.loadingService.startLoading({
          title: 'Desynchronization. Page will be refreshed in 2 seconds.'
        }, 0, true);
        console.error('Action is not found');
        setTimeout(() => {
     //     location.reload();
        }, 2000);
        return;
    }
  }

  private calculateInitiativeScale(): InitiativePortrait[] {
    const allPortraits = this.battleStorageService.scene.actors.map(x => {
      const color = x.visualization.color;
      return {
        color: `rgba(${color.r},${color.g},${color.b},${color.a})`,
        char: x.visualization.char,
        initiativePosition: x.initiativePosition,
        active: true,
        speed: x.initiative,
        x: x.x,
        y: x.y
      };
    }).concat(
      this.battleStorageService.scene.decorations.map(x => {
        const color = x.visualization.color;
        return {
          color: `rgba(${color.r},${color.g},${color.b},${color.a})`,
          char: x.visualization.char,
          initiativePosition: x.initiativePosition,
          active: x.active,
          speed: 1,
          x: x.x,
          y: x.y
        };
      }));
    if (allPortraits.length === 0) {
      return undefined;
    }
    const portraits: InitiativePortrait[] = [];
    while (portraits.length < 6) {
      let candidate = allPortraits[0];
      let candidateInitiative = allPortraits[0].initiativePosition;
      for (let i = 1; i < allPortraits.length; i++) {
        if (allPortraits[i].active && allPortraits[i].initiativePosition < candidateInitiative) {
          candidate = allPortraits[i];
          candidateInitiative = allPortraits[i].initiativePosition;
        }
      }
      portraits.push(candidate);
      candidate.initiativePosition += 1 / candidate.speed;
    }
    return portraits;
  }

  private recalculateSkillActions() {
    if (this.isMyTurn) {
      this.skillList.length = this.battleStorageService.currentActor.skills.length;
      for (let i = 0; i < this.skillList.length; i++) {
        if (!this.skillList[i]) {
          this.skillList[i] = {
            hotKey: this.battleStorageService.skillHotkeys[i],
            type: SmartActionTypeEnum.Toggle,
            pressed: this.pressedKey === this.battleStorageService.skillHotkeys[i],
            title: this.battleStorageService.currentActor.skills[i].name,
            smartValue: this.battleStorageService.currentActor.skills[i].id === this.currentSkillId ? 1 : 0,
            actions: [
              () => {
                const skill = this.battleStorageService.currentActor.skills[i];
                const actor = this.battleStorageService.currentActor;
                this.currentSkillId = skill.id;
                this.battleStorageService.availableActionSquares =
                  this.battlePathCreator.calculateActiveSquares(actor, this.currentSkillId);
                for (const action of this.skillList) {
                  action.smartValue = 0;
                }
                this.skillList[i].smartValue = 1;
              },
              () => {
                this.currentSkillId = undefined;
                this.battleStorageService.availableActionSquares = this.battleStorageService.defaultActionSquares;
                this.skillList[i].smartValue = 0;
              }],
            smartObject: this.battleStorageService.currentActor.skills[i],
            disabled:
              this.battleStorageService.currentActor.skills[i].cost > this.battleStorageService.currentActor.actionPoints ?
              `Not enough action points. Need ${this.battleStorageService.currentActor.skills[i].cost}` :
              this.battleStorageService.currentActor.skills[i].preparationTime > 0 ?
              `Skill will be available after ${Math.floor(this.battleStorageService.currentActor.skills[i].preparationTime)} turns` :
              !this.battleStorageService.currentActor.canAct ?
              `Characted cannot act this time` :
              undefined
          };
        } else {
          if (this.skillList[i].smartObject !== this.battleStorageService.currentActor.skills[i]) {
            this.skillList[i].title = this.battleStorageService.currentActor.skills[i].name;
            this.skillList[i].smartValue = this.battleStorageService.currentActor.skills[i].id === this.currentSkillId ? 1 : 0;
            this.skillList[i].smartObject = this.battleStorageService.currentActor.skills[i];
          }
          this.skillList[i].disabled =
            this.battleStorageService.currentActor.skills[i].cost > this.battleStorageService.currentActor.actionPoints ?
            `Not enough action points. Need ${this.battleStorageService.currentActor.skills[i].cost}` :
            this.battleStorageService.currentActor.skills[i].preparationTime > 0 ?
            `Skill will be available after ${Math.floor(this.battleStorageService.currentActor.skills[i].preparationTime)} turns` :
            !this.battleStorageService.currentActor.canAct ?
            `Characted cannot act this time` :
            undefined;
        }
      }
    } else {
      this.skillList.length = 0;
    }
  }

  private processNextActionFromQueue() {
    this.receivingMessagesFromHubAllowed = false;
    if (this.battleStorageService.version + 1 < this.arenaHub.firstActionVersion) {
      this.loadingService.startLoading({
        title: 'Desynchronization. Page will be refreshed in 2 seconds.'
      }, 0, true);
      console.error('Version is not correct');
      setTimeout(() => {
   //     location.reload();
      }, 2000);
      this.receivingMessagesFromHubAllowed = true;
      return;
    }
    const action = this.arenaHub.pickBattleSynchronizationAction(this.battleStorageService.version);
    if (!action) {
      this.receivingMessagesFromHubAllowed = true;
      return;
    }
    let onlySecondPart = false;
    if (this.specificActionResponseForWait) {
      if (!action.sync.actorId ||
        action.sync.actorId !== this.specificActionResponseForWait.actorId) {
        this.loadingService.startLoading({
          title: 'Desynchronization. Page will be refreshed in 2 seconds.'
        }, 0, true);
        console.error('Action is not correct');
        setTimeout(() => {
    //      location.reload();
        }, 2000);
        return;
      }
      onlySecondPart = true;
    }
    const currentPlayer = action.sync.players.find(x => x.id === this.userService.user.id);
    switch (action.action) {
      case BattleSynchronizationActionEnum.StartGame:
        // TODO Add some introducing animations
        this.restoreScene(action.sync);
        if (!this.loadingFinished) {
          this.processNextActionFromQueue();
        }
        return;
    }
    if (!this.battleAnimationsService.generateAnimationsFromSynchronizer(action, onlySecondPart)) {
      this.processNextActionFromQueueWithChecks();
    }
  }

  private processNextActionFromQueueWithChecks(pending: boolean = false) {
    if (!pending) {
      if (this.specificActionResponseForWait) {
        if (this.actionsQueue.length > 0) {
          this.sendActionFromQueue();
        } else {
          this.specificActionResponseForWait = undefined;
        }
      }
      this.recalculateSkillActions();
      if (!this.specificActionResponseForWait) {
        this.battleStorageService.currentInitiativeList.next(this.calculateInitiativeScale());
      }
    }
    this.processNextActionFromQueue();
  }

  private restoreScene(snapshot: Synchronizer) {
    this.battleSynchronizerService.restoreSceneFromSnapshot(snapshot);
    this.battleStorageService.version = snapshot.version;
    this.lastChange = performance.now();
    this.changed = true;
    this.battleStorageService.currentInitiativeList.next(this.calculateInitiativeScale());
    this.recalculateSkillActions();
  }

  private tick(time: number) {
    const oldState = this.tickState;
    this.tickState += 1 * this.tickingFrequency / time;
    if (Math.floor(this.tickState) !== Math.floor(oldState)) {
      this.changed = true;
    }
    if (this.tickState > 1000) {
      this.tickState -= 1000;
    }
  }

  private endGame(declaration: EndGameDeclaration) {
    this.blocked = true;
    if (this.battleStorageService.openedModal) {
      this.battleStorageService.openedModal.close();
    }
    this.modalService.openModal(VictoryModalComponent, declaration);
  }

  private updateScene() {
    if (this.lastChange !== undefined && this.battleStorageService.turnTime !== undefined) {
      const time = performance.now();
      const shift = time - this.lastChange;
      this.lastChange = time;
      if (this.selectedTile) {
        this.selectedTile.duration += shift;
      }
      this.battleStorageService.turnTime -= shift / 1000;
      if (this.animationTicker && this.battleAnimationsService.processNextAnimationFromQueue()) {
        this.changed = true;
      }
      this.animationTicker = !this.animationTicker;
      this.tick(shift);
      for (let i = 0; i < this.battleStorageService.floatingTexts.length; i++) {
        const floatingText = this.battleStorageService.floatingTexts[i];
        floatingText.time += shift;
        if (floatingText.time >= 0) {
          this.changed = true;
          floatingText.height += this.battleStorageService.floatingTextSpeed * shift;
          floatingText.color.a = Math.min(1, (this.battleStorageService.floatingTextTime - floatingText.time) /
            this.battleStorageService.floatingTextTime * 2);
        }
        if (floatingText.time > this.battleStorageService.floatingTextTime) {
          this.battleStorageService.floatingTexts.splice(i, 1);
          i--;
        }
      }
    }
  }

  private updateCycle(context: AsciiBattleComponent) {
    context.updateScene();
    context.redrawScene();
  }
}
