import { TestBed } from '@angular/core/testing';

import { BattleResolverService } from './battle-resolver.service';

describe('BattleResolverService', () => {
  let service: BattleResolverService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BattleResolverService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
