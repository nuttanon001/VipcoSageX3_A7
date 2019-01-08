import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PoInfoComponent } from './po-info.component';

describe('PoInfoComponent', () => {
  let component: PoInfoComponent;
  let fixture: ComponentFixture<PoInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PoInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PoInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
