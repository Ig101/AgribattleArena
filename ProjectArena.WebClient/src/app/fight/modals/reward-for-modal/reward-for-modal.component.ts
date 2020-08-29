import { Component, OnInit, HostBinding, Host } from '@angular/core';
import { SceneService } from 'src/app/engine/services/scene.service';

@Component({
  selector: 'app-reward-for-modal',
  templateUrl: './reward-for-modal.component.html',
  styleUrls: ['./reward-for-modal.component.scss']
})
export class RewardForModalComponent {

  get reward() {
    return this.sceneService.scene?.reward;
  }

  get experience() {
    return this.sceneService.scene?.reward?.experience;
  }

  constructor(
    private sceneService: SceneService
  ) { }
}
