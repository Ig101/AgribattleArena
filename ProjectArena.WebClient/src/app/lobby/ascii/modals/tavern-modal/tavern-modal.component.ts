import { Component, OnInit, OnDestroy } from '@angular/core';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { Subject } from 'rxjs';
import { OverlayRef } from '@angular/cdk/overlay';
import { UserService } from 'src/app/shared/services/user.service';
import { CharacterForSale } from '../../model/character-for-sale.model';

@Component({
  selector: 'app-tavern-modal',
  templateUrl: './tavern-modal.component.html',
  styleUrls: ['./tavern-modal.component.scss']
})
export class TavernModalComponent implements OnInit, OnDestroy, IModal<any> {

  onClose = new Subject<any>();
  onCancel = new Subject<any>();

  choosingReplacement = false;
  confirmingReplacement = false;
  currentPatron: CharacterForSale;

  constructor(
    private overlay: OverlayRef,
    private userService: UserService
  ) { }


  ngOnInit(): void {
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
    if (event.target !== event.currentTarget) {
      return;
    }
    this.close();
  }

  choosePatron(patron: CharacterForSale) {

  }


}
