import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentCenterComponent } from './payment-center.component';

describe('PaymentCenterComponent', () => {
  let component: PaymentCenterComponent;
  let fixture: ComponentFixture<PaymentCenterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PaymentCenterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentCenterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
