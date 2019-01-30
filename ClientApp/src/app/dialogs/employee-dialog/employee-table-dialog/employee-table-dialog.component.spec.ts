import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeTableDialogComponent } from './employee-table-dialog.component';

describe('EmployeeTableDialogComponent', () => {
  let component: EmployeeTableDialogComponent;
  let fixture: ComponentFixture<EmployeeTableDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EmployeeTableDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeTableDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
