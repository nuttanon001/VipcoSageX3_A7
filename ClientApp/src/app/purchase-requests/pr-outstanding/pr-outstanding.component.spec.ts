import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrOutstandingComponent } from './pr-outstanding.component';

describe('PrOutstandingComponent', () => {
  let component: PrOutstandingComponent;
  let fixture: ComponentFixture<PrOutstandingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrOutstandingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrOutstandingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
