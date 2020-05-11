import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Character } from '../../model/character.model';
import { UserService } from 'src/app/shared/services/user.service';
import { CharacterListElement } from '../../model/character-list-element.model';
import { actorNatives } from 'src/app/battle/ascii/natives';

@Component({
  selector: 'app-characters-list',
  templateUrl: './characters-list.component.html',
  styleUrls: ['./characters-list.component.scss']
})
export class CharactersListComponent implements OnInit {

  @Output() chosenCharacter = new EventEmitter<Character>();

  characters: CharacterListElement[];

  constructor(
    private userService: UserService
  ) { }

  ngOnInit(): void {
    this.characters = this.userService.user.roster.map(x => {
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
