import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DatePickDialogComponent } from './date-pick-dialog.component';

describe('DatePickDialogComponent', () => {
  let component: DatePickDialogComponent;
  let fixture: ComponentFixture<DatePickDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DatePickDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DatePickDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
