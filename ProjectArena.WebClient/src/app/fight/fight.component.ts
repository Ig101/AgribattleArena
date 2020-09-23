import { Component, OnInit, OnDestroy, ViewChild, ElementRef, HostListener } from '@angular/core';
import { LoadingService } from '../shared/services/loading.service';
import { SceneService } from '../engine/services/scene.service';
import { SynchronizationService } from '../engine/services/synchronization.service';
import { FullSynchronizationInfo } from '../shared/models/synchronization/full-synchronization-info.model';
import { ActorSynchronization } from '../shared/models/synchronization/objects/actor-synchronization.model';
import { Subscription, Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ModalService } from '../shared/services/modal.service';
import { CharsService } from '../shared/services/chars.service';
import { AssetsLoadingService } from '../shared/services/assets-loading.service';
import { Random } from '../shared/random/random';
import { fillVertexPosition, drawArrays, fillTileMask, fillBackground, fillColor, fillChar } from '../helpers/webgl.helper';
import { ActionSquareTypeEnum } from '../battle/ascii/models/enum/action-square-type.enum';
import { getRandomBiom } from '../shared/bioms/biom.helper';
import { Color } from '../shared/models/color.model';
import { heightImpact, brightImpact, backgroundImpact } from '../battle/ascii/helpers/scene-draw.helper';
import { Scene } from '../engine/scene/scene.object';
import { BiomEnum } from '../shared/models/enum/biom.enum';
import { Actor } from '../engine/scene/actor.object';
import { Tile } from '../engine/scene/tile.object';
import { DEFAULT_HEIGHT, RANGED_RANGE, VISIBILITY_AMPLIFICATION, LARGE_ACTOR_TRESHOLD_VOLUME } from '../content/content.helper';
import { angleBetween, rangeBetween } from '../helpers/math.helper';
import { MouseState } from '../shared/models/mouse-state.model';
import { ActionClassEnum } from '../engine/models/enums/action-class.enum';
import { SmartAction } from './models/smart-action.model';
import { getMostPrioritizedAction } from '../engine/engine.helper';
import { Action } from '../engine/models/abstract/action.model';
import { IActor } from '../engine/interfaces/actor.interface';
import { IModal } from '../shared/interfaces/modal.interface';
import { ModalPositioning } from './models/modal-positioning.model';
import { ContextMenuComponent } from './context-menu/context-menu.component';
import { ContextMenuContext } from './models/context-menu-context.model';
import { BattlePlayerStatusEnum } from '../shared/models/enum/player-battle-status.enum';
import { UserService } from '../shared/services/user.service';
import { TargetChooseModalComponent } from './modals/target-choose-modal/target-choose-modal.component';

@Component({
  selector: 'app-fight',
  templateUrl: './fight.component.html',
  styleUrls: ['./fight.component.scss']
})
export class FightComponent implements OnInit, OnDestroy {

  @ViewChild('battleCanvas', { static: true }) battleCanvas: ElementRef<HTMLCanvasElement>;
  @ViewChild('hudCanvas', { static: true }) hudCanvas: ElementRef<HTMLCanvasElement>;
  private canvas2DContext: CanvasRenderingContext2D;
  private canvasWebGLContext: WebGLRenderingContext;
  private charsTexture: WebGLTexture;
  private shadersProgram: WebGLProgram;

  mouseState: MouseState = {
    buttonsInfo: {},
    x: -1,
    y: -1,
    realX: -1,
    realY: -1
  };

  blocked = false;

  private tileWidthInternal = 0;
  private tileHeightInternal = 120;
  readonly defaultWidth = 1600;
  readonly defaultHeight = 1080;
  readonly defaultAspectRatio = this.defaultWidth / this.defaultHeight;

  zoom = 0;

  tickerState = false;
  tickerTime: number;
  tickerPeriod = (1000 / 10);

  cameraX: number;
  cameraY: number;
  battleZoom: number;

  updateSubscription: Subscription;

  rangeMap: boolean[][]; // undefined for nothing, false for red, true for yellow
  textureMapping: Float32Array;
  colors: Uint8Array;
  activities: Uint8Array;
  backgroundTextureMapping: Float32Array;
  backgrounds: Uint8Array;
  mainTextureVertexes: Float32Array;
  healthActors: Actor[];

  redPath: Path2D;
  greenPath: Path2D;

  allowedTargetPath: Path2D;

  cursorVertexes: {
    position: number;
    textureMapping: Float32Array;
    colors: Uint8Array;
    activity: number;
  };
  selectedActor: Actor;
  selectedByModal: boolean;

  smartAlt: SmartAction;
  smartDirections: SmartAction[];
  directionTimer: number;

  openedModal: IModal<unknown>;
  modalPositioning: ModalPositioning;
  positioningUpdateSubject = new Subject<any>();
  actionModal: boolean;

  chosenAction: Action;

  get canvasWidth() {
    return this.battleCanvas.nativeElement.width;
  }

  get canvasHeight() {
    return this.battleCanvas.nativeElement.height;
  }

  get tileWidth() {
    return this.tileWidthInternal * this.battleZoom;
  }

  get tileHeight() {
    return this.tileHeightInternal * this.battleZoom;
  }

  get leftInterfaceShift() {
    return 30 / this.zoom;
  }

  get interfaceShift() {
    return 362 / this.zoom;
  }

  get scene() {
    return this.sceneService.scene;
  }

  get canAct() {
    return !this.blocked &&
      this.canActNoBlocked;
  }

  get canActNoBlocked() {
    const can = this.scene &&
      this.scene.currentActor?.isAlive &&
      !this.scene.automatic;
    if (!can && !this.blocked) {
      this.chosenAction = undefined;
    }
    return can;
  }

  get rangeMapIsActive() {
    return this.scene &&
      this.chosenAction &&
      !this.chosenAction.native.untargeted &&
      this.scene.currentActor?.owner?.id === this.scene.currentPlayer.id &&
      this.scene.currentActor.actions.some(x => x.native.actionClass === ActionClassEnum.Default && !x.native.untargeted);
  }

