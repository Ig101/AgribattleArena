import { Injectable } from '@angular/core';
import { AsciiBattleStorageService } from './ascii-battle-storage.service';
import { Synchronizer } from 'src/app/shared/models/battle/synchronizer.model';
import { Player } from '../models/player.model';
import { Tile } from '../models/scene/tile.model';
import { convertTile, convertActor, convertDecoration, convertEffect } from '../helpers/scene-create.helper';
import { Actor } from '../models/scene/actor.model';
import { ActiveDecoration } from '../models/scene/active-decoration.model';
import { SpecEffect } from '../models/scene/spec-effect.model';
import { Scene } from '../models/scene/scene.model';
import { synchronizeTile, synchronizeActor, synchronizeDecoration, synchronizeEffect } from '../helpers/scene-update.helper';
import { removeFromArray } from 'src/app/helpers/extensions/array.extension';
import { UserService } from 'src/app/shared/services/user.service';
import { tileNatives } from '../natives';
import { AsciiBattlePathCreatorService } from './ascii-battle-path-creator.service';
import { SynchronizationDifference } from '../models/synchronization-differences/synchronization-differences.model';

@Injectable()
export class AsciiBattleSynchronizerService {

  constructor(
    private battleStorageService: AsciiBattleStorageService,
    private battlePathCreator: AsciiBattlePathCreatorService,
    private userService: UserService
  ) { }

  restoreSceneFromSnapshot(synchronizer: Synchronizer) {
    // Players
    let currentPlayer: Player;
    this.battleStorageService.players = synchronizer.players.map(player => {
      const isCurrentPlayer = player.id === this.userService.user.id;
      const newPlayer = {
        id: player.id,
        team: player.team,
        name: isCurrentPlayer ? this.userService.user.name : 'Mistspawn gang',
        keyActors: player.keyActorsSync,
        turnsSkipped: player.turnsSkipped,
        status: player.status
      };
      if (isCurrentPlayer) {
        currentPlayer = newPlayer;
      }
      return newPlayer;
    });
    this.battleStorageService.turnTime = synchronizer.turnTime;

    // Tiles
    const tiles = new Array<Tile[]>(synchronizer.tilesetWidth);
    for (let x = 0; x < synchronizer.tilesetWidth; x++) {
      tiles[x] = new Array<Tile>(synchronizer.tilesetHeight);
    }
    for (const tile of synchronizer.changedTiles) {
      const owner = this.battleStorageService.players.find(x => x.id === tile.ownerId);
      tiles[tile.x][tile.y] = convertTile(tile, owner);
    }

    // Actors
    const actors: Actor[] = [];
    for (const actor of synchronizer.changedActors) {
      const owner = this.battleStorageService.players.find(x => x.id === actor.ownerId);
      const character = this.userService.user.roster.find(x => x.id === actor.externalId);
      const newActor = convertActor(actor, owner, owner && currentPlayer.team === owner?.team, character?.name);
      for (const buff of newActor.buffs) {
        buff.passiveAnimation?.doSomethingWithBearer(buff.passiveAnimation, newActor);
      }
      tiles[newActor.x][newActor.y].actor = newActor;
      actors.push(newActor);
    }

    // Decorations
    const decorations: ActiveDecoration[] = [];
    for (const decoration of synchronizer.changedDecorations) {
      const owner = this.battleStorageService.players.find(x => x.id === decoration.ownerId);
      const newDecoration = convertDecoration(decoration, owner, owner && currentPlayer.team === owner?.team);
      tiles[newDecoration.x][newDecoration.y].decoration = newDecoration;
      decorations.push(newDecoration);
    }

    // SpecEffects
    const effects: SpecEffect[] = [];
    for (const specEffect of synchronizer.changedEffects) {
      const owner = this.battleStorageService.players.find(x => x.id === specEffect.ownerId);
      const newEffect = convertEffect(specEffect, owner, owner && currentPlayer.team === owner?.team);
      tiles[newEffect.x][newEffect.y].specEffects.push(newEffect);
      tiles[newEffect.x][newEffect.y].specEffects.sort((a, b) => b.z - a.z);
      effects.push(newEffect);
    }

    // TempValues
    this.battleStorageService.currentActor = synchronizer.tempActor ?
      actors.find(x => x.id === synchronizer.tempActor) :
      undefined;
    this.battleStorageService.currentDecoration = synchronizer.tempDecoration ?
      decorations.find(x => x.id === synchronizer.tempDecoration) :
      undefined;

    this.battleStorageService.scene = {
      id: synchronizer.id,
      actors,
      decorations,
      effects,
      tiles,
      width: synchronizer.tilesetWidth,
      height: synchronizer.tilesetHeight
    } as Scene;
    this.battleStorageService.cameraX = this.battleStorageService.scene.width / 2;
    this.battleStorageService.cameraY = this.battleStorageService.scene.height / 2;
    this.battleStorageService.zoom = 2;
    this.battleStorageService.defaultActionSquares = this.battleStorageService.currentActor?.owner === currentPlayer ?
    this.battlePathCreator.calculateActiveSquares(this.battleStorageService.currentActor) :
    undefined;
    this.battleStorageService.availableActionSquares = this.battleStorageService.defaultActionSquares;
  }

