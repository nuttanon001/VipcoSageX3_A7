import { TestBed, inject } from '@angular/core/testing';

import { ProjectCodeService } from './project-code.service';

describe('ProjectCodeService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ProjectCodeService]
    });
  });

  it('should be created', inject([ProjectCodeService], (service: ProjectCodeService) => {
    expect(service).toBeTruthy();
  }));
});
