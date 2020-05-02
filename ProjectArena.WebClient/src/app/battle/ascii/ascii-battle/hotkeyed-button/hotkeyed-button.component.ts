import { Component, OnInit, Input, OnDestroy, Output, EventEmitter, HostListener } from '@angular/core';
import { SmartAction } from '../../models/actions/smart-action.model';
import { SmartActionTypeEnum } from '../../models/enum/smart-action-type.enum';

@Component({
  selector: 'app-hotkeyed-button',
  templateUrl: './hotkeyed-button.component.html',
  styleUrls: ['./hotkeyed-button.component.scss']
})
export class HotkeyedButtonComponent implements OnInit, OnDestroy {

  @Input() smartAction: SmartAction;
  @Input() disabled: boolean;
  @Input() pressedKey: string;

  @Output() mouseDown = new EventEmitter<any>();
  @Output() mouseUp = new EventEmitter<any>();

  private timer;

  constructor() { }

  ngOnDestroy(): void {
    clearInterval(this.timer);
  }

  ngOnInit(): void {
    if (this.smartAction?.type === SmartActionTypeEnum.Hold) {
      this.timer = setInterval(() => {
        if (this.smartAction.pressed) {
          this.smartAction.smartValue += 0.03;
        } else {
          this.smartAction.smartValue = 0;
        }
      }, 33);
    }
  }

  @HostListener('mousedown', ['$event'])
  down(event: MouseEvent) {
    this.mouseDown.emit();
    this.smartAction.pressed = true;
  }

  @HostListener('mouseup', ['$event'])
  up(event: MouseEvent) {
    if (this.smartAction.pressed) {
      if (this.smartAction.type === SmartActionTypeEnum.Toggle) {
        this.smartAction.actions[this.smartAction.smartValue]();
      } else if (this.smartAction.type !== SmartActionTypeEnum.Hold || this.smartAction.smartValue >= 0.95) {
        this.smartAction.actions[0]();
      }
      this.smartAction.pressed = false;
    }
  }

}
