import { TestBed, inject } from '@angular/core/testing';

import { StockMovementsService } from './stock-movements.service';

describe('StockMovementsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [StockMovementsService]
    });
  });

  it('should be created', inject([StockMovementsService], (service: StockMovementsService) => {
    expect(service).toBeTruthy();
  }));
});
