import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PoOutstandingComponent } from './po-outstanding.component';

describe('PoOutstandingComponent', () => {
  let component: PoOutstandingComponent;
  let fixture: ComponentFixture<PoOutstandingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PoOutstandingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PoOutstandingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
