import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskStatusCenterComponent } from './task-status-center.component';

describe('TaskStatusCenterComponent', () => {
  let component: TaskStatusCenterComponent;
  let fixture: ComponentFixture<TaskStatusCenterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TaskStatusCenterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TaskStatusCenterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
