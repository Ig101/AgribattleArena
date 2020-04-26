import { TestBed } from '@angular/core/testing';

import { LobbyResolverService } from './lobby-resolver.service';

describe('LobbyResolverService', () => {
  let service: LobbyResolverService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LobbyResolverService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
