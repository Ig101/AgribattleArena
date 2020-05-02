import { Component, OnInit, Input } from '@angular/core';
import { ActionPointStatusEnum } from '../../models/enum/action-point-status.enum';
import { Color } from 'src/app/shared/models/color.model';

@Component({
  selector: 'app-action-points-block',
  templateUrl: './action-points-block.component.html',
  styleUrls: ['./action-points-block.component.scss']
})
export class ActionPointsBlockComponent {

  @Input() set actionPointsMaxCount(value: number) {
    this.actionPointsMaxCountInternal = value;
    this.recalculateActionPointsArray();
  }
  @Input() set currentActionPointsCount(value: number) {
    this.currentActionPointsCountInternal = value;
    this.recalculateActionPointsArray();
  }
  @Input() set actionPointsAfterSpendCount(value: number) {
    this.actionPointsAfterSpendCountInternal = value;
    this.recalculateActionPointsArray();
  }
  @Input() set actorColor(value: Color) {
    this.actorColorInternal = value;
    this.recalculateActionPointsArray();
  }

  private actionPointsMaxCountInternal = 0;
  private currentActionPointsCountInternal = 0;
  private actionPointsAfterSpendCountInternal = 0;
  private actorColorInternal: Color;

  actionPoints: { color: string }[] = [];
  actionPointsStatus = ActionPointStatusEnum;

  recalculateActionPointsArray() {
    this.actionPoints.length = this.actionPointsMaxCountInternal;
    for (let i = 0; i < this.actionPointsMaxCountInternal; i++) {
      if (!this.actionPoints[i]) {
        this.actionPoints[i] = { color: '#000414' };
      }
      const color = this.actorColorInternal || { r: 0, g: 0, b: 0, a: 0 };
      if (i < this.actionPointsAfterSpendCountInternal) {
        this.actionPoints[i].color = `rgba(${color.r},${color.g},${color.b},${color.a})`;
      } else if (i < this.currentActionPointsCountInternal) {
        this.actionPoints[i].color = `rgba(${color.r * 0.7},${color.g * 0.7},${color.b * 0.7},${color.a * 0.7})`;
      } else {
        this.actionPoints[i].color = '#000414';
      }
    }
    this.actionPoints.reverse();
  }

  trackByFn(index, item) {
    return item.state;
  }

}
