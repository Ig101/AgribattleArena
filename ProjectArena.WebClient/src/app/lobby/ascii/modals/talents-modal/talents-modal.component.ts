import { Component, OnInit, OnDestroy, Inject, ViewChild, ElementRef, HostListener, Input, Output, EventEmitter } from '@angular/core';
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
import { HintDeclaration } from '../../model/hint-declaration.model';
import { CharsService } from 'src/app/shared/services/chars.service';
import { AssetsLoadingService } from 'src/app/shared/services/assets-loading.service';
import { drawArrays, fillTileMask, fillBackground, fillColor, fillChar, fillVertexPosition } from 'src/app/helpers/webgl.helper';
import { Color } from 'src/app/shared/models/color.model';

@Component({
  selector: 'app-talents-modal',
  templateUrl: './talents-modal.component.html',
  styleUrls: ['./talents-modal.component.scss']
})
export class TalentsModalComponent implements OnInit, OnDestroy {

  @Input() data: Character;

  @Output() closeEvent = new EventEmitter<boolean>();
  @Output() hintEvent = new EventEmitter<HintDeclaration>();

  @ViewChild('talentsCanvas', { static: true }) talentsCanvas: ElementRef<HTMLCanvasElement>;
  private canvasContext: WebGLRenderingContext;
  private shadersProgram: WebGLProgram;
  private charsTexture: WebGLTexture;

  private loaded: boolean;

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

  hoveredTalent: TalentNodeWithStatus;

  mouseState: MouseState = {
    buttonsInfo: {},
    x: -1,
    y: -1,
    realX: -1,
    realY: -1
  };

  loading: boolean;
  errors: string[];

  talentsMaxCount = 30;

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
    private userService: UserService,
    private talentsService: TalentsService,
    private webCommunicationService: WebCommunicationService,
    private charsService: CharsService,
    private assetsLoadingService: AssetsLoadingService
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
        this.hintEvent.emit(undefined);
        this.closeEvent.emit();
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
            this.hintEvent.emit(undefined);
            this.closeEvent.emit();
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
    clearInterval(this.drawingTimer);
    this.hintEvent.unsubscribe();
    this.closeEvent.unsubscribe();
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
      if (talent.prerequisites.some(k => !this.selectedTalents.some(t => t.id === k))) {
        talent.accessible = false;
        talent.inaccessibilityReason = InaccessibilityReasonEnum.Prerequisite;
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
    this.loading = true;
    this.tileWidth = this.tileHeight * 0.6;
    this.canvasContext = this.talentsCanvas.nativeElement.getContext('webgl');
    this.charsTexture = this.charsService.getTexture(this.canvasContext);
    this.setupAspectRatio(this.talentsCanvas.nativeElement.offsetWidth, this.talentsCanvas.nativeElement.offsetHeight);
    this.drawingTimer = setInterval(() => {
      this.redrawScene();
    }, 1000 / this.updateFrequency);
    this.assetsLoadingService.loadShadersAndCreateProgram(
      this.canvasContext,
      'vertex-shader-2d.fx',
      'fragment-shader-2d.fx'
    )
      .subscribe((result) => {
        this.shadersProgram = result;
        this.changed = true;
      });
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
      if (event.target !== this.talentsCanvas.nativeElement) {
        this.recalculateMouseMove(-1, -1, event.timeStamp);
      } else {
        this.recalculateMouseMove(event.offsetX, event.offsetY, event.timeStamp);
      }
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
      let newHoveredTalent: TalentNodeWithStatus;
      if (mouseY >= 0 && mouseY < this.mapHeight && mouseX >= 0 && mouseX < this.mapWidth) {
        newHoveredTalent = this.talents[mouseX][mouseY];
      }
      if (newHoveredTalent !== this.hoveredTalent) {
        this.hoveredTalent = newHoveredTalent;
        if (this.hoveredTalent) {
          let error: string;
          if (!this.hoveredTalent.accessible) {
            switch (this.hoveredTalent.inaccessibilityReason) {
              case InaccessibilityReasonEnum.Unreachable:
                error = 'New talent should be adjacent to existing ones.';
                break;
              case InaccessibilityReasonEnum.Cost:
                error = 'Not enough experience.';
                break;
              case InaccessibilityReasonEnum.Exception:
                const exceptions = this.selectedTalents.filter(k => this.hoveredTalent.exceptions.includes(k.id));
                error = `Excluding talent was found: \"${exceptions.map(k => k.name).join('\", \"')}\".`;
                break;
              case InaccessibilityReasonEnum.Prerequisite:
                const prerequisites = this.userService.user.talentsMap
                  .filter(k => this.hoveredTalent.prerequisites.includes(k.id) && !this.selectedTalents.some(talent => talent.id === k.id));
                error = `Required talent wasn't found: \"${prerequisites.map(k => k.name).join('\", \"')}\".`;
                break;
              case InaccessibilityReasonEnum.Key:
                error = 'All talents should be connected to the root node.';
                break;
              case InaccessibilityReasonEnum.Limit:
                error = `Soldier cannot have more than ${this.talentsMaxCount} talents.`;
                break;
              case InaccessibilityReasonEnum.Skills:
                error = 'Soldier cannot know more than 5 skills.';
                break;
              case InaccessibilityReasonEnum.Stats:
                error = 'Soldier\'s stats cannot be less than 5.';
                break;
            }
          }
          const paragraphs: string[] = [];
          if (this.hoveredTalent.strength) {
            paragraphs.push(`Strength ${this.hoveredTalent.strength > 0 ? '+' : ''}${this.hoveredTalent.strength}`);
          }
          if (this.hoveredTalent.willpower) {
            paragraphs.push(`Willpower ${this.hoveredTalent.willpower > 0 ? '+' : ''}${this.hoveredTalent.willpower}`);
          }
          if (this.hoveredTalent.constitution) {
            paragraphs.push(`Constitution ${this.hoveredTalent.constitution > 0 ? '+' : ''}${this.hoveredTalent.constitution}`);
          }
          if (this.hoveredTalent.speed) {
            paragraphs.push(`Speed ${this.hoveredTalent.speed > 0 ? '+' : ''}${this.hoveredTalent.speed}`);
          }
          const hint = {
            title: this.hoveredTalent.name,
            description: this.hoveredTalent.description,
            error,
            paragraphs
          };
          this.hintEvent.emit(hint);
        } else {
          this.hintEvent.emit(undefined);
        }
      }
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
    x: number,
    y: number,
    cameraLeft: number,
    cameraTop: number,
    active: boolean,
    clicked: boolean,
    center: boolean,

    texturePosition: number,
    mainTextureVertexes: Float32Array,
    colors: Uint8Array,
    textureMapping: Float32Array,
    backgrounds: Uint8Array,
    backgroundTextureMapping: Float32Array) {

    fillBackground(backgrounds, 0, 14, 42, texturePosition);
    if (tile) {
      let color: Color;
      fillTileMask(this.charsService, backgroundTextureMapping, false, false, false, false, texturePosition);
      fillVertexPosition(mainTextureVertexes, x, y, 0, 0, this.tileWidth, this.tileHeight, texturePosition);
      if (tile.accessible && active) {
        color = clicked ? { r: 170, g: 170, b: 0, a: 1 } : { r: 255, g: 255, b: 68, a: 1 };
      } else if (tile.selected) {
        color = tile.initiallySelected ? { r: 136, g: 68, b: 255, a: 1} : { r: 68, g: 68, b: 255, a: 1};
      } else {
        if (tile.accessible) {
          color = tile.initiallySelected ? { r: 255, g: 34, b: 34, a: 1} : { r: 255, g: 255, b: 255, a: 1};
        } else {
          color = tile.initiallySelected ? { r: 153, g: 0, b: 0, a: 1} : { r: 136, g: 136, b: 136, a: 1};
        }
      }
      fillColor(colors, color.r, color.g, color.b, color.a, texturePosition);
      fillChar(this.charsService, textureMapping, tile.char, texturePosition);
    } else {
      fillColor(colors, 0, 0, 0, 0, texturePosition);
    }
  }

