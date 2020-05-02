import { Injectable } from '@angular/core';
import { Synchronizer } from 'src/app/shared/models/battle/synchronizer.model';
import { AnimationDeclaration } from '../models/animations/animation-declaration.model';
import { Subject } from 'rxjs';
import { BattleSynchronizationActionEnum } from 'src/app/shared/models/enum/battle-synchronization-action.enum';
import { Actor } from '../models/scene/actor.model';
import { Skill } from '../models/scene/skill.model';
import { AsciiBattleSynchronizerService } from './ascii-battle-synchronizer.service';
import { AsciiBattleStorageService } from './ascii-battle-storage.service';
import { AnimationTile } from '../models/animations/animation-tile.model';
import { AnimationFrame } from '../models/animations/animation-frame.model';

@Injectable()
export class AsciiBattleAnimationsService {

  animationsQueue: AnimationDeclaration[] = [];

  animationsLoaded: boolean;

  generationConclusion = new Subject<any>();

  constructor(
    private battleSynchronizationService: AsciiBattleSynchronizerService,
    private battleStorageService: AsciiBattleStorageService,
    ) { }

  private synchronizeFromSynchronizer(synchronizer: { action: BattleSynchronizationActionEnum, sync: Synchronizer}) {
    this.battleSynchronizationService.synchronizeScene(synchronizer.sync);
  }

  private mergeFramesToDeclarations(
    action: BattleSynchronizationActionEnum,
    synchronizer: { action: BattleSynchronizationActionEnum, sync: Synchronizer},
    ignoreSynchronizer: boolean,
    ...frames: AnimationFrame[][]): AnimationDeclaration[] {
    const syncIndexes = frames.map(f => {
      let index = f.findIndex(x => x.updateSynchronizer);
      if (index === -1) {
        if (ignoreSynchronizer) {
          index = 0;
        } else {
          index = f.length;
          f.length += 1;
        }
      }
      return index;
    });
    const animationsTilSync = Math.max(...syncIndexes);
    const lengths = frames.map((f, index) => {
      return f.length + animationsTilSync - syncIndexes[index];
    });
    const maxLength = Math.max(...lengths);
    for (let i = 0; i < frames.length; i++) {
      frames[0].unshift(...new Array<AnimationFrame>(animationsTilSync - syncIndexes[i]));
    }
    console.log('MERGE');
    console.log(syncIndexes);
    console.log(animationsTilSync);
    console.log(lengths);
    console.log(maxLength);
    console.log(frames);
    console.log('END MERGE');
    const declarations = new Array<AnimationDeclaration>(maxLength);
    for (let i = 0; i < maxLength; i++) {
      const tiles = new Array<AnimationTile[]>(this.battleStorageService.scene.width);
      for (let t = 0; t < this.battleStorageService.scene.width; t++) {
        tiles[t] = new Array<AnimationTile>(this.battleStorageService.scene.height);
      }
      for (const frameContainer of frames) {
        if (frameContainer[i]) {
          for (const tile of frameContainer[i].animationTiles) {
            const compareTile = tiles[tile.x][tile.y];
            if (!compareTile || compareTile.priority < tile.priority) {
              tiles[tile.x][tile.y] = tile;
            }
          }
        }
      }
      declarations[i] = {
        waitingForSynchronizer: !ignoreSynchronizer && animationsTilSync === i && !synchronizer ? action : undefined,
        updateSynchronizer: !ignoreSynchronizer && animationsTilSync === i && synchronizer ? synchronizer : undefined,
        animationTiles: tiles
      };
    }
    console.log(declarations);
    return declarations;
  }

