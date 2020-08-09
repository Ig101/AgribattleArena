import { Component, OnInit } from '@angular/core';
import { LoadingService } from '../shared/services/loading.service';
import { SceneService } from '../engine/services/scene.service';
import { SynchronizationService } from '../engine/services/synchronization.service';

@Component({
  selector: 'app-fight',
  templateUrl: './fight.component.html',
  styleUrls: ['./fight.component.scss']
})
export class FightComponent implements OnInit {

  constructor(
    private loadingService: LoadingService,
    private sceneService: SceneService
  ) { }

  ngOnInit(): void {
    this.loadingService.finishLoading()
            .subscribe();
  }

}
