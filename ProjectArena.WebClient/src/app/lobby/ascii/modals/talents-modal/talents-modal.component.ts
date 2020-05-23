import { Component, OnInit, OnDestroy, Inject, ViewChild, ElementRef, HostListener } from '@angular/core';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { Subject } from 'rxjs';
import { ComponentSizeEnum } from 'src/app/shared/models/enum/component-size.enum';
import { OverlayRef } from '@angular/cdk/overlay';
import { AppFormGroup } from 'src/app/shared/components/form-group/app-form-group';
import { controlRequiredValidator } from 'src/app/shared/validators/control-required.validator';
import { passwordDigitsValidator } from 'src/app/shared/validators/password-digits.validator';
import { passwordLowercaseValidator } from 'src/app/shared/validators/password-lowercase.validator';
import { passwordUppercaseValidator } from 'src/app/shared/validators/password-uppercase.validator';
import { controlMinLengthValidator } from 'src/app/shared/validators/control-min-length.validator';
import { confirmPasswordValidator } from 'src/app/shared/validators/confirm-password.validator';
import { Character } from '../../model/character.model';
import { MODAL_DATA } from 'src/app/shared/services/modal.service';
import { ActorNative } from 'src/app/battle/ascii/models/natives/actor-native.model';
import { actorNatives } from 'src/app/battle/ascii/natives';
import { DEFAULT_STRENGTH, DEFAULT_WILLPOWER, DEFAULT_CONSTITUTION, DEFAULT_SPEED } from 'src/app/helpers/consts.helper';
import { UserService } from 'src/app/shared/services/user.service';
import { TalentsService } from 'src/app/lobby/services/talents.service';
import { TalentNode } from '../../model/talent-node.model';
import { TalentNodeWithStatus } from '../../model/talent-node-with-status.model';
import { TalentNodeChangeDeclaration } from '../../model/talent-node-change-declaration';
import { WebCommunicationService } from 'src/app/shared/services/web-communication.service';
import { ChangeCharacterTalentsRequest } from '../../model/requests/change-character-talents-request.model';
import { LoadingService } from 'src/app/shared/services/loading.service';
import { Point } from 'src/app/shared/models/point.model';
import { MouseState } from 'src/app/shared/models/mouse-state.model';
import { Random } from 'src/app/shared/random/random';
import { InaccessibilityReasonEnum } from '../../model/enums/inaccessibility-reason.enum';
import { removeFromArray } from 'src/app/helpers/extensions/array.extension';
import { getNativeIdByTalents } from 'src/app/helpers/talents.helper';

@Component({
  selector: 'app-talents-modal',
  templateUrl: './talents-modal.component.html',
  styleUrls: ['./talents-modal.component.scss']
})
export class TalentsModalComponent implements OnInit, OnDestroy, IModal<any> {

  @ViewChild('talentsCanvas', { static: true }) talentsCanvas: ElementRef<HTMLCanvasElement>;
  private canvasContext: CanvasRenderingContext2D;

  cursor: string;

  readonly defaultWidth = 902;
  readonly defaultHeight = 770;
  readonly defaultAspectRatio = this.defaultWidth / this.defaultHeight;

  tileHeight = 30;
  tileWidth = 0;
  mapWidth = 49;
  mapHeight = 25;

  zoom = 0;

  private updateFrequency = 30;
  private drawingTimer;
  changed = false;

  mouseState: MouseState = {
    buttonsInfo: {},
    x: -1,
    y: -1,
    realX: -1,
    realY: -1
  };

  onClose = new Subject<any>();
  onCancel = new Subject<any>();

  loading: boolean;
  errors: string[];

  talentsMaxCount = 15;

  nativeId: string;
  name: string;
  description: string;
  color: string;
  char: string;

  strength: number;
  willpower: number;
  constitution: number;
  speed: number;
  skillsCount: number;
  talentsCount: number;

  initialStrength: number;
  initialWillpower: number;
  initialConstitution: number;
  initialSpeed: number;
  initialSkillsCount: number;
  initialExperience: number;

  talents: TalentNodeWithStatus[][];
  changes: TalentNodeChangeDeclaration[] = [];
  selectedTalents: TalentNodeWithStatus[] = [];

  componentSizeEnum = ComponentSizeEnum;

  get canvasWidth() {
    return this.talentsCanvas.nativeElement.width;
  }

  get canvasHeight() {
    return this.talentsCanvas.nativeElement.height;
  }

