import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PoSubReportComponent } from './po-sub-report.component';

describe('PoSubReportComponent', () => {
  let component: PoSubReportComponent;
  let fixture: ComponentFixture<PoSubReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PoSubReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PoSubReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
