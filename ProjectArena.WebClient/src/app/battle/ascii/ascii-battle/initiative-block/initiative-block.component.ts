import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { Actor } from '../../models/scene/actor.model';
import { InitiativePortrait } from '../../models/gui/initiativePortrait.model';
import { AsciiBattleStorageService } from '../../services/ascii-battle-storage.service';

@Component({
  selector: 'app-initiative-block',
  templateUrl: './initiative-block.component.html',
  styleUrls: ['./initiative-block.component.scss']
})
export class InitiativeBlockComponent implements OnInit, OnDestroy {

  @Output() changeSelected = new EventEmitter<InitiativePortrait>();
  @Output() rightClick = new EventEmitter<{x: number, y: number}>();

  get timer() {
    return this.battleStorageService.turnTime;
  }

  firstActor: InitiativePortrait;
  otherActors: InitiativePortrait[];

  private selectedActior: InitiativePortrait;
  private sub: Subscription;

  constructor(private battleStorageService: AsciiBattleStorageService) { }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
    this.changeSelected.unsubscribe();
    this.rightClick.unsubscribe();
  }

  ngOnInit(): void {
    this.sub = this.battleStorageService.currentInitiativeList.subscribe(result => {
      if (result && result.length > 0) {
        this.firstActor = result[0];
        this.otherActors = result.slice(1);
      } else {
        const dummy = {
          char: '-',
          color: '#444',
          speed: undefined,
          initiativePosition: undefined,
          x: undefined,
          y: undefined
        };
        this.firstActor = dummy;
        this.otherActors = new Array<InitiativePortrait>(5);
        for (let i = 0; i < 5; i++) {
          this.otherActors[i] = dummy;
        }
      }
    });
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
    this.rightClick.next({x: portrait.x, y: portrait.y});
  }
}
