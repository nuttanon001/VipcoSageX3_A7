import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskStatusMasterComponent } from './task-status-master.component';

describe('TaskStatusMasterComponent', () => {
  let component: TaskStatusMasterComponent;
  let fixture: ComponentFixture<TaskStatusMasterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TaskStatusMasterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TaskStatusMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
