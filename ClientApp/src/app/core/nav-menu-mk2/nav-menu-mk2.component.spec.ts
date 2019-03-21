import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NavMenuMk2Component } from './nav-menu-mk2.component';

describe('NavMenuMk2Component', () => {
  let component: NavMenuMk2Component;
  let fixture: ComponentFixture<NavMenuMk2Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NavMenuMk2Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NavMenuMk2Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
