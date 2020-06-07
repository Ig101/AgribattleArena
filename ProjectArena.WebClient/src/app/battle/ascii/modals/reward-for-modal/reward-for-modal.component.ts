import { Component, OnInit, HostBinding, Host } from '@angular/core';
import { AsciiBattleStorageService } from '../../services/ascii-battle-storage.service';

@Component({
  selector: 'app-reward-for-modal',
  templateUrl: './reward-for-modal.component.html',
  styleUrls: ['./reward-for-modal.component.scss']
})
export class RewardForModalComponent {

  get reward() {
    return this.battleStorageService.reward;
  }

  get experience() {
    return this.battleStorageService.reward?.experience;
  }

  constructor(
    private battleStorageService: AsciiBattleStorageService
  ) { }
}
