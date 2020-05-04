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
import { FloatingText } from '../models/animations/floating-text.model';

@Injectable()
export class AsciiBattleAnimationsService {

  animationsQueue: AnimationDeclaration[] = [];

  animationsLoaded: boolean;

  generationConclusion = new Subject<boolean>();

  private pending = false;

  constructor(
    private battleSynchronizationService: AsciiBattleSynchronizerService,
    private battleStorageService: AsciiBattleStorageService,
    ) { }

  private synchronizeFromSynchronizer(synchronizer: { action: BattleSynchronizationActionEnum, sync: Synchronizer}): boolean {
    const frames: AnimationFrame[][] = [];
    const floatingTexts: FloatingText[] = [];
    const difference = this.battleSynchronizationService.synchronizeScene(synchronizer.sync);
    for (const actor of difference.actors) {
      const actorFloats = [];
      if (actor.endedTurn) {
        actorFloats.push({
          text: `*end*`,
          color: { r: 255, g: 255, b: 0, a: 1 },
          time: actorFloats.length * -this.battleStorageService.floatingTextDelay,
          x: actor.x,
          y: actor.y,
          height: 0
        });
      }
      if (actor.healthChange) {
        actorFloats.push({
          text: actor.healthChange.toString(),
          color: { r: 255, g: 0, b: 0, a: 1 },
          time: actorFloats.length * -this.battleStorageService.floatingTextDelay,
          x: actor.x,
          y: actor.y,
          height: 0
        });
      }
      for (const buff of actor.newBuffs) {
        actorFloats.push({
          text: `+${buff.char}`,
          color: { r: buff.color.r, g: buff.color.g, b: buff.color.b, a: 1 },
          time: actorFloats.length * -this.battleStorageService.floatingTextDelay,
          x: actor.x,
          y: actor.y,
          height: 0
        });
        const buffFrames = buff.onApplyAnimation?.generateDeclarations(actor.x, actor.y, buff.onApplyAnimation);
        if (buffFrames) {
          buffFrames[buffFrames.length - 1].specificAction = () => {
            buff.passiveAnimation.doSomethingWithBearer(buff.passiveAnimation, actor.actor);
          };
          frames.push(buffFrames);
        } else {
          buff.passiveAnimation.doSomethingWithBearer(buff.passiveAnimation, actor.actor);
        }
      }
      for (const buff of actor.removedBuffs) {
        const buffFrames = buff.onPurgeAnimation?.generateDeclarations(actor.x, actor.y, buff.onApplyAnimation);
        actorFloats.push({
          text: `-${buff.char}`,
          color: { r: buff.color.r, g: buff.color.g, b: buff.color.b, a: 1 },
          time: actorFloats.length * -this.battleStorageService.floatingTextDelay,
          x: actor.x,
          y: actor.y,
          height: 0
        });
        if (buffFrames) {
          buffFrames[buffFrames.length - 1].specificAction = () => {
            buff.passiveAnimation.resetEffect(buff.passiveAnimation, actor.actor);
          };
          frames.push(buffFrames);
        } else {
          buff.passiveAnimation.resetEffect(buff.passiveAnimation, actor.actor);
        }
      }
      if (actor.changedPosition) {
        const tile = this.battleStorageService.scene.tiles[actor.x][actor.y];
        const tileFrames = tile.onStepAction?.generateDeclarations(actor.x, actor.y, tile.onStepAction);
        if (tileFrames) {
          frames.push(tileFrames);
        }
      }
      floatingTexts.push(...actorFloats);
    }
    for (const decoration of difference.decorations) {
      const decorationFloats = [];
      if (decoration.healthChange) {
        decorationFloats.push({
          text: decoration.healthChange.toString(),
          color: { r: 255, g: 0, b: 0, a: 1 },
          time: decorationFloats.length * -this.battleStorageService.floatingTextDelay,
          x: decoration.x,
          y: decoration.y,
          height: 0
        });
      }
      if (decoration.changedPosition) {
        const tile = this.battleStorageService.scene.tiles[decoration.x][decoration.y];
        const tileFrames = tile.onStepAction?.generateDeclarations(decoration.x, decoration.y, tile.onStepAction);
        if (tileFrames) {
          frames.push(tileFrames);
        }
      }
      floatingTexts.push(...decorationFloats);
    }
    if (floatingTexts.length > 0 && frames.length === 0) {
      frames.push([]);
    }

    this.battleStorageService.version = synchronizer.sync.version;
    if (frames.length > 0) {
      const declarations = this.mergeFramesToDeclarations(undefined, undefined, true, frames);
      declarations[0].floatingTexts = floatingTexts;
      this.animationsQueue.push(...declarations);
      return true;
    }
    return false;
  }

