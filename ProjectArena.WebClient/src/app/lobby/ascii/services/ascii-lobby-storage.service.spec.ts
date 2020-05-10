import { TestBed } from '@angular/core/testing';

import { AsciiLobbyStorageService } from './ascii-lobby-storage.service';

describe('AsciiLobbyStorageService', () => {
  let service: AsciiLobbyStorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AsciiLobbyStorageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