  generateAnimationsFromSynchronizer(
    synchronizer: { action: BattleSynchronizationActionEnum, sync: Synchronizer} ,
    onlySecondPart: boolean = false): boolean {

    let notUploadSynchronizer = false;
    if (onlySecondPart) {
      const action = this.animationsQueue.find(x => x.waitingForSynchronizer);
      if (action.waitingForSynchronizer === synchronizer.action) {
        action.waitingForSynchronizer = undefined;
        action.updateSynchronizer = synchronizer;
        notUploadSynchronizer = true;
      }
    }
    const issuer = synchronizer.sync.actorId && synchronizer.action !== BattleSynchronizationActionEnum.Decoration ?
      this.battleStorageService.scene.actors.find(x => x.id === synchronizer.sync.actorId) :
      undefined;
    const frames: AnimationFrame[][] = [];
    switch (synchronizer.action) {
      case BattleSynchronizationActionEnum.Attack:
        if (issuer) {
          frames.push([]);
          if (!onlySecondPart && issuer.attackingSkill.action.generateIssueDeclarations) {
            frames[0].push(...issuer.attackingSkill.action
              .generateIssueDeclarations(
                issuer.x,
                issuer.y,
                synchronizer.sync.targetX,
                synchronizer.sync.targetY,
                issuer.attackingSkill.action));
          }
          if (issuer.attackingSkill.action.generateSyncDeclarations) {
            frames[0].push(...issuer.attackingSkill.action
              .generateSyncDeclarations(
                issuer.x,
                issuer.y,
                synchronizer.sync.targetX,
                synchronizer.sync.targetY,
                issuer.attackingSkill.action));
          }
        }
        break;
      case BattleSynchronizationActionEnum.Move:
        frames.push([]);
        break;
      case BattleSynchronizationActionEnum.Cast:
        if (issuer) {
          frames.push([]);
          const skill = issuer.skills.find(x => x.id === synchronizer.sync.skillActionId);
          if (skill) {
            if (!onlySecondPart && skill.action.generateIssueDeclarations) {
              frames[0].push(...skill.action
                .generateIssueDeclarations(
                  issuer.x,
                  issuer.y,
                  synchronizer.sync.targetX,
                  synchronizer.sync.targetY,
                  issuer.attackingSkill.action));
            }
            if (skill.action.generateSyncDeclarations) {
              frames[0].push(...skill.action
                .generateSyncDeclarations(
                  issuer.x,
                  issuer.y,
                  synchronizer.sync.targetX,
                  synchronizer.sync.targetY,
                  issuer.attackingSkill.action));
            }
          }
        }
        break;
      case BattleSynchronizationActionEnum.Wait:
        frames.push([]);
        break;
      case BattleSynchronizationActionEnum.Decoration:
        const decoration = this.battleStorageService.scene.decorations.find(x => x.id === synchronizer.sync.actorId);
        if (decoration && decoration.action) {
          frames.push([]);
          frames[0].push(...decoration.action
            .generateDeclarations(
              issuer.x,
              issuer.y,
              synchronizer.sync.targetX,
              synchronizer.sync.targetY,
              decoration.action));
        }
        break;
      case BattleSynchronizationActionEnum.EndTurn:
        frames.push([]);
        console.log('EndTurn');
        break;
      case BattleSynchronizationActionEnum.StartGame:
        return false;
      case BattleSynchronizationActionEnum.EndGame:
        console.log('EndGame');
        frames.push([]);
        break;
      case BattleSynchronizationActionEnum.SkipTurn:
        console.log('SkipTurn');
        return false;
    }
    const declarations = this.mergeFramesToDeclarations(synchronizer.action, synchronizer, notUploadSynchronizer, ...frames);
    this.animationsQueue.push(...declarations);
    this.animationsLoaded = true;
    console.log(declarations);
    return true;
  }

  generateAnimationsFromIssue(action: BattleSynchronizationActionEnum, actor: Actor, x?: number, y?: number, skillId?: number): boolean {
    if (action === BattleSynchronizationActionEnum.Move || action === BattleSynchronizationActionEnum.Wait) {
      return false;
    }
    const skill = action === BattleSynchronizationActionEnum.Attack ? actor.attackingSkill : actor.skills.find(s => s.id === skillId);
    if (!skill.action?.generateIssueDeclarations) {
      return false;
    }
    const declarations = skill.action.generateIssueDeclarations(actor.x, actor.y, x, y, skill.action);
    this.animationsQueue.push(...this.mergeFramesToDeclarations(action, undefined, false, declarations));
    this.animationsLoaded = true;
    console.log(declarations);
    return true;
  }

  processNextAnimationFromQueue(): boolean {
    if (this.animationsQueue.length === 0) {
      this.battleStorageService.currentAnimations = undefined;
      if (this.animationsLoaded) {
        this.animationsLoaded = false;
        this.generationConclusion.next();
      }
      return false;
    }
    const animation = this.animationsQueue.shift();
    this.battleStorageService.currentAnimations = animation.animationTiles;
    if (animation.updateSynchronizer) {
      this.synchronizeFromSynchronizer(animation.updateSynchronizer);
    }
    if (this.animationsQueue.length === 0) {
      this.animationsLoaded = false;
      this.battleStorageService.currentAnimations = undefined;
      this.generationConclusion.next();
    }
    return true;
  }
}
