import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseOverHeaderTableComponent } from './purchase-over-header-table.component';

describe('PurchaseOverHeaderTableComponent', () => {
  let component: PurchaseOverHeaderTableComponent;
  let fixture: ComponentFixture<PurchaseOverHeaderTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseOverHeaderTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseOverHeaderTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
