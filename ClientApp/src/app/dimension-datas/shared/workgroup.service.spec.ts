import { TestBed, inject } from '@angular/core/testing';

import { WorkgroupService } from './workgroup.service';

describe('WorkgroupService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [WorkgroupService]
    });
  });

  it('should be created', inject([WorkgroupService], (service: WorkgroupService) => {
    expect(service).toBeTruthy();
  }));
});
