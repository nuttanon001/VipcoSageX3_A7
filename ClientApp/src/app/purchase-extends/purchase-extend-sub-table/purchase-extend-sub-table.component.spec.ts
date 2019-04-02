import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseExtendSubTableComponent } from './purchase-extend-sub-table.component';

describe('PurchaseExtendSubTableComponent', () => {
  let component: PurchaseExtendSubTableComponent;
  let fixture: ComponentFixture<PurchaseExtendSubTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseExtendSubTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseExtendSubTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
