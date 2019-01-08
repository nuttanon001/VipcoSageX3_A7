import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockLocaltionTableComponent } from './stock-localtion-table.component';

describe('StockLocaltionTableComponent', () => {
  let component: StockLocaltionTableComponent;
  let fixture: ComponentFixture<StockLocaltionTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockLocaltionTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockLocaltionTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
