import { Component, OnInit, Input, OnDestroy } from '@angular/core';
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

}
