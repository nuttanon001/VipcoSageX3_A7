import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskStatusInfoComponent } from './task-status-info.component';

describe('TaskStatusInfoComponent', () => {
  let component: TaskStatusInfoComponent;
  let fixture: ComponentFixture<TaskStatusInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TaskStatusInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TaskStatusInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