  synchronizeScene(synchronizer: Synchronizer): SynchronizationDifference {
    if (!synchronizer.tempActor) {
      synchronizer.tempActor = undefined;
    }
    if (!synchronizer.tempDecoration) {
      synchronizer.tempDecoration = undefined;
    }
    const differences = {
      actors: [],
      decorations: []
    } as SynchronizationDifference;
    // Players
    let currentPlayer: Player;
    for (const syncPlayer of synchronizer.players) {
      const player = this.battleStorageService.players.find(x => x.id === syncPlayer.id);
      player.status = syncPlayer.status;
      player.team = syncPlayer.team;
      player.turnsSkipped = syncPlayer.turnsSkipped;
      if (player.id === this.userService.user.id) {
        currentPlayer = player;
      }
    }
    // Changes
    for (const syncTile of synchronizer.changedTiles) {
      const tile = this.battleStorageService.scene.tiles[syncTile.x][syncTile.y];
      let owner: Player;
      if (syncTile.ownerId !== tile.owner?.id) {
        owner = this.battleStorageService.players.find(x => x.id === syncTile.ownerId);
      }
      synchronizeTile(tile, syncTile, owner);
    }
    for (const syncActor of synchronizer.changedActors) {
      let owner: Player;
      const actor = this.battleStorageService.scene.actors.find(x => x.id === syncActor.id);
      if (!actor) {
        owner = this.battleStorageService.players.find(x => x.id === syncActor.ownerId);
        const character = this.userService.user.roster.find(x => x.id === actor.externalId);
        const newActor = convertActor(syncActor, owner, owner && currentPlayer.team === owner?.team, character?.name);
        this.battleStorageService.scene.tiles[newActor.x][newActor.y].actor = newActor;
        this.battleStorageService.scene.actors.push(newActor);
        continue;
      }
      if (actor.x !== syncActor.x || actor.y !== syncActor.y) {
        if (this.battleStorageService.scene.tiles[actor.x][actor.y].actor === actor) {
          this.battleStorageService.scene.tiles[actor.x][actor.y].actor = undefined;
        }
        this.battleStorageService.scene.tiles[syncActor.x][syncActor.y].actor = actor;
      }
      if (syncActor.ownerId !== actor.owner?.id) {
        owner = this.battleStorageService.players.find(x => x.id === syncActor.ownerId);
      }
      const difference = synchronizeActor(actor, syncActor, currentPlayer.team === (owner ? owner?.team : actor.owner?.team));
      if (difference) {
        differences.actors.push(difference);
      }
    }
    for (const syncDecoration of synchronizer.changedDecorations) {
      let owner: Player;
      const decoration = this.battleStorageService.scene.decorations.find(x => x.id === syncDecoration.id);
      if (!decoration) {
        owner = this.battleStorageService.players.find(x => x.id === syncDecoration.ownerId);
        const newDecoration = convertDecoration(syncDecoration, owner, owner && currentPlayer.team === owner?.team);
        this.battleStorageService.scene.tiles[newDecoration.x][newDecoration.y].decoration = newDecoration;
        this.battleStorageService.scene.decorations.push(newDecoration);
        continue;
      }
      if (decoration.x !== syncDecoration.x || decoration.y !== syncDecoration.y) {
        if (this.battleStorageService.scene.tiles[decoration.x][decoration.y].decoration === decoration) {
          this.battleStorageService.scene.tiles[decoration.x][decoration.y].decoration = undefined;
        }
        this.battleStorageService.scene.tiles[syncDecoration.x][syncDecoration.y].decoration = decoration;
      }
      if (syncDecoration.ownerId !== decoration.owner?.id) {
        owner = this.battleStorageService.players.find(x => x.id === syncDecoration.ownerId);
      }
      const difference = synchronizeDecoration(decoration, syncDecoration, owner);
      if (difference) {
        differences.decorations.push(difference);
      }
    }
    for (const syncEffect of synchronizer.changedEffects) {
      let owner: Player;
      const effect = this.battleStorageService.scene.effects.find(x => x.id === syncEffect.id);
      if (!effect) {
        owner = this.battleStorageService.players.find(x => x.id === syncEffect.ownerId);
        const newEffect = convertEffect(syncEffect, owner, owner && currentPlayer.team === owner?.team);
        this.battleStorageService.scene.tiles[newEffect.x][newEffect.y].specEffects.push(newEffect);
        this.battleStorageService.scene.tiles[newEffect.x][newEffect.y].specEffects.sort((a, b) => b.z - a.z);
        this.battleStorageService.scene.effects.push(newEffect);
        continue;
      }
      if (effect.x !== syncEffect.x || effect.y !== syncEffect.y) {
        removeFromArray(this.battleStorageService.scene.tiles[effect.x][effect.y].specEffects, effect);
        this.battleStorageService.scene.tiles[syncEffect.x][syncEffect.y].specEffects.push(effect);
        this.battleStorageService.scene.tiles[syncEffect.x][syncEffect.y].specEffects.sort((a, b) => b.z - a.z);
      }
      if (syncEffect.ownerId !== effect.owner?.id) {
        owner = this.battleStorageService.players.find(x => x.id === syncEffect.ownerId);
      }
      synchronizeEffect(effect, syncEffect, owner);
    }

    // Deletions
    for (const syncActor of synchronizer.deletedActors) {
      const actor = this.battleStorageService.scene.actors.find(x => x.id === syncActor);
      if (actor) {
        differences.actors.push({
          x: actor.x,
          y: actor.y,
          actor: undefined,
          healthChange: -Math.ceil(actor.health),
          newBuffs: [],
          removedBuffs: [],
          endedTurn: false,
          changedPosition: false
        });
        removeFromArray(this.battleStorageService.scene.actors, actor);
        if (this.battleStorageService.scene.tiles[actor.x][actor.y].actor === actor) {
          this.battleStorageService.scene.tiles[actor.x][actor.y].actor = undefined;
        }
      }
    }
    for (const syncDecoration of synchronizer.deletedDecorations) {
      const decoration = this.battleStorageService.scene.decorations.find(x => x.id === syncDecoration);
      if (decoration) {
        differences.decorations.push({
          x: decoration.x,
          y: decoration.y,
          decoration: undefined,
          healthChange: -Math.ceil(decoration.health),
          changedPosition: false
        });
        removeFromArray(this.battleStorageService.scene.decorations, decoration);
        if (this.battleStorageService.scene.tiles[decoration.x][decoration.y].decoration === decoration) {
          this.battleStorageService.scene.tiles[decoration.x][decoration.y].decoration = undefined;
        }
      }
    }
    for (const syncEffect of synchronizer.deletedEffects) {
      const effect = this.battleStorageService.scene.effects.find(x => x.id === syncEffect);
      if (effect) {
        removeFromArray(this.battleStorageService.scene.effects, effect);
        removeFromArray(this.battleStorageService.scene.tiles[effect.x][effect.y].specEffects, effect);
        this.battleStorageService.scene.tiles[effect.x][effect.y].specEffects.sort((a, b) => b.z - a.z);
      }
    }

    // Change turn
    if (synchronizer.tempActor !== this.battleStorageService.currentActor?.id ||
        synchronizer.tempDecoration !== this.battleStorageService.currentDecoration?.id) {
      if (this.battleStorageService.currentActor) {
        let difference = differences.actors.find(x => x.actor.id === this.battleStorageService.currentActor.id);
        if (!difference) {
          difference = {
            x: this.battleStorageService.currentActor.x,
            y: this.battleStorageService.currentActor.y,
            actor: this.battleStorageService.currentActor,
            healthChange: 0,
            newBuffs: [],
            removedBuffs: [],
            endedTurn: true,
            changedPosition: false
          };
          differences.actors.push(difference);
        } else {
          difference.endedTurn = true;
        }
      }
      this.battleStorageService.currentActor = synchronizer.tempActor ?
        this.battleStorageService.scene.actors.find(x => x.id === synchronizer.tempActor) :
        undefined;
      this.battleStorageService.currentDecoration = synchronizer.tempDecoration ?
        this.battleStorageService.scene.decorations.find(x => x.id === synchronizer.tempDecoration) :
        undefined;
      this.battleStorageService.setTurnTime(synchronizer.turnTime);
    }
    this.battleStorageService.defaultActionSquares = this.battleStorageService.currentActor?.owner === currentPlayer ?
      this.battlePathCreator.calculateActiveSquares(this.battleStorageService.currentActor) :
      undefined;
    this.battleStorageService.availableActionSquares = this.battleStorageService.defaultActionSquares;
    return differences;
  }
}
