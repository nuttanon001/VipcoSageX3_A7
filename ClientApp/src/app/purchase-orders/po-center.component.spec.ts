import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PoCenterComponent } from './po-center.component';

describe('PoCenterComponent', () => {
  let component: PoCenterComponent;
  let fixture: ComponentFixture<PoCenterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PoCenterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PoCenterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
