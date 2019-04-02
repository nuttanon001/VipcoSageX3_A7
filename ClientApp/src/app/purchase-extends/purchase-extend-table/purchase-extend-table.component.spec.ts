import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseExtendTableComponent } from './purchase-extend-table.component';

describe('PurchaseExtendTableComponent', () => {
  let component: PurchaseExtendTableComponent;
  let fixture: ComponentFixture<PurchaseExtendTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseExtendTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseExtendTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
