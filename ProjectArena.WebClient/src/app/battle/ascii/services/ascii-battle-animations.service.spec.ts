import { TestBed } from '@angular/core/testing';

import { AsciiBattleAnimationsService } from './ascii-battle-animations.service';

describe('AsciiBattleAnimationsService', () => {
  let service: AsciiBattleAnimationsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AsciiBattleAnimationsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
