import { TestBed, inject } from '@angular/core/testing';

import { BomLevelService } from './bom-level.service';

describe('BomLevelService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [BomLevelService]
    });
  });

  it('should be created', inject([BomLevelService], (service: BomLevelService) => {
    expect(service).toBeTruthy();
  }));
});
