import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseExtendCenterComponent } from './purchase-extend-center.component';

describe('PurchaseExtendCenterComponent', () => {
  let component: PurchaseExtendCenterComponent;
  let fixture: ComponentFixture<PurchaseExtendCenterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseExtendCenterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseExtendCenterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
