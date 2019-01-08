import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockMovementCenterComponent } from './stock-movement-center.component';

describe('StockMovementCenterComponent', () => {
  let component: StockMovementCenterComponent;
  let fixture: ComponentFixture<StockMovementCenterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockMovementCenterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockMovementCenterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
