import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerTableDialogComponent } from './customer-table-dialog.component';

describe('CustomerTableDialogComponent', () => {
  let component: CustomerTableDialogComponent;
  let fixture: ComponentFixture<CustomerTableDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerTableDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerTableDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