  constructor(
    @Inject(MODAL_DATA) private data: Character,
    private overlay: OverlayRef,
    private userService: UserService,
    private talentsService: TalentsService,
    private webCommunicationService: WebCommunicationService,
    private loadingService: LoadingService
  ) { }

  close() {
    if (!this.loading) {
      let changed = false;
      if (this.changes.length > 0) {
        for (const talentsRow of this.talents) {
          for (const talent of talentsRow) {
            if (talent) {
              changed = talent.selected !== talent.initiallySelected;
              if (changed) {
                break;
              }
            }
          }
          if (changed) {
            break;
          }
        }
      }
      if (!changed) {
        this.onClose.next();
        this.overlay.detach();
        this.overlay.dispose();
        return;
      }
      this.loading = true;
      this.webCommunicationService.post<ChangeCharacterTalentsRequest, void>(`api/user/character/${this.data.id}/talents`, {
        changes: this.changes
      })
        .subscribe(result => {
          if (result.success) {
            this.data.talents = this.selectedTalents;
            this.data.nativeId = this.nativeId;
            this.onClose.next();
            this.overlay.detach();
            this.overlay.dispose();
          } else {
            if (!this.webCommunicationService.desync(result)) {
              this.undo();
              this.errors = result.errors;
            }
          }
        });
    }
  }

  ngOnDestroy() {
    this.onClose.unsubscribe();
    this.onCancel.unsubscribe();
  }

  closeOnClick(event) {
    if (event.target !== event.currentTarget) {
      return;
    }
    this.close();
  }

  findPath(talent: TalentNodeWithStatus, checked: TalentNodeWithStatus[]): boolean {
    const centerX = (this.mapWidth - 1) / 2;
    const centerY = (this.mapHeight - 1) / 2;
    checked.push(talent);
    if (Math.abs(talent.x - centerX) === 1 && talent.y === centerY ||
      Math.abs(talent.y - centerY) === 1 && talent.x === centerX) {
      return true;
    }
    const leftTalent = this.talents[talent.x - 1][talent.y];
    if (leftTalent && leftTalent.selected && !checked.includes(leftTalent) && this.findPath(leftTalent, checked)) {
      return true;
    }
    const rightTalent = this.talents[talent.x + 1][talent.y];
    if (rightTalent && rightTalent.selected && !checked.includes(rightTalent) && this.findPath(rightTalent, checked)) {
      return true;
    }
    const topTalent = this.talents[talent.x][talent.y - 1];
    if (topTalent && topTalent.selected && !checked.includes(topTalent) && this.findPath(topTalent, checked)) {
      return true;
    }
    const bottomTalent = this.talents[talent.x][talent.y + 1];
    if (bottomTalent && bottomTalent.selected && !checked.includes(bottomTalent) && this.findPath(bottomTalent, checked)) {
      return true;
    }
    return false;
  }

  calculateAccessibility(x: number, y: number) {
    const talent = this.talents[x][y];
    if (!talent) {
      return;
    }
    const centerX = (this.mapWidth - 1) / 2;
    const centerY = (this.mapHeight - 1) / 2;
    if (this.userService.user.experience === 0 && talent.initiallySelected === talent.selected) {
      talent.accessible = false;
      talent.inaccessibilityReason = InaccessibilityReasonEnum.Cost;
      return;
    }
    if (talent.selected) {
      if (this.strength - talent.strength < 5 ||
        this.willpower - talent.willpower < 5 ||
        this.constitution - talent.constitution < 5 ||
        this.speed - talent.speed < 5) {
        talent.accessible = false;
        talent.inaccessibilityReason = InaccessibilityReasonEnum.Stats;
        return;
      }
      talent.selected = false;
      const leftTalent = this.talents[x - 1][y];
      const rightTalent = this.talents[x + 1][y];
      const topTalent = this.talents[x][y - 1];
      const bottomTalent = this.talents[x][y + 1];
      if (((leftTalent && leftTalent.selected && !this.findPath(leftTalent, [])) ||
        (rightTalent && rightTalent.selected && !this.findPath(rightTalent, [])) ||
        (topTalent && topTalent.selected && !this.findPath(topTalent, [])) ||
        (bottomTalent && bottomTalent.selected && !this.findPath(bottomTalent, [])))) {
          talent.accessible = false;
          talent.inaccessibilityReason = InaccessibilityReasonEnum.Key;
          talent.selected = true;
          return;
      }
      talent.selected = true;
    } else {
      if (this.talentsCount >= this.talentsMaxCount) {
        talent.accessible = false;
        talent.inaccessibilityReason = InaccessibilityReasonEnum.Limit;
        return;
      }
      if (!(Math.abs(talent.x - centerX) === 1 && talent.y === centerY ||
        Math.abs(talent.y - centerY) === 1 && talent.x === centerX) &&
        !this.talents[x - 1][y]?.selected &&
        !this.talents[x + 1][y]?.selected &&
        !this.talents[x][y - 1]?.selected &&
        !this.talents[x][y + 1]?.selected) {
        talent.accessible = false;
        talent.inaccessibilityReason = InaccessibilityReasonEnum.Unreachable;
        return;
      }
      if (this.strength + talent.strength < 5 ||
        this.willpower + talent.willpower < 5 ||
        this.constitution + talent.constitution < 5 ||
        this.speed + talent.speed < 5) {
        talent.accessible = false;
        talent.inaccessibilityReason = InaccessibilityReasonEnum.Stats;
        return;
      }
      if (this.skillsCount + talent.skillsAmount > 5) {
        talent.accessible = false;
        talent.inaccessibilityReason = InaccessibilityReasonEnum.Skills;
        return;
      }
      if (this.selectedTalents.some(k => talent.exceptions.includes(k.id))) {
        talent.accessible = false;
        talent.inaccessibilityReason = InaccessibilityReasonEnum.Exception;
        return;
      }
    }
    talent.accessible = true;
    talent.inaccessibilityReason = undefined;
  }