  private mergeFramesToDeclarations(
    action: BattleSynchronizationActionEnum,
    synchronizer: { action: BattleSynchronizationActionEnum, sync: Synchronizer},
    ignoreSynchronizer: boolean,
    frames: AnimationFrame[][]): AnimationDeclaration[] {
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
      return Math.max(1, f.length + animationsTilSync - syncIndexes[index]);
    });
    const maxLength = Math.max(...lengths);
    for (let i = 0; i < frames.length; i++) {
      frames[0].unshift(...new Array<AnimationFrame>(animationsTilSync - syncIndexes[i]));
    }
    const declarations = new Array<AnimationDeclaration>(maxLength);
    for (let i = 0; i < maxLength; i++) {
      const tiles = new Array<AnimationTile[]>(this.battleStorageService.scene.width);
      const actions = new Array<() => void>(this.battleStorageService.scene.width);
      for (let t = 0; t < this.battleStorageService.scene.width; t++) {
        tiles[t] = new Array<AnimationTile>(this.battleStorageService.scene.height);
      }
      for (const frameContainer of frames) {
        if (frameContainer[i]) {
          for (const tile of frameContainer[i].animationTiles) {
            if (tile.x >= 0 && tile.y >= 0 &&
              tile.x < this.battleStorageService.scene.width &&
              tile.y < this.battleStorageService.scene.height) {
              const compareTile = tiles[tile.x][tile.y];
              if (!compareTile || compareTile.priority < tile.priority) {
                tiles[tile.x][tile.y] = tile;
              }
            }
          }
          if (frameContainer[i].specificAction) {
            actions.push(frameContainer[i].specificAction);
          }
        }
      }
      declarations[i] = {
        waitingForSynchronizer: !ignoreSynchronizer && animationsTilSync === i && !synchronizer ? action : undefined,
        updateSynchronizer: !ignoreSynchronizer && animationsTilSync === i && synchronizer ? synchronizer : undefined,
        animationTiles: tiles,
        specificActions: actions,
        floatingTexts: undefined
      };
    }
    return declarations;
  }

  private getFramesFromAction(actor: Actor, frames: AnimationFrame[][]): AnimationFrame[][] {
    for (const buff of actor.buffs) {
      buff.onActionEffectAnimation?.generateDeclarations(actor.x, actor.y, buff.onActionEffectAnimation);
    }
    for (const effect of this.battleStorageService.scene.effects) {
      effect.onActionEffectAnimation?.generateDeclarations(effect.x, effect.y, effect.onActionEffectAnimation);
    }
    for (let x = 0; x < this.battleStorageService.scene.width; x++) {
      for (let y = 0; y < this.battleStorageService.scene.height; y++) {
        this.battleStorageService.scene.tiles[x][y].onActionEffectAnimation?.generateDeclarations(x, y,
          this.battleStorageService.scene.tiles[x][y].onActionEffectAnimation);
      }
    }
    return frames;
  }

  private getFramesFromEndTurn(frames: AnimationFrame[][]): AnimationFrame[][] {
    for (const actor of this.battleStorageService.scene.actors) {
      for (const buff of actor.buffs) {
        buff.effectAnimation?.generateDeclarations(actor.x, actor.y, buff.effectAnimation);
      }
    }
    for (const effect of this.battleStorageService.scene.effects) {
      effect.action?.generateDeclarations(effect.x, effect.y, effect.action);
    }
    for (let x = 0; x < this.battleStorageService.scene.width; x++) {
      for (let y = 0; y < this.battleStorageService.scene.height; y++) {
        this.battleStorageService.scene.tiles[x][y].action?.generateDeclarations(x, y,
          this.battleStorageService.scene.tiles[x][y].action);
      }
    }
    return frames;
  }

  generateAnimationsFromSynchronizer(
    synchronizer: { action: BattleSynchronizationActionEnum, sync: Synchronizer} ,
    onlySecondPart: boolean = false): boolean {

    let notUploadSynchronizer = false;
    if (onlySecondPart) {
      const action = this.animationsQueue.find(x => x.waitingForSynchronizer);
      if (action && action.waitingForSynchronizer === synchronizer.action) {
        action.waitingForSynchronizer = undefined;
        action.updateSynchronizer = synchronizer;
        notUploadSynchronizer = true;
      }
    }
    const issuer = synchronizer.sync.actorId && synchronizer.action !== BattleSynchronizationActionEnum.Decoration ?
      this.battleStorageService.scene.actors.find(x => x.id === synchronizer.sync.actorId) :
      undefined;
    const frames: AnimationFrame[][] = [];
    this.pending = false;
    switch (synchronizer.action) {
      case BattleSynchronizationActionEnum.Attack:
        if (issuer) {
          frames.push([]);
          if (!onlySecondPart && issuer.attackingSkill.action?.generateIssueDeclarations) {
            frames[0].push(...issuer.attackingSkill.action
              .generateIssueDeclarations(
                issuer.x,
                issuer.y,
                synchronizer.sync.targetX,
                synchronizer.sync.targetY,
                issuer.attackingSkill.action));
            this.getFramesFromAction(issuer, frames);
          }
          if (issuer.attackingSkill.action?.generateSyncDeclarations) {
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
        if (!onlySecondPart) {
          this.getFramesFromAction(issuer, frames);
        }
        break;
      case BattleSynchronizationActionEnum.Cast:
        if (issuer) {
          frames.push([]);
          const skill = issuer.skills.find(x => x.id === synchronizer.sync.skillActionId);
          if (skill) {
            if (!onlySecondPart) {
              if (skill.action.generateIssueDeclarations) {
              frames[0].push(...skill.action
                .generateIssueDeclarations(
                  issuer.x,
                  issuer.y,
                  synchronizer.sync.targetX,
                  synchronizer.sync.targetY,
                  issuer.attackingSkill.action));
              }
              this.getFramesFromAction(issuer, frames);
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
        return this.synchronizeFromSynchronizer(synchronizer);
      case BattleSynchronizationActionEnum.Decoration:
        const decoration = this.battleStorageService.scene.decorations.find(x => x.id === synchronizer.sync.actorId);
        if (decoration && decoration.action) {
          frames.push([]);
          frames[0].push(...decoration.action
            .generateDeclarations(
              decoration.x,
              decoration.y,
              decoration.action));
        }
        break;
      case BattleSynchronizationActionEnum.EndTurn:
        frames.push([]);
        this.getFramesFromEndTurn(frames);
        break;
      case BattleSynchronizationActionEnum.StartGame:
        return false;
      case BattleSynchronizationActionEnum.EndGame:
        console.log('EndGame');
        return this.synchronizeFromSynchronizer(synchronizer);
      case BattleSynchronizationActionEnum.SkipTurn:
        this.battleStorageService.floatingTexts.push({
          text: `*skipped*`,
          color: { r: 255, g: 255, b: 0, a: 1 },
          time: 0,
          x: this.battleStorageService.currentActor.x,
          y: this.battleStorageService.currentActor.y,
          height: 0
        });
        return this.synchronizeFromSynchronizer(synchronizer);
    }
    const declarations = this.mergeFramesToDeclarations(synchronizer.action, synchronizer, notUploadSynchronizer, frames);
    this.animationsQueue.push(...declarations);
    this.animationsLoaded = true;
    return true;
  }

  generateAnimationsFromIssue(action: BattleSynchronizationActionEnum, actor: Actor, x?: number, y?: number, skillId?: number): boolean {
    if (action === BattleSynchronizationActionEnum.Wait) {
      return false;
    }
    const skill = action === BattleSynchronizationActionEnum.Move ? undefined :
      action === BattleSynchronizationActionEnum.Attack ? actor.attackingSkill : actor.skills.find(s => s.id === skillId);
    const frames = [];
    if (skill?.action?.generateIssueDeclarations) {
      frames.push(skill.action.generateIssueDeclarations(actor.x, actor.y, x, y, skill.action));
    } else {
      frames.push([]);
    }
    this.getFramesFromAction(actor, frames);
    this.animationsQueue.push(...this.mergeFramesToDeclarations(action, undefined, false, frames));
    this.animationsLoaded = true;
    this.pending = true;
    return true;
  }

  processNextAnimationFromQueue(): boolean {
    if (this.animationsQueue.length === 0) {
      this.battleStorageService.currentAnimations = undefined;
      if (this.animationsLoaded) {
        this.animationsLoaded = false;
        this.generationConclusion.next(this.pending);
      }
      return false;
    }
    const animation = this.animationsQueue.shift();
    this.battleStorageService.currentAnimations = animation.animationTiles;
    if (animation.updateSynchronizer) {
      this.synchronizeFromSynchronizer(animation.updateSynchronizer);
    }
    if (animation.floatingTexts) {
      this.battleStorageService.floatingTexts.push(...animation.floatingTexts);
    }
    if (this.animationsQueue.length === 0) {
      this.animationsLoaded = false;
      this.battleStorageService.currentAnimations = undefined;
      this.generationConclusion.next(this.pending);
    }
    return true;
  }
}
