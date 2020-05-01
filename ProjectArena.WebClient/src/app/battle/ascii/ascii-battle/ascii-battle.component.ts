import { Component, OnInit, OnDestroy, ViewChild, ElementRef, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BattleResolverService } from '../../resolvers/battle-resolver.service';
import { AsciiBattleStorageService } from '../services/ascii-battle-storage.service';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';
import { Subscription } from 'rxjs';
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

// TODO Error instead of console.logs

@Component({
  selector: 'app-ascii-battle',
  templateUrl: './ascii-battle.component.html',
  styleUrls: ['./ascii-battle.component.scss']
})
export class AsciiBattleComponent implements OnInit, OnDestroy {

  @ViewChild('battleCanvas', { static: true }) battleCanvas: ElementRef<HTMLCanvasElement>;
  private canvasContext: CanvasRenderingContext2D;

  lastChange: number;

  drawingTimer;
  updatingFrequency = 30;
  changed = false;
  blocked = false;
  mouseBlocked = false;

  private tileWidthInternal = 0;
  private tileHeightInternal = 30;
  readonly defaultWidth = 1180;
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

  firstAnimation = false;
  animationsLoaded = false;
  animationTimer;
  animationFrequency = 30;
  animationsQueue: any[] = [];
  animationReplacements: any[];

  selectedTile: { x: number, y: number };

  tickingFrequency = 2;
  tickState = 0;

  onCloseSubscription: Subscription;
  arenaActionsSubscription: Subscription;
  synchronizationErrorSubscription: Subscription;

  receivingMessagesFromHubAllowed = false;
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

  zoom = 0;