  ngOnInit(): void {
    this.nativeId = this.data.nativeId;
    const native = actorNatives[this.nativeId];
    this.description = native.name;
    this.color = `rgba(${native.visualization.color.r}, ${native.visualization.color.g},
      ${native.visualization.color.b}, ${native.visualization.color.a})`;
    this.char = native.visualization.char;
    this.name = this.data.name;
    this.initialStrength = DEFAULT_STRENGTH;
    this.initialWillpower = DEFAULT_WILLPOWER;
    this.initialConstitution = DEFAULT_CONSTITUTION;
    this.initialSpeed = DEFAULT_SPEED;
    this.initialExperience = this.userService.user.experience;
    this.initialSkillsCount = 1;
    this.talents = this.talentsService.getTalentsMap();
    this.talentsCount = this.data.talents.length;
    for (const chosenTalent of this.data.talents) {
      const talent = this.talents[chosenTalent.x][chosenTalent.y];
      talent.initiallySelected = true;
      talent.selected = true;
      this.selectedTalents.push(talent);
      this.initialStrength += talent.strength;
      this.initialWillpower += talent.willpower;
      this.initialSpeed += talent.speed;
      this.initialConstitution += talent.constitution;
      this.initialSkillsCount += talent.skillsAmount;
    }
    this.skillsCount = this.initialSkillsCount;
    this.strength = this.initialStrength;
    this.willpower = this.initialWillpower;
    this.constitution = this.initialConstitution;
    this.speed = this.initialSpeed;

    if (this.userService.user.experience <= 0 || this.talentsCount >= this.talentsMaxCount) {
      for (const talentsRow of this.talents) {
        for (const talent of talentsRow) {
          if (!talent) {
            continue;
          }
          if (this.userService.user.experience <= 0 && talent.initiallySelected === talent.selected) {
            talent.accessible = false;
            talent.inaccessibilityReason = InaccessibilityReasonEnum.Cost;
          } else if (!talent.selected) {
            talent.accessible = false;
            talent.inaccessibilityReason = InaccessibilityReasonEnum.Limit;
          }
        }
      }
    }
    this.calculateAccessibility((this.mapWidth - 1) / 2 - 1, (this.mapHeight - 1) / 2);
    this.calculateAccessibility((this.mapWidth - 1) / 2 + 1, (this.mapHeight - 1) / 2);
    this.calculateAccessibility((this.mapWidth - 1) / 2, (this.mapHeight - 1) / 2 - 1);
    this.calculateAccessibility((this.mapWidth - 1) / 2, (this.mapHeight - 1) / 2 + 1);
    for (const talent of this.selectedTalents) {
      this.calculateAccessibility(talent.x, talent.y);
      this.calculateAccessibility(talent.x - 1, talent.y);
      this.calculateAccessibility(talent.x + 1, talent.y);
      this.calculateAccessibility(talent.x, talent.y - 1);
      this.calculateAccessibility(talent.x, talent.y + 1);
    }

    this.tileWidth = this.tileHeight * 0.6;
    this.canvasContext = this.talentsCanvas.nativeElement.getContext('2d');
    this.setupAspectRatio(this.talentsCanvas.nativeElement.offsetWidth, this.talentsCanvas.nativeElement.offsetHeight);
    this.drawingTimer = setInterval(() => {
      this.redrawScene();
    }, 1000 / this.updateFrequency);
  }

