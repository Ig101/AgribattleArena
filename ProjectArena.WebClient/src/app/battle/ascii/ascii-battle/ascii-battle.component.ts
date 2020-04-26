import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-ascii-battle',
  templateUrl: './ascii-battle.component.html',
  styleUrls: ['./ascii-battle.component.scss']
})
export class AsciiBattleComponent implements OnInit {

  constructor(
    activatedRoute: ActivatedRoute
  ) {
    console.log(activatedRoute.snapshot.data.battle);
  }

  ngOnInit(): void {
  }

}
