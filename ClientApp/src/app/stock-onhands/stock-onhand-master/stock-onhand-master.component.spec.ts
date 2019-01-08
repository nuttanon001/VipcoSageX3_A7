import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockOnhandMasterComponent } from './stock-onhand-master.component';

describe('StockOnhandMasterComponent', () => {
  let component: StockOnhandMasterComponent;
  let fixture: ComponentFixture<StockOnhandMasterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockOnhandMasterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockOnhandMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
