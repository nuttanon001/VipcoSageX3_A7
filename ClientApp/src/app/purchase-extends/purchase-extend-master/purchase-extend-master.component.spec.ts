import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseExtendMasterComponent } from './purchase-extend-master.component';

describe('PurchaseExtendMasterComponent', () => {
  let component: PurchaseExtendMasterComponent;
  let fixture: ComponentFixture<PurchaseExtendMasterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseExtendMasterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseExtendMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