  get canAct() {
    return !this.blocked && !this.specificActionResponseForWait && this.actionsQueue.length === 0 &&
      this.battleStorageService.turnTime > 0 &&
      this.battleStorageService.currentActor?.owner?.id === this.userService.user.id;
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

  constructor(
    private activatedRoute: ActivatedRoute,
    private battleResolver: BattleResolverService,
    private battleStorageService: AsciiBattleStorageService,
    private battleSynchronizerService: AsciiBattleSynchronizerService,
    private arenaHub: ArenaHubService,
    private userService: UserService
  ) {
    this.mouseState.buttonsInfo[0] = {
      pressed: false,
      timeStamp: 0
    };
    this.mouseState.buttonsInfo[2] = {
      pressed: false,
      timeStamp: 0
    };
    this.onCloseSubscription = arenaHub.onClose.subscribe(() => {
      console.log('Connection error');
    });
    this.synchronizationErrorSubscription = arenaHub.synchronizationErrorState.subscribe((value) => {
      if (value) {
        console.log('Synchronization error');
      }
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
  }

  ngOnInit(): void {
    this.tileWidthInternal = this.tileHeightInternal * 0.6;
    this.setupAspectRatio(this.battleCanvas.nativeElement.offsetWidth, this.battleCanvas.nativeElement.offsetHeight);
    this.canvasContext = this.battleCanvas.nativeElement.getContext('2d');
    this.battleStorageService.version = 0;
    const loadBattle = this.activatedRoute.snapshot.data.battle;
    if (loadBattle) {
      console.log('Restore game');
      const snapshot = this.battleResolver.popBattleSnapshot();
      this.battleSynchronizerService.restoreSceneFromSnapshot(snapshot);
      this.battleStorageService.version = snapshot.version;
      this.lastChange = performance.now();
      this.changed = true;
      this.battleStorageService.currentInitiativeList.next(this.calculateInitiativeScale());
    }
    this.drawingTimer = setInterval(this.updateCycle, this.updatingFrequency, this);
    this.processNextActionFromQueue();
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
    this.selectedTile = x && y ? {x, y} : undefined;
  }

  onMouseDown(event: MouseEvent) {
    this.mouseState.buttonsInfo[event.button] = {pressed: true, timeStamp: event.timeStamp};
  }

  onMouseUp(event: MouseEvent) {
    this.mouseState.buttonsInfo[event.button] = {pressed: false, timeStamp: 0};
    if (!this.blocked) {
      this.recalculateMouseMove(event.x, event.y, event.timeStamp);
      if (event.button === 2) {
        // Context
      }
      if (event.button === 0 && this.canAct) {
        // Action
        const x = Math.floor(this.mouseState.x);
        const y = Math.floor(this.mouseState.y);
        const currentActionSquare = this.battleStorageService.availableActionSquares
        ?.find(s => s.x === x && s.y === y && s.type !== ActionSquareTypeEnum.Actor);
        if (currentActionSquare) {
          // TODO cast spells
          this.actionsQueue = [
            ...currentActionSquare.parentSquares.filter(s => s.type !== ActionSquareTypeEnum.Actor)
              .reverse().map(s => {
                return {
                actorId: this.battleStorageService.currentActor.id,
                x: s.x,
                y: s.y,
                action: s.type === ActionSquareTypeEnum.Move ? BattleSynchronizationActionEnum.Move : BattleSynchronizationActionEnum.Attack
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
  }

  onMouseLeave() {
    for (const state of Object.values(this.mouseState.buttonsInfo)) {
      state.pressed = false;
      state.timeStamp = 0;
    }
  }

  onMouseMove(event: MouseEvent) {
    this.mouseState.realX = event.x;
    this.mouseState.realY = event.y;
    if (!this.mouseBlocked) {
      this.recalculateMouseMove(event.x, event.y, event.timeStamp);
    }
  }

  onKeyPress(event: KeyboardEvent) {

  }

  onKeyDown(event: KeyboardEvent) {
    /*const action = this.gameSettingsService.smartActionsKeyBindings[event.key];
    if (action && !this.blocked) {
      const thisAction = this.pressedKey && this.pressedKey.action === action;
      if (thisAction && this.pressedKey.pressedTime > 0) {
        return;
      }
      if (!thisAction) {
        this.pressedKey = {
          pressedTime: 120,
          action,
          key: event.key
        };
      } else {
        this.pressedKey.pressedTime = 120;
      }
      const x = this.pressedKey.action.xShift + this.gameStateService.playerX;
      const y = this.pressedKey.action.yShift + this.gameStateService.playerY;
      const validation = this.engineFacadeService.validateSmartAction(x, y);
      if (validation.success) {
        this.doSmartAction(x, y);
      } else if (validation.reason) {
        for (const logItem of this.log) {
          logItem.expiring = true;
        }
        this.drawAnimationMessage({
          level: ReactionMessageLevelEnum.Information,
          message: validation.reason
        });
      }
    }
    if (action) {
    }*/
  }

  onKeyUp(event: KeyboardEvent) {
    /*if (this.pressedKey && this.pressedKey.key === event.key) {
      this.pressedKey = undefined;
    }*/
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
      const cameraLeft = this.battleStorageService.cameraX - this.canvasWidth / 2 / this.tileWidth + 0.5;
      const cameraTop = this.battleStorageService.cameraY - this.canvasHeight / 2 / this.tileHeight + 0.5;
      const newX = x / this.zoom / this.tileWidth + cameraLeft;
      const newY = y / this.zoom / this.tileHeight + cameraTop;
      this.mouseState.x = newX;
      this.mouseState.y = newY;
      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);
      if (this.battleStorageService.scene && mouseX >= 0 && mouseY >= 0 &&
        mouseX < this.battleStorageService.scene.width && mouseY < this.battleStorageService.scene.height) {
        this.selectedTile = {x: mouseX, y: mouseY};
      }
      this.changed = true;
    }
  }

   // Drawing
   private brightImpact(bright: boolean, color: Color) {
    if (bright) {
      return {
        r: color.r * 0.2,
        g: color.g * 0.2,
        b: color.b * 0.2,
        a: color.a
      };
    }
    return color;
  }

  private heightImpact(z: number, color: Color): Color {
    if (z !== 0) {
      return {
        r: Math.min(255, Math.max(0, color.r * (1 + z / 200))),
        g: Math.min(255, Math.max(0, color.g * (1 + z / 200))),
        b: Math.min(255, Math.max(0, color.b * (1 + z / 200))),
        a: color.a
      };
    }
    return color;
  }

  private drawPoint(scene: Scene, x: number, y: number, cameraLeft: number, cameraTop: number, drawChar: Visualization) {
    const tile = scene.tiles[x][y];
    if (tile) {
      const canvasX = (x - cameraLeft) * this.tileWidth;
      const canvasY = (y - cameraTop) * this.tileHeight;
      const symbolY = canvasY + this.tileHeight * 0.75;
      if (tile.backgroundColor) {
        const color = this.heightImpact(tile.height, tile.backgroundColor);
        this.canvasContext.fillStyle = `rgb(${color.r}, ${color.g}, ${color.b})`;
        this.canvasContext.fillRect(canvasX, canvasY, this.tileWidth + 1, this.tileHeight + 1);
      }
      const selected = this.selectedTile && this.selectedTile.x === x && this.selectedTile.y === y;
      if (drawChar) {
        const color = this.brightImpact(tile.bright, drawChar.color);
        this.canvasContext.fillStyle = `rgba(${color.r},${color.g},${color.b},${color.a})`;
        this.canvasContext.fillText(drawChar.char, canvasX, symbolY);
      } else
      /*let replacement;
      if (this.animationReplacements) {
        replacement = this.animationReplacements.find(o => o.x === x && o.y === y);
      }
      if (replacement) {
        const color = this.brightImpact(tile.bright, replacement.color);
        this.canvasContext.fillStyle = `rgba(${color.r},${color.g},${color.b},${color.a})`;
        this.canvasContext.fillText(replacement.character, canvasX, symbolY);
      } else*/
      if ((tile.actor || tile.decoration) && ((tile.specEffects.length === 0 && !selected) || Math.floor(this.tickState) % 2 === 1)) {
        if (tile.actor) {
          const color = this.heightImpact(tile.actor.z, this.brightImpact(tile.bright,
            tile.actor === this.battleStorageService.currentActor &&
            this.battleStorageService.currentActor.owner?.id === this.userService.user.id ?
            {r: 255, g: 255, b: 0, a: tile.actor.visualization.color.a} :
            tile.actor.visualization.color));
          this.canvasContext.fillStyle = `rgba(${color.r},${color.g},${color.b},${color.a})`;
          this.canvasContext.fillText(tile.actor.visualization.char, canvasX, symbolY);
        } else if (tile.decoration) {
          const color = this.heightImpact(tile.actor.z, this.brightImpact(tile.bright, tile.actor.visualization.color));
          this.canvasContext.fillStyle = `rgba(${color.r},${color.g},${color.b},${color.a})`;
          this.canvasContext.fillText(tile.actor.visualization.char, canvasX, symbolY);
        }
      } else if (tile.specEffects.length > 0 && selected) {
        const firstEffect = tile.specEffects[0];
        const color = this.heightImpact(tile.actor.z, this.brightImpact(tile.bright, firstEffect.visualization.color));
        this.canvasContext.fillStyle = `rgba(${color.r},${color.g},${color.b},${color.a})`;
        this.canvasContext.fillText(firstEffect.visualization.char, canvasX, symbolY);
      } else {
        const color = this.heightImpact(tile.height, tile.visualization.color);
        this.canvasContext.fillStyle = `rgba(${color.r}, ${color.g},
          ${color.b}, ${color.a})`;
        this.canvasContext.fillText(tile.visualization.char, canvasX, symbolY);
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
    if (!this.changed) {
      return;
    }
    this.changed = false;
    const scene = this.battleStorageService.scene;
    if (scene) {
      const cameraLeft = this.battleStorageService.cameraX - this.canvasWidth / 2 / this.tileWidth + 0.5;
      const cameraTop = this.battleStorageService.cameraY - this.canvasHeight / 2 / this.tileHeight + 0.5;
      this.canvasContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
      this.canvasContext.font = `${this.tileHeight}px PT Mono`;
      const left = Math.max(0, Math.floor(cameraLeft));
      const right = Math.min(scene.width - 1, Math.ceil(cameraLeft + this.canvasWidth / (this.tileWidth)));
      const top = Math.max(0, Math.floor(cameraTop));
      const bottom = Math.min(scene.height - 1, Math.ceil(cameraTop + this.canvasHeight / (this.tileHeight)));

      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);
      const currentActionSquare = this.battleStorageService.availableActionSquares
        ?.find(s => s.x === mouseX && s.y === mouseY && s.type !== ActionSquareTypeEnum.Actor);
      for (let x = left; x <= right; x++) {
        for (let y = top; y <= bottom; y++ ) {
          let drawChar;
          if (currentActionSquare?.x === x && currentActionSquare?.y === y) {
            drawChar = {
              char: 'x',
              color: currentActionSquare.type === ActionSquareTypeEnum.Act ? {r: 255, g: 0, b: 0, a: 1} : {r: 255, g: 255, b: 0, a: 1}
            };
          } else if (currentActionSquare?.type === ActionSquareTypeEnum.Act && currentActionSquare.parentSquares.length > 0 &&
            currentActionSquare?.parentSquares[0].x === x && currentActionSquare?.parentSquares[0].y === y &&
            currentActionSquare?.parentSquares[0].type !== ActionSquareTypeEnum.Actor) {
            drawChar = {
              char: 'x',
              color: {r: 255, g: 255, b: 0, a: 1}
            };
          } else if (currentActionSquare?.parentSquares.some(s => s.x === x && s.y === y && s.type !== ActionSquareTypeEnum.Actor)) {
            drawChar = {
              char: '.',
              color: {r: 255, g: 255, b: 0, a: 1}
            };
          }
          this.drawPoint(scene, x, y, cameraLeft, cameraTop, drawChar);
        }
      }
      /*if (currentTile) {
        this.canvasContext.strokeStyle = `rgba(${255}, ${0}, ${0}, 0.4)`;
        this.canvasContext.lineWidth = 2;
        this.canvasContext.strokeRect(currentTile.canvasX + 2, currentTile.canvasY + 2, this.tileWidth - 4, this.tileHeight - 4);
      }*/
      if (this.battleStorageService.availableActionSquares?.length > 0) {
        const path = this.generateActionSquareGrid(cameraLeft, cameraTop);
        this.canvasContext.strokeStyle = `rgba(${255}, ${255}, ${0}, 0.8)`;
        this.canvasContext.lineWidth = 2;
        this.canvasContext.stroke(path);
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
          this.arenaHub.orderMove(newAction.actorId, newAction.x, newAction.y);
        } else {
          console.log('cannot move');
          this.specificActionResponseForWait = undefined;
        }
        break;
      case BattleSynchronizationActionEnum.Attack:
        if (newAction.actorId === this.battleStorageService.currentActor.id &&
          (sX === 1 && sY === 0 || sX === 0 && sY === 1) && this.battleStorageService.currentActor.canAct &&
          (!this.battleStorageService.currentActor.attackingSkill.meleeOnly || Math.abs(tile.height - initialTile.height) < 10) &&
          (tile.actor || tile.decoration) && tile.actor !== this.battleStorageService.currentActor) {
          this.arenaHub.orderAttack(newAction.actorId, newAction.x, newAction.y);
        } else {
          console.log('cannot attack');
          this.specificActionResponseForWait = undefined;
        }
        break;
      default:
        console.log('Unprocessible action');
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
        if (allPortraits[i].initiativePosition < candidateInitiative) {
          candidate = allPortraits[i];
          candidateInitiative = allPortraits[i].initiativePosition;
        }
      }
      portraits.push(candidate);
      candidate.initiativePosition += 1 / candidate.speed;
    }
    return portraits;
  }

  private processNextActionFromQueue() {
    this.receivingMessagesFromHubAllowed = false;
    if (this.battleStorageService.version + 1 < this.arenaHub.firstActionVersion) {
      console.log('Version issue');
      this.receivingMessagesFromHubAllowed = true;
      return;
    }
    const action = this.arenaHub.pickBattleSynchronizationAction(this.battleStorageService.version);
    if (!action) {
      this.receivingMessagesFromHubAllowed = true;
      return;
    }
    const currentPlayer = action.sync.players.find(x => x.id === this.userService.user.id);
    if (currentPlayer.status === BattlePlayerStatusEnum.Defeated) {
      console.log('DEFEAT');
      return;
    }
    switch (action.action) {
      case BattleSynchronizationActionEnum.StartGame:
        console.log('Start game');
        // TODO Add some introducing animations
        this.battleSynchronizerService.restoreSceneFromSnapshot(action.sync);
        this.lastChange = performance.now();
        break;
      case BattleSynchronizationActionEnum.EndGame:
        if (currentPlayer.status === BattlePlayerStatusEnum.Victorious) {
          console.log('VICTORY');
        }
        this.battleSynchronizerService.synchronizeScene(action.sync);
        return;
      case BattleSynchronizationActionEnum.NoActorsDraw:
        console.log('DRAW');
        this.battleSynchronizerService.synchronizeScene(action.sync);
        break;
      case BattleSynchronizationActionEnum.SkipTurn:
        console.log('Skip turn');
        this.battleSynchronizerService.synchronizeScene(action.sync);
        break;
      case BattleSynchronizationActionEnum.EndTurn:
        console.log('End turn');
        this.battleSynchronizerService.synchronizeScene(action.sync);
        break;
      case BattleSynchronizationActionEnum.Attack:
        console.log('Attack');
        this.battleSynchronizerService.synchronizeScene(action.sync);
        break;
      case BattleSynchronizationActionEnum.Cast:
        console.log('Cast');
        this.battleSynchronizerService.synchronizeScene(action.sync);
        break;
      case BattleSynchronizationActionEnum.Decoration:
        console.log('Decoration acts');
        this.battleSynchronizerService.synchronizeScene(action.sync);
        break;
      case BattleSynchronizationActionEnum.Move:
        console.log('Move');
        this.battleSynchronizerService.synchronizeScene(action.sync);
        break;
      case BattleSynchronizationActionEnum.Wait:
        console.log('Wait');
        this.battleSynchronizerService.synchronizeScene(action.sync);
        break;
    }
    this.changed = true;
    this.battleStorageService.version = action.sync.version;
    if (this.specificActionResponseForWait) {
      if (!action.sync.tempActor ||
        action.sync.tempActor !== this.specificActionResponseForWait.actorId) {
        console.log('Awaiting action version issue');
        return;
      }
      if (this.actionsQueue.length > 0) {
        this.sendActionFromQueue();
      } else {
        this.specificActionResponseForWait = undefined;
      }
    }
    if (!this.specificActionResponseForWait) {
      this.battleStorageService.currentInitiativeList.next(this.calculateInitiativeScale());
    }
    setTimeout(() => this.processNextActionFromQueue());
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

  private updateScene() {
    if (this.lastChange !== undefined && this.battleStorageService.turnTime !== undefined) {
      const time = performance.now();
      const shift = time - this.lastChange;
      this.lastChange = time;
      this.battleStorageService.turnTime -= shift / 1000;
      this.tick(shift);
    }
  }

  private updateCycle(context: AsciiBattleComponent) {
    context.updateScene();
    context.redrawScene();
  }
}
