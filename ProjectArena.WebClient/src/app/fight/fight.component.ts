import { Component, OnInit } from '@angular/core';
import { LoadingService } from '../shared/services/loading.service';
import { SceneService } from '../engine/services/scene.service';
import { SynchronizationService } from '../engine/services/synchronization.service';
import { FullSynchronizationInfo } from '../shared/models/synchronization/full-synchronization-info.model';
import { ActorSynchronization } from '../shared/models/synchronization/objects/actor-synchronization.model';

@Component({
  selector: 'app-fight',
  templateUrl: './fight.component.html',
  styleUrls: ['./fight.component.scss']
})
export class FightComponent implements OnInit {

  constructor(
    private loadingService: LoadingService,
    private sceneService: SceneService
  ) { }

  ngOnInit(): void {
    this.loadingService.finishLoading()
            .subscribe();
  }

  createSampleScene() {
    let idCounterPosition = 1000;
    const actors: ActorSynchronization[] = [];
    let tilesCounter = 1;
    for (let x = 0; x < 28; x++) {
      for (let y = 0; y < 16; y++) {
        actors.push({
          reference: {
            id: idCounterPosition++,
            x,
            y
          },
          name: 'Ground',
          char: 'ground',
          color: {r: 60, g: 61, b: 95, a: 1},
          ownerId: undefined,
          tags: ['tile'],
          parentId: tilesCounter,
          durability: 100000,
          maxDurability: 100000,
          turnCost: 1,
          initiativePosition: 0,
          height: 300,
          volume: 15000,
          freeVolume: 2700,
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
          name: 'Grass',
          char: 'grass',
          color: { r: 45, g: 60, b: 150, a: 1 },
          ownerId: undefined,
          tags: ['tile'],
          parentId: tilesCounter,
          durability: 100,
          maxDurability: 100,
          turnCost: 1,
          initiativePosition: 0,
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
        tilesCounter++;
      }
    }
    actors.push({
      reference: {
        id: 5000,
        x: 5,
        y: 5
      },
      name: 'Actor',
      char: 'adventurer',
      color: { r: 55, g: 55, b: 255, a: 1 },
      ownerId: 'sampleP',
      tags: ['intelligent'],
      parentId: 1 + 5 * 16 + 5,
      durability: 100,
      maxDurability: 100,
      turnCost: 1,
      initiativePosition: 0,
      height: 180,
      volume: 120,
      freeVolume: 40,
      preparationReactions: [],
      activeReactions: [],
      clearReactions: [],
      actions: [],
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
            id: 'sampleP'
          }
        ],
        width: 28,
        height: 16,
        waitingActions: []
      },
      undefined,
      {
        time: 8000000000,
        tempActor: {
          id: 5000,
          x: 5,
          y: 5
        }
      }
    );
  }

}
