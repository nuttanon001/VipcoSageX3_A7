import { TestBed } from '@angular/core/testing';

import { PurchaseOverHeaderCommunicateService } from './purchase-over-header-communicate.service';

describe('PurchaseOverHeaderCommunicateService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PurchaseOverHeaderCommunicateService = TestBed.get(PurchaseOverHeaderCommunicateService);
    expect(service).toBeTruthy();
  });
});