  get userName() {
    return this.userService.user?.name || 'Player not found';
  }

  constructor(
    private loadingService: LoadingService,
    private sceneService: SceneService,
    private userService: UserService,
    private activatedRoute: ActivatedRoute,
    private modalService: ModalService,
    private charsService: CharsService,
    private assetsLoadingService: AssetsLoadingService
  ) {
    this.mouseState.buttonsInfo[0] = {
      pressed: false,
      timeStamp: 0
    };
    this.mouseState.buttonsInfo[2] = {
      pressed: false,
      timeStamp: 0
    };
    this.smartAlt = {
      hotKey: 'ShiftLeft',
      keyVisualization: 'Shift',
      title: 'Alternative',
      pressed: false,
      action: undefined
    };
    this.smartDirections = [
      {
        hotKey: 'KeyA',
        keyVisualization: 'A',
        title: 'Left',
        pressed: false,
        action: () => this.directActorTo(this.scene.currentActor?.x - 1, this.scene.currentActor?.y)
      },
      {
        hotKey: 'KeyD',
        keyVisualization: 'D',
        title: 'Right',
        pressed: false,
        action: () => this.directActorTo(this.scene.currentActor?.x + 1, this.scene.currentActor?.y)
      },
      {
        hotKey: 'KeyW',
        keyVisualization: 'W',
        title: 'Up',
        pressed: false,
        action: () => this.directActorTo(this.scene.currentActor?.x, this.scene.currentActor?.y - 1)
      },
      {
        hotKey: 'KeyS',
        keyVisualization: 'S',
        title: 'Down',
        pressed: false,
        action: () => this.directActorTo(this.scene.currentActor?.x, this.scene.currentActor?.y + 1)
      }
    ];
  }

  ngOnInit(): void {
    this.tileWidthInternal = this.tileHeightInternal * 0.6;
    this.setupAspectRatio(this.battleCanvas.nativeElement.offsetWidth, this.battleCanvas.nativeElement.offsetHeight);
    this.canvasWebGLContext = this.battleCanvas.nativeElement.getContext('webgl');
    this.canvas2DContext = this.hudCanvas.nativeElement.getContext('2d');
    this.canvas2DContext.font = `${26}px PT Mono`;
    this.canvas2DContext.textAlign = 'center';
    this.canvas2DContext.globalAlpha = 1.0;
    this.createSampleScene();
    this.cameraX = this.scene.width / 2;
    this.cameraY = this.scene.height / 2;
    this.battleZoom = 1;
    this.updateSubscription = this.sceneService.updateSub.subscribe((shift) => {
      this.redraw(shift);
    });
    const mapSize = RANGED_RANGE * 2 + 1;
    this.rangeMap = new Array<boolean[]>(mapSize);
    for (let i = 0; i < mapSize; i++) {
      this.rangeMap[i] = new Array<boolean>(mapSize);
    }

    this.assetsLoadingService.loadShadersAndCreateProgram(
      this.canvasWebGLContext,
      'vertex-shader-2d.vert',
      'fragment-shader-2d.frag'
    )
      .subscribe((result) => {
        this.charsTexture = this.charsService.getTexture(this.canvasWebGLContext);
        this.shadersProgram = result;
        this.redraw();
        setTimeout(() => {
          this.loadingService.finishLoading()
          .subscribe(() => {
            this.sceneService.startUpdates();
          });
        }, 200);
      });
  }

  ngOnDestroy(): void {
    this.updateSubscription.unsubscribe();
    this.sceneService.clearScene();
    this.positioningUpdateSubject.unsubscribe();
  }

  createSampleScene() {
    let idCounterPosition = 1000;
    const actors: ActorSynchronization[] = [];
    let tilesCounter = 1;
    for (let x = 0; x < 14; x++) {
      for (let y = 0; y < 8; y++) {
        if (x < 4 || x > 6 || y !== 2) {
          actors.push({
            reference: {
              id: idCounterPosition++,
              x,
              y
            },
            position: 1,
            left: false,
            name: 'Ground',
            char: 'ground',
            color: {r: 60, g: 61, b: 95, a: 1},
            ownerId: undefined,
            tags: ['tile'],
            parentId: tilesCounter,
            durability: 10000,
            maxDurability: 10000,
            initiative: 1,
            height: x > 8 && y > 3 ? 900 : 500,
            volume: 10000,
            freeVolume: 9000,
            preparationReactions: [],
            activeReactions: [],
            clearReactions: [],
            actions: [],
            actors: [],
            buffs: [],
          });
          actors.push({
            reference: {
              id: idCounterPosition++,
              x,
              y
            },
            position: 1,
            left: false,
            name: 'Grass',
            char: 'grass',
            color: { r: 45, g: 60, b: 150, a: 1 },
            ownerId: undefined,
            tags: ['tile'],
            parentId: tilesCounter,
            durability: 1,
            maxDurability: 1,
            initiative: 1,
            height: 5,
            volume: 250,
            freeVolume: 0,
            preparationReactions: [],
            activeReactions: [],
            clearReactions: [],
            actions: [],
            actors: [],
            buffs: [],
          });
        }
        tilesCounter++;
      }
    }
    actors.push({
      reference: {
        id: idCounterPosition,
        x: 12,
        y: 7
      },
      position: 1,
      left: false,
      name: 'Actor',
      char: 'adventurer',
      color: { r: 0, g: 0, b: 255, a: 1 },
      ownerId: 'sampleP2',
      tags: ['active'],
      parentId: 1 + 12 * 8 + 7,
      durability: 200,
      maxDurability: 200,
      initiative: 1,
      height: 180,
      volume: 120,
      freeVolume: 40,
      preparationReactions: [],
      activeReactions: [],
      clearReactions: [],
      actions: [
        {
          id: 'move',
          isAutomatic: false,
          blocked: false,
          remainedTime: 0
        }
      ],
      actors: [],
      buffs: [],
    });
    actors.push({
      reference: {
        id: ++idCounterPosition,
        x: 13,
        y: 6
      },
      position: 1,
      left: false,
      name: 'Hero',
      char: 'adventurer',
      color: { r: 255, g: 155, b: 55, a: 1 },
      ownerId: 'sampleP',
      tags: ['active'],
      parentId: 1 + 13 * 8 + 6,
      durability: 200,
      maxDurability: 200,
      initiative: 1,
      height: 180,
      volume: 120,
      freeVolume: 40,
      preparationReactions: [],
      activeReactions: [],
      clearReactions: [],
      actions: [
        {
          id: 'move',
          isAutomatic: false,
          blocked: false,
          remainedTime: 0
        },
        {
          id: 'slash',
          isAutomatic: false,
          blocked: false,
          remainedTime: 0
        },
        {
          id: 'wait',
          isAutomatic: false,
          blocked: false,
          remainedTime: 0
        },
        {
          id: 'shot',
          isAutomatic: false,
          blocked: false,
          remainedTime: 0
        }
      ],
      actors: [],
      buffs: [],
    });
    this.sceneService.setupGame(
      {
        id: 'sampleS',
        timeLine: 0,
        idCounterPosition,
        currentPlayerId: 'sampleP',
        actors,
        players: [
          {
            id: 'sampleP',
            battlePlayerStatus: BattlePlayerStatusEnum.Playing
          },
          {
            id: 'sampleP2',
            battlePlayerStatus: BattlePlayerStatusEnum.Playing
          }
        ],
        width: 14,
        height: 8,
        biom: BiomEnum.Grass,
        waitingActions: []
      }
    );
  }

