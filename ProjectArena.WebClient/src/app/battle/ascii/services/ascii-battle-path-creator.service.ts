import { Injectable } from '@angular/core';
import { AsciiBattleStorageService } from './ascii-battle-storage.service';
import { Actor } from '../models/scene/actor.model';
import { ActionSquare } from '../models/actions/action-square.model';
import { Tile } from '../models/scene/tile.model';
import { rangeBetween, rangeBetweenShift } from 'src/app/helpers/math.helper';
import { Skill } from '../models/scene/skill.model';
import { removeFromArray } from 'src/app/helpers/extensions/array.extension';
import { ActionSquareTypeEnum } from '../models/enum/action-square-type.enum';
import { checkSkillTargets, checkMilliness } from '../helpers/scene-actions.helper';

@Injectable()
export class AsciiBattlePathCreatorService {

  constructor(
    private battleStorageService: AsciiBattleStorageService
  ) { }

  calculateNeighbourhood(square: ActionSquare, neighbourSquare: ActionSquare) {
    const sX = neighbourSquare.x - square.x;
    const sY = neighbourSquare.y - square.y;
    if (sX === -1) {
      square.leftSquare = true;
      neighbourSquare.rightSquare = true;
    }
    if (sX === 1) {
      square.rightSquare = true;
      neighbourSquare.leftSquare = true;
    }
    if (sY === -1) {
      square.topSquare = true;
      neighbourSquare.bottomSquare = true;
    }
    if (sY === 1) {
      square.bottomSquare = true;
      neighbourSquare.topSquare = true;
    }
  }

  private calculateRangeSquares(
    actor: Actor,
    initialTile: Tile,
    x: number,
    y: number,
    previousSquare: ActionSquare,
    remainedActionPoints: number,
    remainedSteps: number,
    allSquares: ActionSquare[],
    onlyTargets: boolean,
    skill: Skill
    ) {
    if (!actor.canAct || remainedActionPoints < 0 || skill.preparationTime > 0) {
      return;
    }
    for (let sX = -skill.range; sX <= skill.range; sX++) {
      for (let sY = -skill.range; sY <= skill.range; sY++) {
        const newX = x + sX;
        const newY = y + sY;
        if (
          newX < this.battleStorageService.scene.width && newX >= 0 &&
          newY < this.battleStorageService.scene.height && newY >= 0 &&
          rangeBetweenShift(sX, sY) <= skill.range) {
            const tile = this.battleStorageService.scene.tiles[newX][newY];
            if (checkSkillTargets(tile, actor, skill.availableTargets) &&
              (!skill.onlyVisibleTargets || checkMilliness(initialTile, tile, skill.range <= 1, this.battleStorageService.scene.tiles)) &&
              (!onlyTargets || tile.actor || tile.decoration) && tile.actor !== actor) {
              const existingSquare = allSquares.find(s => s.x === newX && s.y === newY);
              if (existingSquare && remainedSteps <= existingSquare.remainedSteps) {
                continue;
              } else {
                removeFromArray(allSquares, existingSquare);
              }
              const tempSquare = {
                x: newX,
                y: newY,
                remainedPoints: remainedActionPoints,
                remainedSteps: 0,
                parentSquares: !previousSquare ? [] : [previousSquare, ...previousSquare.parentSquares],
                type: ActionSquareTypeEnum.Act,
                isActor: false,
                topSquare: existingSquare?.topSquare,
                bottomSquare: existingSquare?.bottomSquare,
                leftSquare: existingSquare?.leftSquare,
                rightSquare: existingSquare?.rightSquare
              };
              const leftSquare = allSquares.find(s => s.x === newX - 1 && s.y === newY && (s.type !== undefined || onlyTargets));
              if (leftSquare) {
                this.calculateNeighbourhood(tempSquare, leftSquare);
              }
              const rightSquare = allSquares.find(s => s.x === newX + 1 && s.y === newY && (s.type !== undefined || onlyTargets));
              if (rightSquare) {
                this.calculateNeighbourhood(tempSquare, rightSquare);
              }
              const topSquare = allSquares.find(s => s.x === newX && s.y === newY - 1 && (s.type !== undefined || onlyTargets));
              if (topSquare) {
                this.calculateNeighbourhood(tempSquare, topSquare);
              }
              const bottomSquare = allSquares.find(s => s.x === newX && s.y === newY + 1 && (s.type !== undefined || onlyTargets));
              if (bottomSquare) {
                this.calculateNeighbourhood(tempSquare, bottomSquare);
              }
              allSquares.push(tempSquare);
            }
        }
      }
    }
  }

  calculateActiveSquares(actor: Actor, actionId?: number): ActionSquare[] {
    const skill = actionId && actionId > 0 ? actor.skills.find(x => x.id === actionId) : actor.attackingSkill;
    if (!skill) {
      return [];
    }
    const actorSquare = {
      x: actor.x,
      y: actor.y,
      remainedPoints: actor.actionPoints,
      remainedSteps: 0,
      parentSquares: [],
      type: skill.availableTargets.self ? ActionSquareTypeEnum.Act : undefined,
      isActor: true
    };
    const allSquares: ActionSquare[] = [actorSquare];
    const actorTile = this.battleStorageService.scene.tiles[actor.x][actor.y];
    if (!skill) {
      return [];
    }
    this.calculateRangeSquares(actor, actorTile, actor.x, actor.y, actorSquare,
      actor.actionPoints - skill.cost, 0, allSquares, false, skill);
    return allSquares.filter(x => x.type !== undefined);
  }
}
