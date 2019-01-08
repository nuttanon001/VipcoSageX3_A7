import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockOnhandCenterComponent } from './stock-onhand-center.component';

describe('StockOnhandCenterComponent', () => {
  let component: StockOnhandCenterComponent;
  let fixture: ComponentFixture<StockOnhandCenterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockOnhandCenterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockOnhandCenterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