  undo() {
    this.userService.user.experience = this.initialExperience;
    this.strength = this.initialStrength;
    this.willpower = this.initialWillpower;
    this.constitution = this.initialConstitution;
    this.speed = this.initialSpeed;
    this.skillsCount = this.initialSkillsCount;
    this.changes.length = 0;
    this.selectedTalents = this.data.talents.map(x => this.talents[x.x][x.y]);
    this.talentsCount = this.data.talents.length;
    for (const talentsRow of this.talents) {
      for (const talent of talentsRow) {
        if (!talent) {
          continue;
        }
        talent.selected = talent.initiallySelected;
        talent.accessible = false;
        talent.inaccessibilityReason = InaccessibilityReasonEnum.Unreachable;
      }
    }
    if (this.userService.user.experience <= 0 || this.talentsCount >= this.talentsMaxCount) {
      for (const talentsRow of this.talents) {
        for (const talent of talentsRow) {
          if (!talent) {
            continue;
          }
          if (this.userService.user.experience <= 0 && talent.initiallySelected === talent.selected) {
            talent.accessible = false;
            talent.inaccessibilityReason = InaccessibilityReasonEnum.Cost;
          } else if (!talent.selected) {
            talent.accessible = false;
            talent.inaccessibilityReason = InaccessibilityReasonEnum.Limit;
          }
        }
      }
    }
    this.calculateAccessibility((this.mapWidth - 1) / 2 - 1, (this.mapHeight - 1) / 2);
    this.calculateAccessibility((this.mapWidth - 1) / 2 + 1, (this.mapHeight - 1) / 2);
    this.calculateAccessibility((this.mapWidth - 1) / 2, (this.mapHeight - 1) / 2 - 1);
    this.calculateAccessibility((this.mapWidth - 1) / 2, (this.mapHeight - 1) / 2 + 1);
    for (const talent of this.selectedTalents) {
      this.calculateAccessibility(talent.x, talent.y);
      this.calculateAccessibility(talent.x - 1, talent.y);
      this.calculateAccessibility(talent.x + 1, talent.y);
      this.calculateAccessibility(talent.x, talent.y - 1);
      this.calculateAccessibility(talent.x, talent.y + 1);
    }
    const nativeId = getNativeIdByTalents(this.selectedTalents);
    if (nativeId !== this.nativeId) {
      this.nativeId = nativeId;
      const native = actorNatives[this.data.nativeId];
      this.description = native.name;
      this.color = `rgba(${native.visualization.color.r}, ${native.visualization.color.g},
        ${native.visualization.color.b}, ${native.visualization.color.a})`;
      this.char = native.visualization.char;
    }
  }

