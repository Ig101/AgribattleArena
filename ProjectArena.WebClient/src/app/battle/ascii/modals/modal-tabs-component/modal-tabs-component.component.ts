import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModalObject } from '../../models/modals/modal-object.model';

@Component({
  selector: 'app-modal-tabs-component',
  templateUrl: './modal-tabs-component.component.html',
  styleUrls: ['./modal-tabs-component.component.scss']
})
export class ModalTabsComponentComponent implements OnInit {

  @Input() modalObjects: ModalObject[];

  @Output() chooseItem = new EventEmitter<ModalObject>();

  constructor() { }

  ngOnInit(): void {
  }

  choose(event, item: ModalObject) {
    event.stopPropagation();
    this.chooseItem.emit(item);
  }
}
