import { Component, OnInit, Input, ViewChild, ElementRef, Output,
  EventEmitter, ChangeDetectionStrategy, OnDestroy, Inject, HostListener } from '@angular/core';
import { ContextMenuContext } from '../models/context-menu-context.model';
import { ContextMenuItemPage } from '../models/context-menu-item-page.model';
import { ContextMenuSystemTypesEnum } from '../models/enums/context-menu-system-types.enum';
import { ContextMenuItem } from '../models/context-menu-item.model';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { Action } from 'src/app/engine/models/abstract/action.model';
import { Observable, Subject } from 'rxjs';
import { MODAL_DATA } from 'src/app/shared/services/modal.service';
import { OverlayRef } from '@angular/cdk/overlay';
import { CharsService } from 'src/app/shared/services/chars.service';
import { ActionClassEnum } from 'src/app/engine/models/enums/action-class.enum';

@Component({
  selector: 'app-context-menu',
  templateUrl: './context-menu.component.html',
  styleUrls: ['./context-menu.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ContextMenuComponent implements OnInit, OnDestroy {

  @ViewChild('overlay', { static: true }) overlay: ElementRef<HTMLDivElement>;

  @Output() act = new EventEmitter<Action>();

  @Input() set actions(actions: Action[]) {
    this.actionsInternal = actions
      .filter(x => x.native.actionClass === ActionClassEnum.Default)
      .sort((a, b) => b.native.aiPriority - a.native.aiPriority);
    this.updateActions();
  }

  @Input() currentAction: Action;

  @Input() conflict: boolean;

  private actionsInternal: Action[];

  get pageSize() {
    return Math.floor((this.overlay.nativeElement.clientHeight - 20) / 70);
  }

  fixedPageSize: number;

  items: ContextMenuItemPage[] = [];
  currentPage = 0;

  get maxPage() {
    return this.items.length;
  }

  get currentItems() {
    return this.items[this.currentPage].items;
  }

  constructor() { }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event) {
    event.preventDefault();
  }

  ngOnInit() {
  }

  ngOnDestroy(): void {
  }

  updateActions() {
    this.items.length = 0;
    let counter = this.pageSize;
    let pageNumber = -1;
    let page: ContextMenuItemPage;
    this.fixedPageSize = this.pageSize;
    for (const action of this.actionsInternal) {
      if (counter >= this.pageSize) {
        counter = 0;
        page = {
          items: []
        } as ContextMenuItemPage;
        this.items.push(page);
        pageNumber++;
        if (pageNumber > 0) {
          page.items.push({
            systemType: ContextMenuSystemTypesEnum.Previous,
            action: undefined,
            character: '▲',
            name: 'Previous',
            notAvailable: false
          } as ContextMenuItem);
          counter++;

          const previousPage = this.items[pageNumber - 1];
          const moveItem = previousPage.items[this.pageSize - 1];
          previousPage.items[this.pageSize - 1] = {
            systemType: ContextMenuSystemTypesEnum.Next,
            action: undefined,
            character: '▼',
            name: 'Next',
            notAvailable: false
          } as ContextMenuItem;

          page.items.push(moveItem);
          counter++;
        }
      }
      page.items.push({
        action,
        name: action.native.name,
        character: action.native.char,
        notAvailable: false,
        notAvailableReason: undefined
      });
      counter++;
    }
    if (this.items.length === 0) {
      const first = {
        items: []
      } as ContextMenuItemPage;
      this.items.push(first);
    }
  }

  onResize() {
    if (this.pageSize !== this.fixedPageSize) {
      this.updateActions();
    }
  }

  onClick(event: ContextMenuItem) {
    if (event.systemType === ContextMenuSystemTypesEnum.Next) {
      this.currentPage++;
      return;
    }
    if (event.systemType === ContextMenuSystemTypesEnum.Previous) {
      this.currentPage--;
      return;
    }
    if (event.notAvailable) {
      return;
    }
    this.act.next(event.action);
  }
}
