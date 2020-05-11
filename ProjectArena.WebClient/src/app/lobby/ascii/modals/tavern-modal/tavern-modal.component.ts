import { Component, OnInit, OnDestroy } from '@angular/core';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { Subject } from 'rxjs';
import { OverlayRef } from '@angular/cdk/overlay';
import { UserService } from 'src/app/shared/services/user.service';
import { CharacterForSale } from '../../model/character-for-sale.model';
import { TavernStateEnum } from '../../model/enums/tavern-state.enum';
import { QueueService } from 'src/app/lobby/services/queue.service';
import { FormGroup, FormBuilder, MinLengthValidator } from '@angular/forms';
import { AppFormGroup } from 'src/app/shared/components/form-group/app-form-group';
import { Character } from '../../model/character.model';
import { exactWordValidator } from 'src/app/shared/validators/exact-word.validator';
import { controlMaxLengthValidator } from 'src/app/shared/validators/control-max-length-validator';
import { controlRequiredValidator } from 'src/app/shared/validators/control-required.validator';
import { ComponentSizeEnum } from 'src/app/shared/models/enum/component-size.enum';
import { WebCommunicationService } from 'src/app/shared/services/web-communication.service';
import { HirePatronRequest } from '../../model/requests/hire-patron-request.model';
import { NewCharacterResponse } from '../../model/requests/new-character-response.model';

@Component({
  selector: 'app-tavern-modal',
  templateUrl: './tavern-modal.component.html',
  styleUrls: ['./tavern-modal.component.scss']
})
export class TavernModalComponent implements OnInit, OnDestroy, IModal<any> {

  nameForm: AppFormGroup;
  fireForm: AppFormGroup;

  onClose = new Subject<any>();
  onCancel = new Subject<any>();

  loading: boolean;
  errors: string[];

  state: TavernStateEnum;
  currentPatron: CharacterForSale;
  currentCharacterForReplace: Character;

  tavernStateEnum = TavernStateEnum;
  componentSizeEnum = ComponentSizeEnum;

  constructor(
    private overlay: OverlayRef,
    private userService: UserService,
    private queueService: QueueService,
    private formBuilder: FormBuilder,
    private webCommunicationService: WebCommunicationService
  ) { }


  ngOnInit(): void {
    if (this.queueService.inQueue) {
      this.state = TavernStateEnum.Restricted;
      return;
    }
    this.state = TavernStateEnum.Tavern;
    this.fireForm = new AppFormGroup({
      textField: this.formBuilder.control('', [
        exactWordValidator($localize`:@@controls.text-field-small:text field`, 'FIRE')
      ]),
    });
    this.nameForm = new AppFormGroup({
      textField: this.formBuilder.control('', [
        controlRequiredValidator($localize`:@@controls.text-field:Text field`),
        controlMaxLengthValidator($localize`:@@controls.text-field:Text field`, 15)
      ]),
    });
  }

  close() {
    this.onClose.next();
    this.overlay.detach();
    this.overlay.dispose();
  }

  ngOnDestroy() {
    this.onClose.unsubscribe();
    this.onCancel.unsubscribe();
  }

  closeOnClick(event) {
    if (event.target !== event.currentTarget || (this.loading && !this.errors)) {
      return;
    }
    this.close();
  }

  choosePatron(patron: CharacterForSale) {
    this.currentPatron = patron;
    if (this.userService.user.roster.length >= 6) {
      this.state = TavernStateEnum.ChooseReplacement;
    } else {
      this.currentCharacterForReplace = undefined;
      this.state = TavernStateEnum.ChooseName;
    }
  }

  chooseCharacter(character: Character) {
    this.currentCharacterForReplace = character;
    this.state = TavernStateEnum.ConfirmReplacement;
  }

  fireCharacter() {
    const errors = this.fireForm.appErrors;
    if (errors.length > 0) {
      this.errors = errors;
      this.loading = true;
    } else {
      this.state = TavernStateEnum.ChooseName;
    }
  }

  hirePatron() {
    const errors = this.nameForm.appErrors;
    if (errors.length > 0) {
      this.errors = errors;
      this.loading = true;
    } else {
      this.errors = undefined;
      this.loading = true;
      this.webCommunicationService.put<HirePatronRequest, NewCharacterResponse>('api/user/character', {
        patronId: this.currentPatron.id,
        characterForReplace: this.currentCharacterForReplace?.id,
        name: this.nameForm.controls.textField.value
      })
        .subscribe((result) => {
          if (result.success) {
            this.userService.user.tavern.splice(this.userService.user.tavern.indexOf(this.currentPatron), 1);
            const newCharacter = {
              id: result.result.id,
              name: this.nameForm.controls.textField.value,
              nativeId: 'adventurer'
            } as Character;
            if (this.currentCharacterForReplace) {
              this.userService.user.roster[this.userService.user.roster.indexOf(this.currentCharacterForReplace)] = newCharacter;
            } else {
              this.userService.user.roster.push(newCharacter);
            }
            this.userService.userChanged.next();
            this.state = TavernStateEnum.Tavern;
            this.errors = [`You recruited ${newCharacter.name}.`];
          } else {
            this.errors = result.errors;
          }
        });
    }
  }

  backFromName() {
    if (this.userService.user.roster.length >= 6) {
      this.state = TavernStateEnum.ConfirmReplacement;
    } else {
      this.currentCharacterForReplace = undefined;
      this.state = TavernStateEnum.Tavern;
    }
  }
}
