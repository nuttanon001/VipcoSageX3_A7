import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseOverHeaderMasterComponent } from './purchase-over-header-master.component';

describe('PurchaseOverHeaderMasterComponent', () => {
  let component: PurchaseOverHeaderMasterComponent;
  let fixture: ComponentFixture<PurchaseOverHeaderMasterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseOverHeaderMasterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseOverHeaderMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
