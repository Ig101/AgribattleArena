
import { TestBed } from '@angular/core/testing';
import { ArenaHubService } from './arena-hub.service';

describe('ArenaHubService', () => {
  let service: ArenaHubService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ArenaHubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
