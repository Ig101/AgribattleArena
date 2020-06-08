import { Component, OnInit, Output, EventEmitter, OnDestroy, Input } from '@angular/core';
import { Character } from '../../model/character.model';
import { UserService } from 'src/app/shared/services/user.service';
import { CharacterListElement } from '../../model/character-list-element.model';
import { actorNatives } from 'src/app/battle/ascii/natives';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-characters-list',
  templateUrl: './characters-list.component.html',
  styleUrls: ['./characters-list.component.scss']
})
export class CharactersListComponent implements OnInit, OnDestroy {

  @Input() includeKeyActors = false;

  @Output() chosenCharacter = new EventEmitter<Character>();

  userChangedSubscription: Subscription;

  characters: CharacterListElement[];

  constructor(
    private userService: UserService
  ) {
    this.userChangedSubscription = this.userService.userChanged.subscribe(() => this.onUpdate() );
  }

  ngOnInit(): void {
    this.characters = this.userService.user.roster
    .filter(x => !x.isKeyCharacter || this.includeKeyActors)
    .map(x => {
      const native = actorNatives[x.nativeId];
      return {
        character: x,
        color: `rgba(${native.visualization.color.r},
          ${native.visualization.color.g},
          ${native.visualization.color.b},
          ${native.visualization.color.a})`
      };
    });
  }

  ngOnDestroy(): void {
    this.userChangedSubscription.unsubscribe();
  }

  onUpdate() {
    this.characters = this.userService.user.roster
    .filter(x => !x.isKeyCharacter || this.includeKeyActors)
    .map(x => {
      const native = actorNatives[x.nativeId];
      return {
        character: x,
        color: `rgba(${native.visualization.color.r},
          ${native.visualization.color.g},
          ${native.visualization.color.b},
          ${native.visualization.color.a})`
      };
    });
  }

  click(item: CharacterListElement) {
    this.chosenCharacter.emit(item.character);
  }
}
