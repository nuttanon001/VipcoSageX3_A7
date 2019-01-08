import { TestBed, inject } from '@angular/core/testing';

import { PurchaseOrderCommunicateService } from './purchase-order-communicate.service';

describe('PurchaseOrderCommunicateService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PurchaseOrderCommunicateService]
    });
  });

  it('should be created', inject([PurchaseOrderCommunicateService], (service: PurchaseOrderCommunicateService) => {
    expect(service).toBeTruthy();
  }));
});
