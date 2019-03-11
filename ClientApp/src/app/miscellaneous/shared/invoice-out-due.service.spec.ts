import { TestBed } from '@angular/core/testing';

import { InvoiceOutDueService } from './invoice-out-due.service';

describe('InvoiceOutDueService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: InvoiceOutDueService = TestBed.get(InvoiceOutDueService);
    expect(service).toBeTruthy();
  });
});