  changeSelection(actor: Actor) {
    if (!this.selectedByModal) {
      this.selectedActor = actor;
    }
  }

  directActorTo(x: number, y: number) {
    if (this.canAct && this.directionTimer <= 0 && x >= 0 && y >= 0 && x < this.scene.width && y < this.scene.height) {
      const tile = this.scene.tiles[x][y];
      let action: Action;
      let actor: IActor;
      let onTarget: boolean;
      if (!this.scene.currentActor.parentActor.isRoot) {
        x = this.scene.currentActor.x;
        y = this.scene.currentActor.y;
      }
      if (this.smartAlt.pressed) {
        action = getMostPrioritizedAction(
          this.scene.currentActor.actions.filter(a =>
            a.native.actionClass === ActionClassEnum.Attack &&
            !this.scene.currentActor.validateTargeted(a, x, y)));
        onTarget = true;
        if (action?.native.actionOnObject) {
          onTarget = false;
          const actors: Actor[] = [];
          if (this.scene.currentActor.parentActor.isRoot) {
            for (let i = tile.actors.length - 1; i >= 0; i--) {
              const tempActor = tile.actors[i];
              actors.push(tempActor);
              if (tempActor.tags.includes('tile')) {
                break;
              }
            }
            if (actors.length === 0) {
              actor = tile;
            }
          } else {
            actors.push(this.scene.currentActor.parentActor as Actor);
          }
          if (actors.length > 0) {
            this.blocked = true;
            this.actionModal = true;
            this.selectedByModal = true;
            this.mouseState.x = x;
            this.mouseState.y = y;
            this.modalPositioning = {
              left: (Math.floor(this.mouseState.x) + 0.3 - this.cameraX +
                (this.canvasWidth - this.interfaceShift + this.leftInterfaceShift) / 2 /
                this.tileWidth) * this.zoom * this.tileWidth,
              top: (Math.floor(this.mouseState.y) + 0.5 - this.cameraY + this.canvasHeight / 2 /
                this.tileHeight) * this.zoom * this.tileHeight,
              textHeight: this.tileHeight * this.zoom,
              updateSubject: this.positioningUpdateSubject
            } as ModalPositioning;
            const selectionsObserver = new Subject<Actor>();
            selectionsObserver.subscribe(chosenActor => {
              this.selectedActor = chosenActor;
            });
            this.openedModal = this.modalService.openModal(TargetChooseModalComponent, {
              modalPosition: this.modalPositioning,
              selectionsObserver,
              actors
            });
            this.openedModal.onClose.subscribe((chosenActor: Actor) => {
              this.blocked = false;
              this.openedModal = undefined;
              this.actionModal = false;
              this.selectedByModal = false;
              selectionsObserver.unsubscribe();
              this.directionTimer = 0.2;
              this.scene.intendedOnObjectAction(action, chosenActor);
            });
            this.openedModal.onCancel.subscribe(_ => {
              this.blocked = false;
              this.openedModal = undefined;
              this.actionModal = false;
              this.selectedByModal = false;
              selectionsObserver.unsubscribe();
            });
            return;
          }
        }
      } else {
        let attack = false;
        if (this.scene.currentActor.parentActor.isRoot) {
          for (let i = tile.actors.length - 1; i >= 0; i--) {
            actor = tile.actors[i];
            if (actor.tags.includes('tile') || actor.tags.includes('flat')) {
              break;
            }
            if (actor.volume >= LARGE_ACTOR_TRESHOLD_VOLUME) {
              attack = true;
              break;
            }
          }
        } else {
          attack = true;
          actor = this.scene.currentActor.parentActor;
        }
        action = getMostPrioritizedAction(
          this.scene.currentActor.actions.filter(a =>
            ((a.native.actionClass === ActionClassEnum.Attack && attack) ||
            (a.native.actionClass === ActionClassEnum.Move && !attack)) &&
            !this.scene.currentActor.validateTargeted(a, x, y)));
        onTarget = !!action?.native.actionTargeted;
      }
      if (action) {
        this.directionTimer = 0.2;
        if (onTarget) {
          this.scene.intendedTargetedAction(action, x, y);
        } else {
          this.scene.intendedOnObjectAction(action, actor || tile);
        }
        return;
      }
    }
    this.directionTimer = 0.04;
  }

