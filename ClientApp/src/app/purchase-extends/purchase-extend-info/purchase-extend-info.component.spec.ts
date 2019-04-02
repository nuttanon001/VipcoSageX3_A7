import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseExtendInfoComponent } from './purchase-extend-info.component';

describe('PurchaseExtendInfoComponent', () => {
  let component: PurchaseExtendInfoComponent;
  let fixture: ComponentFixture<PurchaseExtendInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseExtendInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseExtendInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
