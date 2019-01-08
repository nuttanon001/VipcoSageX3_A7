import { TestBed, inject } from '@angular/core/testing';

import { EmployeeCommuncateService } from './employee-communcate.service';

describe('EmployeeCommuncateService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EmployeeCommuncateService]
    });
  });

  it('should be created', inject([EmployeeCommuncateService], (service: EmployeeCommuncateService) => {
    expect(service).toBeTruthy();
  }));
});