  openSettings() {
   /* this.blocked = true;
    this.openedModal = this.modalService.openModalWithoutArgs(
      MenuModalComponent);
    this.openedModal.onClose.subscribe(_ => {
      this.blocked = false;
      this.openedModal = undefined;
    });*/
  }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event) {
    event.preventDefault();
  }

  onResize() {
    this.setupAspectRatio(this.battleCanvas.nativeElement.offsetWidth, this.battleCanvas.nativeElement.offsetHeight);
  }

  private recalculateMouseMove(x: number, y: number, timeStamp?: number) {
    const leftKey = this.mouseState.buttonsInfo[0];
    const rightKey = this.mouseState.buttonsInfo[2];
    if (!rightKey.pressed && !leftKey.pressed) {
      const cameraLeft = this.cameraX - (this.canvasWidth - this.interfaceShift + this.leftInterfaceShift) / 2 / this.tileWidth;
      const cameraTop = this.cameraY - this.canvasHeight / 2 / this.tileHeight;
      const newX = x / this.zoom / this.tileWidth + cameraLeft;
      const newY = y / this.zoom / this.tileHeight + cameraTop;
      this.mouseState.x = newX;
      this.mouseState.y = newY;
      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);
    }
  }

  chooseAction(action: Action) {
    if (action === this.chosenAction) {
      this.chosenAction = undefined;
      return;
    }
    if (!action.native.untargeted) {
      this.chosenAction = action;
      const cameraLeft = this.cameraX - (this.canvasWidth - this.interfaceShift + this.leftInterfaceShift) / 2 / this.tileWidth;
      const cameraTop = this.cameraY - this.canvasHeight / 2 / this.tileHeight;
      this.fillRangeMapAndPathes(
        this.scene.currentActor.z + this.scene.currentActor.height,
        this.scene.currentActor.x,
        this.scene.currentActor.y,
        cameraLeft,
        cameraTop);
    } else {
      this.chosenAction = undefined;
      if (action.native.actionTargeted) {
        this.scene.intendedTargetedAction(action, this.scene.currentActor.x, this.scene.currentActor.y);
      } else {
        const actors = this.scene.currentActor.actors;
        // TODO Open dialog with current actor actors and choose one
        if (actors.length > 0) {
          this.scene.intendedOnObjectAction(action, actors[0]);
        }
      }
    }
  }

  onMouseMove(event: MouseEvent) {
    if (!this.blocked) {
      this.mouseState.realX = event.x;
      this.mouseState.realY = event.y;
      this.recalculateMouseMove(event.x, event.y, event.timeStamp);
    }
  }

  onKeyDown(event: KeyboardEvent) {
    if (this.smartAlt.hotKey === event.code) {
      event.preventDefault();
      this.smartAlt.pressed = true;
      return;
    }
    if (!this.blocked) {
      const directionAction = this.smartDirections.find(x => x.hotKey === event.code);
      if (directionAction) {
        event.preventDefault();
        if (!this.smartDirections.some(x => x.pressed)) {
          this.directionTimer = 0;
          directionAction.action();
          this.directionTimer *= 2;
        }
        directionAction.pressed = true;
        this.smartDirections.sort((a, b) => {
          const bVal = b === directionAction ? 1 : 0;
          const aVal = a === directionAction ? 1 : 0;
          return bVal - aVal;
        });
        return;
      }
    }
  }

  onKeyUp(event: KeyboardEvent) {
    if (this.smartAlt.hotKey === event.code) {
      event.preventDefault();
      this.smartAlt.pressed = false;
      return;
    }
    const directionAction = this.smartDirections.find(x => x.hotKey === event.code);
    if (directionAction) {
      event.preventDefault();
      directionAction.pressed = false;
      return;
    }
  }

  onMouseDown(event: MouseEvent) {
    this.mouseState.buttonsInfo[event.button] = {pressed: true, timeStamp: event.timeStamp};
  }

  cast(x: number, y: number, target?: Actor) {
    const shiftedX = x - this.scene.currentActor.x + RANGED_RANGE;
    const shiftedY = y - this.scene.currentActor.y + RANGED_RANGE;
    if (shiftedX <= RANGED_RANGE * 2 && shiftedY <= RANGED_RANGE * 2 && shiftedX >= 0 && shiftedY >= 0 &&
        this.rangeMap[shiftedX][shiftedY] !== undefined) {
      const tile = this.scene.tiles[x][y];
      let actor: IActor;
      let onTarget: boolean;
      if (this.smartAlt.pressed) {
        onTarget = true;
        if (this.chosenAction.native.actionOnObject) {
          onTarget = false;
          const actors = [];
          if (this.scene.currentActor.parentActor.isRoot) {
            for (let i = tile.actors.length - 1; i >= 0; i--) {
              const tempActor = tile.actors[i];
              actors.push(tempActor);
              if (tempActor.tags.includes('tile')) {
                break;
              }
            }
            if (actors.length === 0) {
              actor = tile;
            }
          } else {
            actors.push(this.scene.currentActor.parentActor);
          }
          if (actors.length > 0) {
            this.blocked = true;
            this.actionModal = true;
            this.mouseState.x = x;
            this.mouseState.y = y;
            this.modalPositioning = {
              left: (Math.floor(this.mouseState.x) + 0.3 - this.cameraX +
                (this.canvasWidth - this.interfaceShift + this.leftInterfaceShift) / 2 /
                this.tileWidth) * this.zoom * this.tileWidth,
              top: (Math.floor(this.mouseState.y) + 0.5 - this.cameraY + this.canvasHeight / 2 /
                this.tileHeight) * this.zoom * this.tileHeight,
              textHeight: this.tileHeight * this.zoom,
              updateSubject: this.positioningUpdateSubject
            } as ModalPositioning;
            const selectionsObserver = new Subject<Actor>();
            selectionsObserver.subscribe(chosenActor => {
              this.selectedActor = chosenActor;
            });
            this.openedModal = this.modalService.openModal(TargetChooseModalComponent, {
              modalPosition: this.modalPositioning,
              selectionsObserver,
              actors
            });
            this.openedModal.onClose.subscribe((chosenActor: Actor) => {
              this.blocked = false;
              this.openedModal = undefined;
              this.actionModal = false;
              selectionsObserver.unsubscribe();
              this.directionTimer = 0.2;
              this.scene.intendedOnObjectAction(this.chosenAction, chosenActor);
              this.chosenAction = undefined;
            });
            this.openedModal.onCancel.subscribe(_ => {
              this.blocked = false;
              this.openedModal = undefined;
              this.actionModal = false;
              selectionsObserver.unsubscribe();
            });
            return;
          }
          actor = actors.find(a => a.volume >= LARGE_ACTOR_TRESHOLD_VOLUME) || actors[0];
        }
      } else {
        if (target) {
          actor = target;
        } else if (this.scene.currentActor.parentActor.isRoot) {
          for (let i = tile.actors.length - 1; i >= 0; i--) {
            actor = tile.actors[i];
            if (actor.tags.includes('tile') || actor.tags.includes('flat') || actor.volume >= LARGE_ACTOR_TRESHOLD_VOLUME) {
              break;
            }
          }
        } else {
          actor = this.scene.currentActor.parentActor;
        }
        onTarget = !!this.chosenAction.native.actionTargeted;
      }
      if (onTarget) {
        this.scene.intendedTargetedAction(this.chosenAction, x, y);
      } else {
        this.scene.intendedOnObjectAction(this.chosenAction, actor || tile);
      }
      this.chosenAction = undefined;
    }
  }

  onExternalClick(actor: Actor) {
    if (this.canAct) {
      const x = actor.x;
      const y = actor.y;
      if (this.rangeMapIsActive) {
        this.cast(x, y, actor);
      }
    }
  }

  onMouseUp(event: MouseEvent) {
    this.mouseState.buttonsInfo[event.button] = {pressed: false, timeStamp: 0};
    this.recalculateMouseMove(event.x, event.y, event.timeStamp);
    if (!this.blocked) {
      if (this.canAct && this.rangeMapIsActive && event.button === 0) {
        const x = Math.floor(this.mouseState.x);
        const y = Math.floor(this.mouseState.y);
        this.cast(x, y);
      }
    }
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
    this.hudCanvas.nativeElement.width = this.battleCanvas.nativeElement.width;
    this.hudCanvas.nativeElement.height = this.battleCanvas.nativeElement.height;
    this.zoom = this.battleCanvas.nativeElement.offsetWidth / this.canvasWidth;
    if (this.modalPositioning) {
      this.modalPositioning.left = (Math.floor(this.mouseState.x) + 0.3 - this.cameraX +
        (this.canvasWidth - this.interfaceShift + this.leftInterfaceShift) / 2 /
        this.tileWidth) * this.zoom * this.tileWidth;
      this.modalPositioning.top = (Math.floor(this.mouseState.y) + 0.5 - this.cameraY + this.canvasHeight / 2 /
        this.tileHeight) * this.zoom * this.tileHeight;
      this.modalPositioning.textHeight = this.tileHeight * this.zoom;
      this.positioningUpdateSubject.next();
    }
    if (this.scene) {
      this.scene.visualizationChanged = true;
    }
    this.redraw();
  }

  private fillRangeMapPart(
    actorZ: number,
    actorX: number,
    actorY: number,
    startPointX: number,
    startPointY: number,
    hasNoVisibleSkills: boolean
  ) {
    let nextX = actorX;
    let nextY = actorY;
    let visible = true;
    const angle = angleBetween(actorX, actorY, startPointX, startPointY);
    const sin = Math.sin(angle) * 0.8;
    const cos = Math.cos(angle) * 0.8;
    while (Math.round(nextX) !== startPointX || Math.round(nextY) !== startPointY) {
      let currentX = Math.round(nextX);
      let currentY = Math.round(nextY);
      while (Math.round(nextX) === currentX && Math.round(nextY) === currentY) {
        nextX += cos;
        nextY += sin;
      }
      currentX = Math.round(nextX);
      currentY = Math.round(nextY);
      if (currentX < 0 || currentY < 0 || currentX >= this.scene.width || currentY >= this.scene.height) {
        return;
      }
      const range = rangeBetween(actorX, actorY, currentX, currentY);
      if (range > RANGED_RANGE) {
        return;
      }
      if (this.rangeMap[currentX - actorX + RANGED_RANGE][currentY - actorY + RANGED_RANGE] !== true) {
        if (!this.scene.currentActor.validateTargeted(this.chosenAction, currentX, currentY) &&
          (visible || !this.chosenAction.native.onlyVisible)) {
          this.rangeMap[currentX - actorX + RANGED_RANGE][currentY - actorY + RANGED_RANGE] = visible;
        } else {
          this.rangeMap[currentX - actorX + RANGED_RANGE][currentY - actorY + RANGED_RANGE] = undefined;
        }
      }
      if (this.scene.tiles[currentX][currentY].height > actorZ + VISIBILITY_AMPLIFICATION * range) {
        if (hasNoVisibleSkills) {
          visible = false;
        } else {
          return;
        }
      }
    }
  }

  private fillRangeMapAndPathes(
    actorZ: number,
    actorX: number,
    actorY: number,
    cameraLeft: number,
    cameraTop: number
  ) {
    this.allowedTargetPath = new Path2D();
    for (let x = 0; x < RANGED_RANGE * 2 + 1; x++) {
      for (let y = 0; y < RANGED_RANGE * 2 + 1; y++) {
        this.rangeMap[x][y] = x === RANGED_RANGE && y === RANGED_RANGE ? true : undefined;
      }
    }
    if (this.scene.currentActor.parentActor.isRoot) {
      const hasNoVisibleSkills = this.scene.currentActor.actions
        .some(x => x.native.actionClass === ActionClassEnum.Default && !x.native.untargeted && !x.native.onlyVisible);
      for (let i = -RANGED_RANGE; i <= RANGED_RANGE; i++) {
        let newX = actorX + i;
        let newY = actorY - RANGED_RANGE;
        this.fillRangeMapPart(actorZ, actorX, actorY, newX, newY, hasNoVisibleSkills);
        newX = actorX - RANGED_RANGE;
        newY = actorY + i;
        this.fillRangeMapPart(actorZ, actorX, actorY, newX, newY, hasNoVisibleSkills);
        newX = actorX + i;
        newY = actorY + RANGED_RANGE;
        this.fillRangeMapPart(actorZ, actorX, actorY, newX, newY, hasNoVisibleSkills);
        newX = actorX + RANGED_RANGE;
        newY = actorY + i;
        this.fillRangeMapPart(actorZ, actorX, actorY, newX, newY, hasNoVisibleSkills);
      }
    }
    for (let x = 0; x < RANGED_RANGE * 2 + 1; x++) {
      for (let y = 0; y < RANGED_RANGE * 2 + 1; y++) {
        const value = this.rangeMap[x][y];
        if (value !== undefined) {
          const canvasX = (actorX + x - RANGED_RANGE - cameraLeft) * this.tileWidth;
          const canvasY = (actorY + y - RANGED_RANGE - cameraTop) * this.tileHeight;
          // left
          if (x === 0 || this.rangeMap[x - 1][y] === undefined) {
            this.allowedTargetPath.moveTo(canvasX, canvasY - 1);
            this.allowedTargetPath.lineTo(canvasX, canvasY + this.tileHeight + 1);
          }
          // right
          if (x === RANGED_RANGE * 2 || this.rangeMap[x + 1][y] === undefined) {
            this.allowedTargetPath.moveTo(canvasX + this.tileWidth, canvasY - 1);
            this.allowedTargetPath.lineTo(canvasX + this.tileWidth, canvasY + this.tileHeight + 1);
          }
          // top
          if (y === 0 || this.rangeMap[x][y - 1] === undefined) {
            this.allowedTargetPath.moveTo(canvasX - 1, canvasY);
            this.allowedTargetPath.lineTo(canvasX + this.tileWidth + 1, canvasY);
          }
          // bottom
          if (y === RANGED_RANGE * 2 || this.rangeMap[x][y + 1] === undefined) {
            this.allowedTargetPath.moveTo(canvasX - 1, canvasY + this.tileHeight);
            this.allowedTargetPath.lineTo(canvasX + this.tileWidth + 1, canvasY + this.tileHeight);
          }
        }
      }
    }
  }

  private drawDummyPoint(
    x: number,
    y: number,
    char: string,
    color: Color,
    texturePosition: number,

    colors: Uint8Array,
    activities: Uint8Array,
    textureMapping: Float32Array,
    backgrounds: Uint8Array,
    backgroundTextureMapping: Float32Array) {

    const dim = 0.2;
    fillTileMask(
      this.charsService, backgroundTextureMapping,
      DEFAULT_HEIGHT - this.getTileHeight(x - 1, y) >= 120,
      DEFAULT_HEIGHT - this.getTileHeight(x + 1, y) >= 120,
      DEFAULT_HEIGHT - this.getTileHeight(x, y - 1) >= 120,
      DEFAULT_HEIGHT - this.getTileHeight(x, y + 1) >= 120,
      texturePosition);
    fillBackground(backgrounds, color.r * dim / 5, color.g * dim / 5, color.b * dim / 5, texturePosition);
    fillColor(colors, activities, color.r * dim, color.g * dim, color.b * dim, color.a, false, texturePosition);
    fillChar(this.charsService, textureMapping, char, texturePosition);
  }

  private getTileActorAndVisibleActors(tile: Tile) {
    let backgroundActor: Actor;
    let visibleActor: Actor;
    let multiActor = false;
    for (let i = tile.actors.length - 1; i >= 0; i--) {
      const actor = tile.actors[i];
      if (actor.tags.includes('tile')) {
        if (!visibleActor) {
          visibleActor = actor;
        }
        backgroundActor = actor;
        break;
      }
      if (!visibleActor) {
        visibleActor = actor;
        continue;
      }
      if (visibleActor.volume < LARGE_ACTOR_TRESHOLD_VOLUME) {
        if (actor.volume >= LARGE_ACTOR_TRESHOLD_VOLUME) {
          visibleActor = actor;
        }
        multiActor = true;
      }
    }
    return {
      backgroundActor,
      visibleActor: (visibleActor && visibleActor.volume < LARGE_ACTOR_TRESHOLD_VOLUME && multiActor) ? undefined : visibleActor,
      multiActor
    };
  }

  private getTileHeight(x: number, y: number) {
    if (x < 0 || y < 0 || x >= this.scene.width || y >= this.scene.height) {
      return DEFAULT_HEIGHT;
    }
    const tile = this.scene.tiles[x][y];
    for (let i = tile.actors.length - 1; i >= 0; i--) {
      const actor = tile.actors[i];
      if (actor.tags.includes('tile')) {
        return actor.z + actor.height;
      }
    }
    return 0;
  }

  private drawPoint(
    x: number,
    y: number,
    texturePosition: number,

    cameraLeft: number,
    cameraTop: number,

    greenPath: Path2D,
    redPath: Path2D,
    colors: Uint8Array,
    activities: Uint8Array,
    textureMapping: Float32Array,
    backgrounds: Uint8Array,
    backgroundTextureMapping: Float32Array): Actor {

    const tile = this.scene.tiles[x][y];
    if (tile) {
      const tileStub = this.scene.tileStubs.find(s => s.x === x && s.y === y);
      const info = this.getTileActorAndVisibleActors(tile);
      const currentTileHeight = info.backgroundActor ? info.backgroundActor.z + info.backgroundActor.height : 0;
      fillTileMask(
        this.charsService,
        backgroundTextureMapping,
        currentTileHeight - this.getTileHeight(x - 1, y) >= 120,
        currentTileHeight - this.getTileHeight(x + 1, y) >= 120,
        currentTileHeight - this.getTileHeight(x, y - 1) >= 120,
        currentTileHeight - this.getTileHeight(x, y + 1) >= 120,
        texturePosition);
      if (info.backgroundActor) {
        const background = tileStub?.backgroundColor || backgroundImpact(heightImpact(currentTileHeight, info.backgroundActor.color));
        fillBackground(
          backgrounds,
          background.r,
          background.g,
          background.b,
          texturePosition);
      } else {
        fillBackground(backgrounds, 0, 0, 0, texturePosition);
      }
      let color: Color;
      let char: string;
      let mirrored: boolean;
      if (info.multiActor && (!info.visibleActor || this.tickerState)) {
        color = { r: 255, g: 255, b: 0, a: 1 };
        char = '*';
        mirrored = false;
      } else if (!info.visibleActor) {
        color = { r: 23, g: 23, b: 23, a: 1 };
        char = 'ground';
        mirrored = false;
      } else {
        color = this.scene.currentActor.x === info.visibleActor.x && this.scene.currentActor.y === info.visibleActor.y ?
        { r: 225, g: 225, b: 0, a: info.visibleActor.color.a } :
        info.visibleActor === info.backgroundActor ?
        heightImpact(currentTileHeight, info.visibleActor.color) :
        info.visibleActor.color;
        char = info.visibleActor.char;
        mirrored = info.visibleActor.left;
      }

      if (tileStub?.color) {
        color = tileStub.color;
      }
      if (tileStub?.char) {
        char = tileStub.char;
      }

      fillColor(colors, activities, color.r, color.g, color.b, color.a, tileStub?.active, texturePosition);
      fillChar(
        this.charsService, textureMapping, char, texturePosition, mirrored);

      return info.visibleActor;
    }
  }

  drawHealth(healthActor: Actor, canvasX: number, canvasY: number) {
    const percentOfHealth = Math.max(0, Math.min(healthActor.durability / healthActor.maxDurability, 1));
    let path: Path2D;
    if (healthActor.owner && healthActor.owner.id === this.scene.currentPlayer.id) {
      path = this.greenPath;
    } else {
      path = this.redPath;
    }
    const zoomMultiplier = Math.floor(this.battleZoom);
    path.moveTo(canvasX + 2 + zoomMultiplier, canvasY + 2 + zoomMultiplier);
    path.lineTo(
      canvasX + percentOfHealth * (this.tileWidth - 4 * 1 - zoomMultiplier) + 2 + zoomMultiplier,
      canvasY + 2 + zoomMultiplier);
  }

  redraw(shift?: number) {
    if (this.openedModal && this.actionModal && !this.canActNoBlocked) {
      this.openedModal.close();
    }
    if (this.scene && this.shadersProgram) {

      if (shift) {
        if (this.directionTimer > 0) {
          this.directionTimer -= shift;
        } else {
          for (const action of this.smartDirections) {
            if (action.pressed) {
              action.action();
              break;
            }
          }
        }
        this.tickerTime -= shift;
        if (this.tickerTime <= 0) {
          this.tickerState = !this.tickerState;
          this.tickerTime += this.tickerPeriod;
        }
      }

      const time = performance.now();
      const sceneRandom = new Random(this.scene.hash);
      const cameraLeft = this.cameraX - (this.canvasWidth - this.interfaceShift + this.leftInterfaceShift) / 2 / this.tileWidth;
      const cameraTop = this.cameraY - this.canvasHeight / 2 / this.tileHeight;
      const left = Math.floor(cameraLeft) - 1;
      const right = Math.ceil(cameraLeft + this.canvasWidth / (this.tileWidth)) + 1;
      const top = Math.floor(cameraTop) - 1;
      const bottom = Math.ceil(cameraTop + this.canvasHeight / (this.tileHeight)) + 1;
      const width = right - left + 1;
      const height = bottom - top + 1;
      if (this.scene.visualizationChanged) {

        if (this.rangeMapIsActive) {
          this.fillRangeMapAndPathes(
            this.scene.currentActor.z + this.scene.currentActor.height,
            this.scene.currentActor.x,
            this.scene.currentActor.y,
            cameraLeft,
            cameraTop);
        }

        this.textureMapping = new Float32Array(width * height * 12);
        this.colors = new Uint8Array(width * height * 4);
        this.activities = new Uint8Array(width * height);
        this.backgroundTextureMapping = new Float32Array(width * height * 12);
        this.backgrounds = new Uint8Array(width * height * 4);
        this.mainTextureVertexes = new Float32Array(width * height * 12);
        let texturePosition = 0;
        this.healthActors = [];
        for (let y = -20; y <= 60; y++) {
          for (let x = -40; x <= 80; x++) {
            if (x >= left && y >= top && x <= right && y <= bottom) {
              fillVertexPosition(this.mainTextureVertexes, x, y, left, top, this.tileWidth, this.tileHeight, texturePosition);
              if (x >= 0 && y >= 0 && x < this.scene.width && y < this.scene.height) {
                sceneRandom.next();
                const visibleActor = this.drawPoint(x, y, texturePosition, cameraLeft, cameraTop,
                  this.greenPath, this.redPath, this.colors, this.activities, this.textureMapping,
                  this.backgrounds, this.backgroundTextureMapping);
                if (visibleActor && visibleActor.tags.includes('active') && visibleActor.maxDurability) {
                  this.healthActors.push(visibleActor);
                }
              } else {
                const biom = getRandomBiom(sceneRandom, this.scene.biom);
                this.drawDummyPoint(x, y, biom.char, biom.color, texturePosition,
                  this.colors, this.activities, this.textureMapping, this.backgrounds, this.backgroundTextureMapping);
              }
              texturePosition++;
            } else {
              sceneRandom.next();
            }
          }
        }
        this.cursorVertexes = undefined;
      }

      this.redPath = new Path2D();
      this.greenPath = new Path2D();
      for (const actor of this.healthActors) {
        if (!this.selectedActor || this.selectedActor.x !== actor.x || this.selectedActor.y !== actor.y) {
          this.drawHealth(actor, Math.round((actor.x - cameraLeft) * this.tileWidth), Math.round((actor.y - cameraTop) * this.tileHeight));
        }
      }

      if (this.cursorVertexes) {
        for (let i = 0; i < 12; i++) {
          this.textureMapping[this.cursorVertexes.position * 12 + i] = this.cursorVertexes.textureMapping[i];
        }
        for (let i = 0; i < 4; i++) {
          this.colors[this.cursorVertexes.position * 4 + i] = this.cursorVertexes.colors[i];
        }
        this.activities[this.cursorVertexes.position] = this.cursorVertexes.activity;
        this.cursorVertexes = undefined;
      }

      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);

      const rangeMapPositionX = mouseX - this.scene.currentActor.x + RANGED_RANGE;
      const rangeMapPositionY = mouseY - this.scene.currentActor.y + RANGED_RANGE;
      if (this.selectedActor) {
        const position = (this.selectedActor.y - top) * width + (this.selectedActor.x - left);
        this.cursorVertexes = {
          position,
          textureMapping: this.textureMapping.slice(position * 12, position * 12 + 12),
          colors: this.colors.slice(position * 4, position * 4 + 4),
          activity: this.activities[position]
        };
        fillColor(this.colors, this.activities, 255, 255, 0, 1, true, position);
        fillChar(this.charsService, this.textureMapping, this.selectedActor.char, position, this.selectedActor.left);
        this.drawHealth(
          this.selectedActor,
          Math.round((this.selectedActor.x - cameraLeft) * this.tileWidth),
          Math.round((this.selectedActor.y - cameraTop) * this.tileHeight));
      } else if (this.canAct &&
        this.rangeMapIsActive &&
        rangeMapPositionX >= 0 &&
        rangeMapPositionY >= 0 &&
        rangeMapPositionX < RANGED_RANGE * 2 + 1 &&
        rangeMapPositionY < RANGED_RANGE * 2 + 1 &&
        this.rangeMap[rangeMapPositionX][rangeMapPositionY] !== undefined) {

        const value = this.rangeMap[rangeMapPositionX][rangeMapPositionY];
        if (value !== undefined) {
          const position = (mouseY - top) * width + (mouseX - left);
          this.cursorVertexes = {
            position,
            textureMapping: this.textureMapping.slice(position * 12, position * 12 + 12),
            colors: this.colors.slice(position * 4, position * 4 + 4),
            activity: this.activities[position]
          };
          fillColor(this.colors, this.activities, 255, 255, 0, 1, true, position);
          const actors = this.scene.tiles[mouseX][mouseY].actors;
          if (actors.length === 0 || actors[actors.length - 1].tags.includes('tile')) {
            fillChar(this.charsService, this.textureMapping, 'x', position, false);
          }
        }
      }

      drawArrays(
        this.canvasWebGLContext,
        this.shadersProgram,
        this.mainTextureVertexes,
        this.colors,
        this.backgrounds,
        this.textureMapping,
        this.backgroundTextureMapping,
        this.activities,
        this.charsTexture,
        Math.round((left - cameraLeft) * this.tileWidth),
        Math.round((top - cameraTop - 1) * this.tileHeight),
        (right - left + 1) * this.tileWidth,
        (bottom - top + 1) * this.tileHeight,
        (right - left + 1),
        (bottom - top + 1),
        this.charsService.width,
        this.charsService.spriteHeight);
      this.canvas2DContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
      this.canvas2DContext.lineWidth = 4;
      this.canvas2DContext.strokeStyle = 'rgba(255, 0, 0, 1.0)';
      this.canvas2DContext.stroke(this.redPath);
      this.canvas2DContext.strokeStyle = 'rgba(0, 255, 0, 1.0)';
      this.canvas2DContext.stroke(this.greenPath);

      if (this.rangeMapIsActive && (this.canActNoBlocked)) {
        this.canvas2DContext.lineWidth = 2;
        this.canvas2DContext.strokeStyle = 'rgba(255, 255, 0, 1.0)';
        this.canvas2DContext.stroke(this.allowedTargetPath);
      }

      this.scene.visualizationChanged = false;

      // console.log(performance.now() - time);

      /* if (this.battleStorageService.availableActionSquares?.length > 0) {
        this.generateActionSquareGrid(this.battleStorageService.currentActionId ? redPath : yellowPath, cameraLeft, cameraTop);
      }

      this.canvas2DContext.lineWidth = 1;
      for (const text of this.battleStorageService.floatingTexts) {
        if (text.time >= 0) {
          const x = (text.x + 0.5 - cameraLeft) * this.tileWidth;
          const y = (text.y - cameraTop) * this.tileHeight - text.height;
          this.canvas2DContext.fillStyle = `rgba(${text.color.r}, ${text.color.g},
            ${text.color.b}, ${text.color.a})`;
          this.canvas2DContext.strokeStyle = `rgba(0, 8, 24, ${text.color.a})`;
          this.canvas2DContext.fillText(text.text, x, y);
          this.canvas2DContext.strokeText(text.text, x, y);
        }
      }

      if (!this.loaded) {
        this.loaded = true;
        if (this.battleStorageService.version > 1) {
          this.finishLoadingSubscription = this.loadingService.finishLoading()
            .subscribe(() => {
              this.loadingFinished = true;
              this.processNextActionFromQueue();
            });
         }
      }*/
    }
  }

}
