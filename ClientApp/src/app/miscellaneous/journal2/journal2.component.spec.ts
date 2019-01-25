import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { Journal2Component } from './journal2.component';

describe('Journal2Component', () => {
  let component: Journal2Component;
  let fixture: ComponentFixture<Journal2Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Journal2Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Journal2Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
