import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrMasterComponent } from './pr-master.component';

describe('PrMasterComponent', () => {
  let component: PrMasterComponent;
  let fixture: ComponentFixture<PrMasterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrMasterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
