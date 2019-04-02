import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseRequestTableDialogComponent } from './purchase-request-table-dialog.component';

describe('PurchaseRequestTableDialogComponent', () => {
  let component: PurchaseRequestTableDialogComponent;
  let fixture: ComponentFixture<PurchaseRequestTableDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PurchaseRequestTableDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurchaseRequestTableDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
