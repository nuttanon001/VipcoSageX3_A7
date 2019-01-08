import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BankDialogComponent } from './bank-dialog.component';

describe('BankDialogComponent', () => {
  let component: BankDialogComponent;
  let fixture: ComponentFixture<BankDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BankDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BankDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