  changeSelectedState(changedTalent: TalentNodeWithStatus) {
    const oldExperience = this.userService.user.experience;
    const oldTalents = this.talentsCount;
    changedTalent.selected = !changedTalent.selected;
    if (this.changes.length > 0 &&
      this.changes[this.changes.length - 1].x === changedTalent.x &&
      this.changes[this.changes.length - 1].y === changedTalent.y) {
      this.changes.splice(this.changes.length - 1);
    } else {
      this.changes.push({
        x: changedTalent.x,
        y: changedTalent.y,
        state: changedTalent.selected
      });
    }
    if (changedTalent.selected === changedTalent.initiallySelected) {
      this.userService.user.experience++;
    } else {
      this.userService.user.experience--;
    }
    if (changedTalent.selected) {
      this.strength += changedTalent.strength;
      this.willpower += changedTalent.willpower;
      this.constitution += changedTalent.constitution;
      this.speed += changedTalent.speed;
      this.skillsCount += changedTalent.skillsAmount;
      this.talentsCount++;
      this.selectedTalents.push(changedTalent);
    } else {
      this.strength -= changedTalent.strength;
      this.willpower -= changedTalent.willpower;
      this.constitution -= changedTalent.constitution;
      this.speed -= changedTalent.speed;
      this.skillsCount -= changedTalent.skillsAmount;
      this.talentsCount--;
      removeFromArray(this.selectedTalents, changedTalent);
      this.calculateAccessibility(changedTalent.x, changedTalent.y);
      this.calculateAccessibility(changedTalent.x - 1, changedTalent.y);
      this.calculateAccessibility(changedTalent.x + 1, changedTalent.y);
      this.calculateAccessibility(changedTalent.x, changedTalent.y - 1);
      this.calculateAccessibility(changedTalent.x, changedTalent.y + 1);
    }
    if (this.userService.user.experience <= 0 || this.talentsCount >= this.talentsMaxCount) {
      for (const talentsRow of this.talents) {
        for (const talent of talentsRow) {
          if (!talent) {
            continue;
          }
          if (this.userService.user.experience <= 0 && talent.initiallySelected === talent.selected) {
            talent.accessible = false;
            talent.inaccessibilityReason = InaccessibilityReasonEnum.Cost;
          } else if (!talent.selected) {
            talent.accessible = false;
            talent.inaccessibilityReason = InaccessibilityReasonEnum.Limit;
          }
        }
      }
    }
    this.calculateAccessibility((this.mapWidth - 1) / 2 - 1, (this.mapHeight - 1) / 2);
    this.calculateAccessibility((this.mapWidth - 1) / 2 + 1, (this.mapHeight - 1) / 2);
    this.calculateAccessibility((this.mapWidth - 1) / 2, (this.mapHeight - 1) / 2 - 1);
    this.calculateAccessibility((this.mapWidth - 1) / 2, (this.mapHeight - 1) / 2 + 1);
    for (const talent of this.selectedTalents) {
      this.calculateAccessibility(talent.x, talent.y);
      this.calculateAccessibility(talent.x - 1, talent.y);
      this.calculateAccessibility(talent.x + 1, talent.y);
      this.calculateAccessibility(talent.x, talent.y - 1);
      this.calculateAccessibility(talent.x, talent.y + 1);
    }
    const nativeId = getNativeIdByTalents(this.selectedTalents);
    if (nativeId !== this.nativeId) {
      this.nativeId = nativeId;
      const native = actorNatives[this.nativeId];
      this.description = native.name;
      this.color = `rgba(${native.visualization.color.r}, ${native.visualization.color.g},
        ${native.visualization.color.b}, ${native.visualization.color.a})`;
      this.char = native.visualization.char;
      this.changed = true;
    }
  }

  onResize() {
    this.setupAspectRatio(this.talentsCanvas.nativeElement.offsetWidth, this.talentsCanvas.nativeElement.offsetHeight);
  }

  onMouseLeave() {
    for (const state of Object.values(this.mouseState.buttonsInfo)) {
      state.pressed = false;
      state.timeStamp = 0;
    }
    this.cursor = undefined;
    this.mouseState.realX = undefined;
    this.mouseState.realY = undefined;
  }

  onMouseUp(event: MouseEvent) {
    const x = Math.floor(this.mouseState.x);
    const y = Math.floor(this.mouseState.y);
    this.mouseState.buttonsInfo[event.button] = {pressed: false, timeStamp: 0};
    this.recalculateMouseMove(event.offsetX, event.offsetY, event.timeStamp);
    const newX = Math.floor(this.mouseState.x);
    const newY = Math.floor(this.mouseState.y);
    if (event.button === 0 && x >= 0 && x < this.mapWidth && y >= 0 && y < this.mapHeight) {
      const talent = this.talents[x][y];
      const newTalent = this.talents[newX][newY];
      if (talent && talent === newTalent && talent.accessible) {
        this.changeSelectedState(talent);
      }
    }
  }

  onMouseUpWindow(event: MouseEvent) {
    setTimeout(() => {
      this.mouseState.buttonsInfo[event.button] = {pressed: false, timeStamp: 0};
      this.changed = true;
      this.recalculateMouseMove(event.offsetX, event.offsetY, event.timeStamp);
    });
  }

  onMouseDown(event: MouseEvent) {
    this.changed = true;
    this.mouseState.buttonsInfo[event.button] = {pressed: true, timeStamp: 0};
  }

