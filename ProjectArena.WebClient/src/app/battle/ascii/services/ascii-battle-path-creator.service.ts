import { Injectable } from '@angular/core';
import { AsciiBattleStorageService } from './ascii-battle-storage.service';
import { Actor } from '../models/scene/actor.model';
import { ActionSquare } from '../models/actions/action-square.model';
import { Tile } from '../models/scene/tile.model';
import { rangeBetween, rangeBetweenShift } from 'src/app/helpers/math.helper';
import { Skill } from '../models/scene/skill.model';
import { removeFromArray } from 'src/app/helpers/extensions/array.extension';
import { ActionSquareTypeEnum } from '../models/enum/action-square-type.enum';

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
            if ((!skill.meleeOnly || Math.abs(tile.height - initialTile.height) < 10) &&
              (!onlyTargets || tile.actor || tile.decoration) && tile.actor !== actor) {
              const existingSquare = allSquares.find(s => s.x === newX && s.y === newY);
              if (existingSquare && remainedActionPoints <= existingSquare.remainedPoints) {
                continue;
              } else {
                removeFromArray(allSquares, existingSquare);
              }
              const tempSquare = {
                x: newX,
                y: newY,
                remainedPoints: remainedActionPoints,
                parentSquares: !previousSquare ? [] : [previousSquare, ...previousSquare.parentSquares],
                type: ActionSquareTypeEnum.Act,
                topSquare: existingSquare?.topSquare,
                bottomSquare: existingSquare?.bottomSquare,
                leftSquare: existingSquare?.leftSquare,
                rightSquare: existingSquare?.rightSquare
              };
              const leftSquare = allSquares.find(s => s.x === newX - 1 && s.y === newY);
              if (leftSquare) {
                this.calculateNeighbourhood(tempSquare, leftSquare);
              }
              const rightSquare = allSquares.find(s => s.x === newX + 1 && s.y === newY);
              if (rightSquare) {
                this.calculateNeighbourhood(tempSquare, rightSquare);
              }
              const topSquare = allSquares.find(s => s.x === newX && s.y === newY - 1);
              if (topSquare) {
                this.calculateNeighbourhood(tempSquare, topSquare);
              }
              const bottomSquare = allSquares.find(s => s.x === newX && s.y === newY + 1);
              if (bottomSquare) {
                this.calculateNeighbourhood(tempSquare, bottomSquare);
              }
              allSquares.push(tempSquare);
            }
        }
      }
    }
  }

  private calculateNextSquare(
    actor: Actor,
    initialTile: Tile,
    x: number,
    y: number,
    previousSquare: ActionSquare,
    remainedActionPoints: number,
    allSquares: ActionSquare[],
    skill: Skill) {
    if (x >= this.battleStorageService.scene.width || x < 0 ||
      y >= this.battleStorageService.scene.height || y < 0) {
      return;
    }
    const tile = this.battleStorageService.scene.tiles[x][y];
    if (actor.canMove && !tile.unbearable && !tile.actor && !tile.decoration &&
      Math.abs(tile.height - initialTile.height) < 10) {

      const existingSquare = allSquares.find(s => s.x === x && s.y === y);
      const actionPointsAfterMove = remainedActionPoints - 1;
      if (existingSquare && actionPointsAfterMove <= existingSquare.remainedPoints) {
        return;
      } else {
        removeFromArray(allSquares, existingSquare);
      }
      const tempSquare = {
        x,
        y,
        remainedPoints: remainedActionPoints - 1,
        parentSquares: !previousSquare ? [] : [previousSquare, ...previousSquare.parentSquares],
        type: ActionSquareTypeEnum.Move,
        topSquare: existingSquare?.topSquare,
        bottomSquare: existingSquare?.bottomSquare,
        leftSquare: existingSquare?.leftSquare,
        rightSquare: existingSquare?.rightSquare
      };
      const leftSquare = allSquares.find(s => s.x === x - 1 && s.y === y);
      if (leftSquare) {
        this.calculateNeighbourhood(tempSquare, leftSquare);
      }
      const rightSquare = allSquares.find(s => s.x === x + 1 && s.y === y);
      if (rightSquare) {
        this.calculateNeighbourhood(tempSquare, rightSquare);
      }
      const topSquare = allSquares.find(s => s.x === x && s.y === y - 1);
      if (topSquare) {
        this.calculateNeighbourhood(tempSquare, topSquare);
      }
      const bottomSquare = allSquares.find(s => s.x === x && s.y === y + 1);
      if (bottomSquare) {
        this.calculateNeighbourhood(tempSquare, bottomSquare);
      }
      allSquares.push(tempSquare);
      if (actionPointsAfterMove === 0) {
        return;
      }
      for (let sX = -1; sX <= 1; sX++) {
        for (let sY = -1; sY <= 1; sY++) {
          if (Math.abs(sX) === 1 && sY === 0 || sX === 0 && Math.abs(sY) === 1) {
            this.calculateNextSquare(actor, tile, x + sX, y + sY, tempSquare, actionPointsAfterMove, allSquares, skill);
          }
        }
      }
      this.calculateRangeSquares(actor, tile, x, y, tempSquare,
        actionPointsAfterMove - skill.cost, allSquares, true, skill);
      return;
    }
    return;
  }

  calculateActiveSquares(actor: Actor, actionId?: number): ActionSquare[] {
    if (actor.actionPoints === 0) {
      return [];
    }
    const actorSquare = {
      x: actor.x,
      y: actor.y,
      remainedPoints: actor.actionPoints,
      parentSquares: [],
      type: ActionSquareTypeEnum.Actor
    };
    const allSquares: ActionSquare[] = [actorSquare];
    const actorTile = this.battleStorageService.scene.tiles[actor.x][actor.y];
    const skill = actionId || actionId === -1 ? actor.skills.find(x => x.id === actionId) : actor.attackingSkill;
    if (!skill) {
      return [];
    }
    for (let sX = -1; sX <= 1; sX++) {
      for (let sY = -1; sY <= 1; sY++) {
        if (Math.abs(sX) === 1 && sY === 0 || sX === 0 && Math.abs(sY) === 1) {
          if (!actionId) {
            this.calculateNextSquare(actor, actorTile, actor.x + sX, actor.y + sY, actorSquare, actor.actionPoints, allSquares, skill);
          }
        }
      }
    }
    this.calculateRangeSquares(actor, actorTile, actor.x, actor.y, actorSquare,
      actor.actionPoints - skill.cost, allSquares, !actionId, skill);
    return allSquares;
  }
}
