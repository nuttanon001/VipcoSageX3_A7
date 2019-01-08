import { TestBed, inject } from '@angular/core/testing';

import { StockOnhandService } from './stock-onhand.service';

describe('StockOnhandService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [StockOnhandService]
    });
  });

  it('should be created', inject([StockOnhandService], (service: StockOnhandService) => {
    expect(service).toBeTruthy();
  }));
});
