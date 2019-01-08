import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PoMasterComponent } from './po-master.component';

describe('PoMasterComponent', () => {
  let component: PoMasterComponent;
  let fixture: ComponentFixture<PoMasterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PoMasterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PoMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
