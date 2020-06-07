import { Component, OnInit, Input, ViewChild, ElementRef, AfterViewInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { ButtonComponent } from 'src/app/shared/components/button/button.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-modal-loading',
  templateUrl: './modal-loading.component.html',
  styleUrls: ['./modal-loading.component.scss']
})
export class ModalLoadingComponent implements OnDestroy {

  @Input() set errors(value: string[]) {
    this.errorsInternal = value;
    if (value) {
      setTimeout(() => { this.loadingOk.button.nativeElement.focus(); });
    }
  }

  @Output() finishLoading = new EventEmitter<any>();

  errorsInternal: string[];

  @ViewChild('loadingOk') loadingOk: ButtonComponent;

  constructor() { }

  ngOnDestroy(): void {
    this.finishLoading.unsubscribe();
  }


  loadingEnd() {
    this.finishLoading.emit();
  }
}
