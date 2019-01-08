import { TestBed, inject } from '@angular/core/testing';

import { EmployeeGroupService } from './employee-group.service';

describe('EmployeeGroupService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EmployeeGroupService]
    });
  });

  it('should be created', inject([EmployeeGroupService], (service: EmployeeGroupService) => {
    expect(service).toBeTruthy();
  }));
});
