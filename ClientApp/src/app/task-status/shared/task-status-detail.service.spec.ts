import { TestBed } from '@angular/core/testing';

import { TaskStatusDetailService } from './task-status-detail.service';

describe('TaskStatusDetailService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: TaskStatusDetailService = TestBed.get(TaskStatusDetailService);
    expect(service).toBeTruthy();
  });
});
