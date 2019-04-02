import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerTableDialogComponent } from './partner-table-dialog.component';

describe('PartnerTableDialogComponent', () => {
  let component: PartnerTableDialogComponent;
  let fixture: ComponentFixture<PartnerTableDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerTableDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerTableDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
