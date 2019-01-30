import { TestBed } from '@angular/core/testing';

import { TaskStatusMasterCommunicateService } from './task-status-master-communicate.service';

describe('TaskStatusMasterCommunicateService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: TaskStatusMasterCommunicateService = TestBed.get(TaskStatusMasterCommunicateService);
    expect(service).toBeTruthy();
  });
});
