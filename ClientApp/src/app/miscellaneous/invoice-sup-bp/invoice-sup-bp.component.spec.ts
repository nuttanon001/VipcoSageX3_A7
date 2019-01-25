import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceSupBpComponent } from './invoice-sup-bp.component';

describe('InvoiceSupBpComponent', () => {
  let component: InvoiceSupBpComponent;
  let fixture: ComponentFixture<InvoiceSupBpComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InvoiceSupBpComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoiceSupBpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
