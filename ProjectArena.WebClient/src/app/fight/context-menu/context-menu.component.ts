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

@Component({
  selector: 'app-context-menu',
  templateUrl: './context-menu.component.html',
  styleUrls: ['./context-menu.component.scss']
})
export class ContextMenuComponent implements OnInit, OnDestroy, IModal<Action> {

  onClose = new Subject<Action>();
  onCancel = new Subject<Action>();

  @ViewChild('contextMenu', { static: true }) contextMenu: ElementRef<HTMLDivElement>;
  @ViewChild('overlay', { static: true }) overlayRef: ElementRef<HTMLDivElement>;

  get left() {
    return Math.max(125,
      Math.min(this.overlayRef.nativeElement.clientWidth - 487, this.data.positioning.left));
  }
  get top() {
    return Math.max(this.radius + this.data.positioning.textHeight,
      Math.min(this.overlayRef.nativeElement.clientHeight - (this.radius + this.data.positioning.textHeight), this.data.positioning.top));
  }

  get textHeight() {
    return this.data.positioning.textHeight;
  }

  pageSize = 8;

  targetName: string;
  items: ContextMenuItemPage[] = [];
  currentPage = 0;
  radius = 90;
  startAngle = Math.PI / this.pageSize + Math.PI / 2;

  inversion: boolean;

  preparedToExit: boolean[] = [];

  get maxPage() {
    return this.items.length;
  }

  get currentItems() {
    return this.items[this.currentPage].items;
  }

  constructor(
    @Inject(MODAL_DATA) private data: ContextMenuContext,
    private overlay: OverlayRef
  ) { }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event) {
    event.preventDefault();
  }

  close() {
    this.onClose.next(undefined);
    this.overlay.detach();
    this.overlay.dispose();
  }

  calculateShifts(position: number): {left: number, top: number, leftTooltip: boolean} {
    const angle = this.startAngle + position * Math.PI * 2 / this.pageSize;
    const cos = Math.cos(angle);
    return {
      left: this.radius * cos - 19,
      top: this.radius * Math.sin(angle) - 19,
      leftTooltip: (cos < 0 && this.left > 300) || this.left > this.overlayRef.nativeElement.clientWidth - 662
    };
  }

  ngOnInit() {
    this.items.length = 0;
    let counter = this.pageSize;
    let pageNumber = -1;
    let page: ContextMenuItemPage;
    for (const action of this.data.actions) {
      if (counter >= this.pageSize) {
        counter = 0;
        page = {
          items: []
        } as ContextMenuItemPage;
        this.items.push(page);
        pageNumber++;
        if (pageNumber > 0) {
          const previousShift = this.calculateShifts(counter);
          page.items.push({
            systemType: ContextMenuSystemTypesEnum.Previous,
            action: undefined,
            character: '<',
            name: 'Previous page',
            left: previousShift.left,
            top: previousShift.top,
            notAvailable: false,
            leftTooltip: previousShift.leftTooltip
          } as ContextMenuItem);
          counter++;

          const previousPage = this.items[pageNumber - 1];
          const moveItem = previousPage.items[this.pageSize - 1];
          previousPage.items[this.pageSize - 1] = {
            systemType: ContextMenuSystemTypesEnum.Next,
            action: undefined,
            character: '>',
            name: 'Next page',
            left: moveItem.left,
            top: moveItem.top,
            notAvailable: false,
            leftTooltip: moveItem.leftTooltip
          } as ContextMenuItem;

          const moveShift = this.calculateShifts(counter);
          moveItem.left = moveShift.left;
          moveItem.top = moveShift.top;
          moveItem.leftTooltip = moveShift.leftTooltip;
          page.items.push(moveItem);
          counter++;
        }
      }
      const shift = this.calculateShifts(counter);
      page.items.push({
        action: action.action,
        name: action.action.name,
        character: action.action.name[0].toUpperCase(),
        left: shift.left,
        top: shift.top,
        notAvailable: !!action.error,
        notAvailableReason: action.error,
        leftTooltip: shift.leftTooltip
      });
      counter++;
    }
    if (this.items.length === 0) {
      const first = {
        items: []
      } as ContextMenuItemPage;
      this.items.push(first);
      const shift = this.calculateShifts(0);
      first.items.push({
        systemType: ContextMenuSystemTypesEnum.Nothing,
        action: undefined,
        character: 'X',
        name: 'Nothing',
        left: shift.left,
        top: shift.top,
        notAvailable: true,
        leftTooltip: shift.leftTooltip
      } as ContextMenuItem);
    }
  }

  ngOnDestroy(): void {
  }

  getItemLeft(item: ContextMenuItem) {
    return Math.round(this.left + item.left);
  }

  getItemTop(item: ContextMenuItem) {
    return Math.round(this.top + item.top);
  }

  getTargetLeft() {
    return this.left - 125;
  }

  getTargetTop() {
    return Math.round(this.top - this.radius - this.data.positioning.textHeight * 2);
  }

  prepareExit(event: MouseEvent) {
    if (event.target !== event.currentTarget) {
      return;
    }
    this.preparedToExit[event.button] = true;
  }

  onExit(event: MouseEvent) {
    if (event.target !== event.currentTarget || !this.preparedToExit[event.button]) {
      return;
    }
    this.preparedToExit[event.button] = undefined;
    this.close();
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
    this.onClose.next(event.action);
    this.overlay.detach();
    this.overlay.dispose();
  }

}
