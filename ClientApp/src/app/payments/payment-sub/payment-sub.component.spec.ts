import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentSubComponent } from './payment-sub.component';

describe('PaymentSubComponent', () => {
  let component: PaymentSubComponent;
  let fixture: ComponentFixture<PaymentSubComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PaymentSubComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentSubComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
