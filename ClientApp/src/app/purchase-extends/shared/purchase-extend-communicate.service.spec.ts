import { TestBed } from '@angular/core/testing';

import { PurchaseExtendCommunicateService } from './purchase-extend-communicate.service';

describe('PurchaseExtendCommunicateService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PurchaseExtendCommunicateService = TestBed.get(PurchaseExtendCommunicateService);
    expect(service).toBeTruthy();
  });
});
