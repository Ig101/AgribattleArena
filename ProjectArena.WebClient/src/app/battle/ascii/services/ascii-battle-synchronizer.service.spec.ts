import { TestBed } from '@angular/core/testing';

import { AsciiBattleSynchronizerService } from './ascii-battle-synchronizer.service';

describe('AsciiBattleSynchronizerService', () => {
  let service: AsciiBattleSynchronizerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AsciiBattleSynchronizerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
