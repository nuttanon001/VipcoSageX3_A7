import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseRequestDialogComponent } from './purchase-request-dialog.component';

describe('PurchaseRequestDialogComponent', () => {
  let component: PurchaseRequestDialogComponent;
  let fixture: ComponentFixture<PurchaseRequestDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PurchaseRequestDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseRequestDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
