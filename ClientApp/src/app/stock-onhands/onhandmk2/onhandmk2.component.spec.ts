import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { Onhandmk2Component } from './onhandmk2.component';

describe('Onhandmk2Component', () => {
  let component: Onhandmk2Component;
  let fixture: ComponentFixture<Onhandmk2Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Onhandmk2Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Onhandmk2Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
