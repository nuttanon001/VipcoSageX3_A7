import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BankTableDialogComponent } from './bank-table-dialog.component';

describe('BankTableDialogComponent', () => {
  let component: BankTableDialogComponent;
  let fixture: ComponentFixture<BankTableDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BankTableDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BankTableDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
