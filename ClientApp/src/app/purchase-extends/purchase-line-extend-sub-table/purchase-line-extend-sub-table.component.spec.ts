import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseLineExtendSubTableComponent } from './purchase-line-extend-sub-table.component';

describe('PurchaseLineExtendSubTableComponent', () => {
  let component: PurchaseLineExtendSubTableComponent;
  let fixture: ComponentFixture<PurchaseLineExtendSubTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseLineExtendSubTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseLineExtendSubTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
