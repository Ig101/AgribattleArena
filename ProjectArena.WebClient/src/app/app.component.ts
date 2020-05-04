import { Component } from '@angular/core';
import { LoadingService } from './shared/services/loading.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Blue Plague';

  get loadingScreenOn() {
    return this.loadingService.on;
  }

  constructor(
    private loadingService: LoadingService
  ) { }
}
