import { TestBed } from '@angular/core/testing';

import { AsciiBattlePathCreatorService } from './ascii-battle-path-creator.service';

describe('AsciiBattlePathCreatorService', () => {
  let service: AsciiBattlePathCreatorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AsciiBattlePathCreatorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
