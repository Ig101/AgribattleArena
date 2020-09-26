import { TestBed } from '@angular/core/testing';

import { FightResolverService } from './fight-resolver.service';

describe('FightResolverService', () => {
  let service: FightResolverService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FightResolverService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
