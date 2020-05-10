import { Component, OnInit, OnDestroy, Inject, HostListener } from '@angular/core';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { Subject } from 'rxjs';
import { MODAL_DATA } from 'src/app/shared/services/modal.service';
import { Actor } from '../../models/scene/actor.model';
import { OverlayRef } from '@angular/cdk/overlay';
import { Skill } from '../../models/scene/skill.model';

@Component({
  selector: 'app-skill-modal',
  templateUrl: './skill-modal.component.html',
  styleUrls: ['./skill-modal.component.scss']
})
export class SkillModalComponent implements IModal<any>, OnDestroy {

  onClose = new Subject<any>();
  onCancel = new Subject<any>();

  skillName: string;
  skillDescription: string;
  skillCd: number;
  skillCost: number;
  skillRange: number;

  constructor(
    @Inject(MODAL_DATA) data: Skill,
    private overlay: OverlayRef
  ) {
    this.skillName = data.name;
    this.skillDescription = data.description;
    this.skillCd = data.cd;
    this.skillCost = data.cost;
    this.skillRange = data.range;
  }

  ngOnDestroy() {
    this.onClose.unsubscribe();
    this.onCancel.unsubscribe();
  }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event) {
    event.preventDefault();
  }

  @HostListener('mouseup', ['$event'])
  onClick(event) {
    if (event.button === 0) {
      event.stopPropagation();
    }
    this.close();
  }

  @HostListener('keypress', ['$event'])
  onKey(event) {
    this.close();
  }

  close() {
    this.onClose.next();
    this.overlay.detach();
    this.overlay.dispose();
  }
}
