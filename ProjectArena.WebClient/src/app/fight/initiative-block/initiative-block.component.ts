import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { CharsService } from 'src/app/shared/services/chars.service';
import { SceneService } from 'src/app/engine/services/scene.service';
import { InitiativePortrait } from '../models/initiative-portrait.model';
import { Actor } from 'src/app/engine/scene/actor.object';

@Component({
  selector: 'app-initiative-block',
  templateUrl: './initiative-block.component.html',
  styleUrls: ['./initiative-block.component.scss']
})
export class InitiativeBlockComponent implements OnInit, OnDestroy {

  @Output() changeSelected = new EventEmitter<InitiativePortrait>();
  @Output() rightClick = new EventEmitter<Actor>();

  get timer() {
    return !this.sceneService.scene || this.sceneService.scene.turnTime <= 0 ? undefined : this.sceneService.scene?.turnTime;
  }

  get lineWidth() {
    return Math.min(240, this.timer * 8);
  }


  // TODO Turn signature:
  // !turnReallyStarted -> line + currentActor
  // turnTime <= 0 -> line + nextActor
  // other -> currentActor + line
  get startPosition() {
    return !this.sceneService.scene || this.sceneService.scene.turnTime > 0 ? 0 : 1;
  }

  get noOneTurn() {
    return this.sceneService.scene && (this.sceneService.scene.turnTime <= 0 || !this.sceneService.scene.turnReallyStarted);
  }

  actors = new Array<InitiativePortrait>(7);

  private selectedActior: InitiativePortrait;
  private sub: Subscription;

  constructor(
    private sceneService: SceneService,
    public charsService: CharsService
  ) { }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
    this.changeSelected.unsubscribe();
    this.rightClick.unsubscribe();
  }

  ngOnInit(): void {
    this.updateCards();
    this.sub = this.sceneService.synchronizersSub.subscribe(_ => {
      this.updateCards();
    });
  }

  updateCards() {
    if (this.sceneService.scene) {
      const allPortraits = this.sceneService.scene
        .getAllActiveActors()
        .map(x => (
          {
            color: `rgba(${x.color.r},${x.color.g},${x.color.b},${x.color.a})`,
            actor: x,
            initiativePosition: x.initiativePosition,
            turnCost: x.turnCost,
            definition: {
              char: x.char,
              color: x.color
            }
          }));
      if (allPortraits.length === 0) {
        return undefined;
      }
      const portraits: InitiativePortrait[] = [];
      while (portraits.length < 7) {
        let candidate = allPortraits[0];
        let candidateInitiative = allPortraits[0].initiativePosition;
        for (let i = 1; i < allPortraits.length; i++) {
          if (allPortraits[i] && allPortraits[i].initiativePosition < candidateInitiative) {
            candidate = allPortraits[i];
            candidateInitiative = allPortraits[i].initiativePosition;
          }
        }
        portraits.push(candidate);
        candidate.initiativePosition += candidate.turnCost;
      }
      this.actors = portraits;
    }
  }

  enterActorCard(actor: InitiativePortrait) {
    this.selectedActior = actor;
    this.changeSelected.next(actor);
  }

  leaveActorCard(actor: InitiativePortrait) {
    if (this.selectedActior === actor) {
      this.selectedActior = undefined;
      this.changeSelected.emit(undefined);
    }
  }

  onClick(portrait: InitiativePortrait) {
    this.selectedActior = undefined;
    this.changeSelected.emit(undefined);
    this.rightClick.next(portrait.actor);
  }
}
