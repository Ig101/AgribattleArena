import { TestBed } from '@angular/core/testing';

import { AssetsLoadingService } from './assets-loading.service';

describe('AssetsLoadingService', () => {
  let service: AssetsLoadingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AssetsLoadingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
