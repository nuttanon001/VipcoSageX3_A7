import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockMovement2TableComponent } from './stock-movement2-table.component';

describe('StockMovement2TableComponent', () => {
  let component: StockMovement2TableComponent;
  let fixture: ComponentFixture<StockMovement2TableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockMovement2TableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockMovement2TableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
