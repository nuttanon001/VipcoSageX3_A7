import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseOverHeaderInfoComponent } from './purchase-over-header-info.component';

describe('PurchaseOverHeaderInfoComponent', () => {
  let component: PurchaseOverHeaderInfoComponent;
  let fixture: ComponentFixture<PurchaseOverHeaderInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseOverHeaderInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseOverHeaderInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
