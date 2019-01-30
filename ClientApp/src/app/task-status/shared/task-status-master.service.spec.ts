import { TestBed } from '@angular/core/testing';

import { TaskStatusMasterService } from './task-status-master.service';

describe('TaskStatusMasterService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: TaskStatusMasterService = TestBed.get(TaskStatusMasterService);
    expect(service).toBeTruthy();
  });
});
