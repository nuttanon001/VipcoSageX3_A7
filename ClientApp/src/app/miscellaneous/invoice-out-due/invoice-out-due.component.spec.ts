import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceOutDueComponent } from './invoice-out-due.component';

describe('InvoiceOutDueComponent', () => {
  let component: InvoiceOutDueComponent;
  let fixture: ComponentFixture<InvoiceOutDueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InvoiceOutDueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoiceOutDueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
