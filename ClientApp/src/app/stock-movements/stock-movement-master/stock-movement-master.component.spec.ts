import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockMovementMasterComponent } from './stock-movement-master.component';

describe('StockMovementMasterComponent', () => {
  let component: StockMovementMasterComponent;
  let fixture: ComponentFixture<StockMovementMasterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockMovementMasterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockMovementMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
