import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrCenterComponent } from './pr-center.component';

describe('PrCenterComponent', () => {
  let component: PrCenterComponent;
  let fixture: ComponentFixture<PrCenterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrCenterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrCenterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
