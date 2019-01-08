import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MiscMasterComponent } from './misc-master.component';

describe('MiscMasterComponent', () => {
  let component: MiscMasterComponent;
  let fixture: ComponentFixture<MiscMasterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MiscMasterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MiscMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
