import { TestBed } from '@angular/core/testing';

import { PurchaseExtendService } from './purchase-extend.service';

describe('PurchaseExtendService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PurchaseExtendService = TestBed.get(PurchaseExtendService);
    expect(service).toBeTruthy();
  });
});
