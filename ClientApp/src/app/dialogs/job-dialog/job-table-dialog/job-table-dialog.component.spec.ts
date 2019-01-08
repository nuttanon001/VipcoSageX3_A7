import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { JobTableDialogComponent } from './job-table-dialog.component';

describe('JobTableDialogComponent', () => {
  let component: JobTableDialogComponent;
  let fixture: ComponentFixture<JobTableDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ JobTableDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JobTableDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