  private redrawScene() {
    if (!this.changed) {
      return;
    }
    this.changed = false;
    if (this.shadersProgram) {
      this.canvasContext.clearColor(0, 8, 24, 1);
      const cameraLeft = this.mapWidth / 2 - (this.canvasWidth) / 2 / this.tileWidth;
      const cameraTop = this.mapHeight / 2 - this.canvasHeight / 2 / this.tileHeight;
      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);
      const clicked = this.mouseState.buttonsInfo[0] && this.mouseState.buttonsInfo[0].pressed;
      const textureMapping: Float32Array = new Float32Array(this.mapWidth * this.mapHeight * 12);
      const colors: Uint8Array = new Uint8Array(this.mapWidth * this.mapHeight * 4);
      const backgroundTextureMapping: Float32Array = new Float32Array(this.mapWidth * this.mapHeight * 12);
      const backgrounds: Uint8Array = new Uint8Array(this.mapWidth * this.mapHeight * 4);
      const mainTextureVertexes: Float32Array = new Float32Array(this.mapWidth * this.mapHeight * 12);
      let texturePosition = 0;
      for (let y = 0; y < this.mapHeight; y++) {
        for (let x = 0; x < this.mapWidth; x++) {
          this.drawPoint(this.talents[x][y], x, y, cameraLeft, cameraTop,
            mouseX >= x &&
            mouseX <= x &&
            mouseY >= y &&
            mouseY <= y,
            clicked,
            x === (this.mapWidth - 1) / 2 && y === (this.mapHeight - 1) / 2,
            texturePosition,
            mainTextureVertexes,
            colors,
            textureMapping,
            backgrounds,
            backgroundTextureMapping);
          texturePosition++;
        }
      }
      drawArrays(
        this.canvasContext,
        this.shadersProgram,
        mainTextureVertexes,
        colors,
        backgrounds,
        textureMapping,
        backgroundTextureMapping,
        this.charsTexture,
        Math.round((0 - cameraLeft) * this.tileWidth),
        Math.round((0 - cameraTop) * this.tileHeight),
        this.mapWidth * this.tileWidth,
        this.mapHeight * this.tileHeight,
        this.mapWidth,
        this.mapHeight,
        this.charsService.width,
        this.charsService.spriteHeight);

      if (!this.loaded) {
        this.loaded = true;
        this.loading = false;
      }
    }
  }

  showStrengthHint() {
    this.hintEvent.emit({
      title: 'Strength',
      description: 'Each point of strength increases damage soldier\'s physical abilities do.',
      error: undefined,
      paragraphs: []
    } as HintDeclaration);
  }

  showWillpowerHint() {
    this.hintEvent.emit({
      title: 'Willpower',
      description: 'Each point of willpower increases damage soldier\'s magic abilities do.',
      error: undefined,
      paragraphs: []
    } as HintDeclaration);
  }

  showConstitutionHint() {
    this.hintEvent.emit({
      title: 'Constitution',
      description: 'Each point of constitution increases soldier\'s amount of health.',
      error: undefined,
      paragraphs: []
    } as HintDeclaration);
  }

  showSpeedHint() {
    this.hintEvent.emit({
      title: 'Speed',
      description: 'Each point of speed decreases the time between soldier\'s turns.',
      error: undefined,
      paragraphs: []
    } as HintDeclaration);
  }

  hideHint() {
    this.hintEvent.emit(undefined);
  }
}
