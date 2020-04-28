import { TestBed } from '@angular/core/testing';

import { AsciiBattleStorageService } from './ascii-battle-storage.service';

describe('AsciiBattleStorageService', () => {
  let service: AsciiBattleStorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AsciiBattleStorageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
