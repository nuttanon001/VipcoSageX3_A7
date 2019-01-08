import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MiscCenterComponent } from './misc-center.component';

describe('MiscCenterComponent', () => {
  let component: MiscCenterComponent;
  let fixture: ComponentFixture<MiscCenterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MiscCenterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MiscCenterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
