import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskStatusDetailTableComponent } from './task-status-detail-table.component';

describe('TaskStatusDetailTableComponent', () => {
  let component: TaskStatusDetailTableComponent;
  let fixture: ComponentFixture<TaskStatusDetailTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TaskStatusDetailTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TaskStatusDetailTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
