import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { SceneService } from 'src/app/engine/services/scene.service';
import { BiomEnum } from 'src/app/shared/models/enum/biom.enum';
import { BattlePlayerStatusEnum } from 'src/app/shared/models/enum/player-battle-status.enum';
import { FullSynchronizationInfo } from 'src/app/shared/models/synchronization/full-synchronization-info.model';
import { ActorSynchronization } from 'src/app/shared/models/synchronization/objects/actor-synchronization.model';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';
import { UserService } from 'src/app/shared/services/user.service';
import { WebCommunicationService } from 'src/app/shared/services/web-communication.service';

@Injectable()
export class FightResolverService implements Resolve<boolean> {

  constructor(
    private userService: UserService,
    private arenaHubService: ArenaHubService,
    private webCommunicationService: WebCommunicationService,
    private sceneService: SceneService
  ) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | Observable<boolean> | Promise<boolean> {
    return this.createSampleScene()
      .pipe(map(scene => {
        this.sceneService.setupGame(scene);
        return true;
      }));
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
        id: 5000,
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
      activeReactions: [
        {
          id: 'physicalResponse',
          mod: 1
        }
      ],
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
        }
      ],
      actors: [],
      buffs: [],
    });
    actors.push({
      reference: {
        id: 5001,
        x: 12,
        y: 6
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
      activeReactions: [
        {
          id: 'physicalResponse',
          mod: 1
        }
      ],
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
        }
      ],
      actors: [],
      buffs: [],
    });
    actors.push({
      reference: {
        id: 5003,
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
      activeReactions: [
        {
          id: 'physicalResponse',
          mod: 1
        }
      ],
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
          id: 'shot',
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
        id: 5004,
        x: 13,
        y: 5
      },
      position: 1,
      left: false,
      name: 'Actor',
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
      activeReactions: [
        {
          id: 'physicalResponse',
          mod: 1
        }
      ],
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
        }
      ],
      actors: [],
      buffs: [],
    });
    return of({
      id: 'sampleS',
      timeLine: 0,
      idCounterPosition: 5010,
      actors,
      players: [
        {
          id: 'sampleP',
          keyActors: [ 5003, 5004 ],
          battlePlayerStatus: BattlePlayerStatusEnum.Playing
        },
        {
          id: 'sampleP2',
          keyActors: [ 5000, 5001 ],
          battlePlayerStatus: BattlePlayerStatusEnum.Playing
        }
      ],
      currentActor: {
        id: idCounterPosition,
        x: 13,
        y: 6
      },
      width: 14,
      height: 8,
      biom: BiomEnum.Grass
    } as FullSynchronizationInfo);
  }
}
