import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { Subject } from 'rxjs';
import { ComponentSizeEnum } from 'src/app/shared/models/enum/component-size.enum';
import { OverlayRef } from '@angular/cdk/overlay';
import { AppFormGroup } from 'src/app/shared/components/form-group/app-form-group';
import { controlRequiredValidator } from 'src/app/shared/validators/control-required.validator';
import { passwordDigitsValidator } from 'src/app/shared/validators/password-digits.validator';
import { passwordLowercaseValidator } from 'src/app/shared/validators/password-lowercase.validator';
import { passwordUppercaseValidator } from 'src/app/shared/validators/password-uppercase.validator';
import { controlMinLengthValidator } from 'src/app/shared/validators/control-min-length.validator';
import { confirmPasswordValidator } from 'src/app/shared/validators/confirm-password.validator';
import { Character } from '../../model/character.model';
import { MODAL_DATA } from 'src/app/shared/services/modal.service';
import { ActorNative } from 'src/app/battle/ascii/models/natives/actor-native.model';
import { actorNatives } from 'src/app/battle/ascii/natives';

@Component({
  selector: 'app-talents-modal',
  templateUrl: './talents-modal.component.html',
  styleUrls: ['./talents-modal.component.scss']
})
export class TalentsModalComponent implements OnInit, OnDestroy, IModal<any> {

  onClose = new Subject<any>();
  onCancel = new Subject<any>();

  loading: boolean;
  errors: string[];

  nativeId: string;
  name: string;
  description: string;
  color: string;

  componentSizeEnum = ComponentSizeEnum;

  constructor(
    @Inject(MODAL_DATA) private data: Character,
    private overlay: OverlayRef
  ) { }

  close() {
    if (!this.loading) {
      this.onClose.next();
      this.overlay.detach();
      this.overlay.dispose();
    }
  }

  ngOnDestroy() {
    this.onClose.unsubscribe();
    this.onCancel.unsubscribe();
  }

  closeOnClick(event) {
    if (event.target !== event.currentTarget) {
      return;
    }
    this.close();
  }

  ngOnInit(): void {
    this.nativeId = this.data.nativeId;
    const native = actorNatives[this.data.nativeId];
    this.description = native.description;
    this.color = `rgba(${native.visualization.color.r}, ${native.visualization.color.g},
      ${native.visualization.color.b}, ${native.visualization.color.a})`;
    this.name = this.data.name;
  }

  undo() {
    
  }
}
