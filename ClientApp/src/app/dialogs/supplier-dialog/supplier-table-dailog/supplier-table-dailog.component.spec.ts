import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierTableDailogComponent } from './supplier-table-dailog.component';

describe('SupplierTableDailogComponent', () => {
  let component: SupplierTableDailogComponent;
  let fixture: ComponentFixture<SupplierTableDailogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SupplierTableDailogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SupplierTableDailogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
