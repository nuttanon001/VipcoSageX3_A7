import { TestBed } from '@angular/core/testing';

import { PurchaseOverHeaderService } from './purchase-over-header.service';

describe('PurchaseOverHeaderService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PurchaseOverHeaderService = TestBed.get(PurchaseOverHeaderService);
    expect(service).toBeTruthy();
  });
});
