import { TestBed } from '@angular/core/testing';

import { PurchaseLineExtendService } from './purchase-line-extend.service';

describe('PurchaseLineExtendService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PurchaseLineExtendService = TestBed.get(PurchaseLineExtendService);
    expect(service).toBeTruthy();
  });
});
