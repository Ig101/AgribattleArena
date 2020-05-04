import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { LoadingService } from 'src/app/shared/services/loading.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.scss']
})
export class LobbyComponent implements OnDestroy, AfterViewInit {

  finishLoadingSubscription: Subscription;

  constructor(
    private loadingService: LoadingService
  ) { }

  ngOnDestroy(): void {
    if (this.finishLoadingSubscription) {
      this.finishLoadingSubscription.unsubscribe();
    }
  }

  ngAfterViewInit(): void {
    this.finishLoadingSubscription = this.loadingService.finishLoading()
      .subscribe(() => {});
  }
}