  onMouseMove(event: MouseEvent) {
    this.mouseState.realX = event.x;
    this.mouseState.realY = event.y;
    this.recalculateMouseMove(event.offsetX, event.offsetY, event.timeStamp);
  }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event) {
    event.preventDefault();
  }

  private recalculateMouseMove(x: number, y: number, timeStamp?: number) {
    const leftKey = this.mouseState.buttonsInfo[0];
    const rightKey = this.mouseState.buttonsInfo[2];
    const cameraLeft = this.mapWidth / 2 - this.canvasWidth / 2 / this.tileWidth;
    const cameraTop = this.mapHeight / 2 - this.canvasHeight / 2 / this.tileHeight;
    const newX = x / this.zoom / this.tileWidth + cameraLeft;
    const newY = y / this.zoom / this.tileHeight + cameraTop;
    const mouseX = Math.floor(newX);
    const mouseY = Math.floor(newY);
    this.cursor = undefined;
    if (this.talents && mouseX >= 0 && mouseX < this.mapWidth && mouseY >= 0 && mouseY < this.mapHeight) {
      if (this.talents[mouseX][mouseY] && this.talents[mouseX][mouseY].accessible) {
        this.cursor = 'pointer';
      }
    }
    if (!rightKey?.pressed && !leftKey?.pressed) {
      this.mouseState.x = newX;
      this.mouseState.y = newY;
      this.changed = true;
    }
  }

  private setupAspectRatio(width: number, height: number) {
    const newAspectRatio = width / height;
    if (newAspectRatio < this.defaultAspectRatio) {
      const oldWidth = this.defaultWidth;
      this.talentsCanvas.nativeElement.width = oldWidth;
      this.talentsCanvas.nativeElement.height = oldWidth / newAspectRatio;
    } else {
      const oldHeight = this.defaultHeight;
      this.talentsCanvas.nativeElement.width = oldHeight * newAspectRatio;
      this.talentsCanvas.nativeElement.height = oldHeight;
    }
    this.zoom = this.talentsCanvas.nativeElement.offsetWidth / this.canvasWidth;
    this.changed = true;
    this.redrawScene();
  }

  private drawPoint(
    tile: TalentNodeWithStatus,
    x: number, y: number,
    cameraLeft: number,
    cameraTop: number,
    active: boolean,
    clicked: boolean,
    center: boolean) {
    if (center) {
      const canvasX = (x - cameraLeft) * this.tileWidth;
      const canvasY = (y - cameraTop) * this.tileHeight;
      const symbolY = canvasY + this.tileHeight * 0.75;
      this.canvasContext.font = `${this.tileHeight}px PT Mono`;
      this.canvasContext.fillStyle = this.color;
      this.canvasContext.fillText(this.char, canvasX, symbolY);
    }
    if (tile) {
      const canvasX = (x - cameraLeft) * this.tileWidth;
      const canvasY = (y - cameraTop) * this.tileHeight;
      const symbolY = canvasY + this.tileHeight * 0.75;
      let color: string;
      if (tile.selected) {
        color = tile.initiallySelected ? '#8800FF' : '#4444FF';
      } else {
        if (tile.accessible) {
          color = tile.initiallySelected ? '#FF2222' : '#FFFFFF';
        } else {
          color = tile.initiallySelected ? '#990000' : '#888888';
        }
      }
      this.canvasContext.font = `${this.tileHeight - 6}px PT Mono`;
      this.canvasContext.fillStyle = tile.accessible && active ? (clicked ? `rgb(170, 170, 0)` : `rgb(255, 255, 68)`) :
        color;
      this.canvasContext.fillText(tile.char, canvasX + 2, symbolY + 3);
    }
  }

  private redrawScene() {
    if (!this.changed) {
      return;
    }
    this.changed = false;
    const cameraLeft = this.mapWidth / 2 - (this.canvasWidth) / 2 / this.tileWidth;
    const cameraTop = this.mapHeight / 2 - this.canvasHeight / 2 / this.tileHeight;
    this.canvasContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
    this.canvasContext.textAlign = 'left';
    const mouseX = Math.floor(this.mouseState.x);
    const mouseY = Math.floor(this.mouseState.y);
    const clicked = this.mouseState.buttonsInfo[0] && this.mouseState.buttonsInfo[0].pressed;
    for (let x = 0; x < this.mapWidth; x++) {
      for (let y = 0; y < this.mapHeight; y++) {
        this.drawPoint(this.talents[x][y], x, y, cameraLeft, cameraTop,
          mouseX >= x &&
          mouseX <= x &&
          mouseY >= y &&
          mouseY <= y,
          clicked,
          x === (this.mapWidth - 1) / 2 && y === (this.mapHeight - 1) / 2);
      }
    }
  }
}
