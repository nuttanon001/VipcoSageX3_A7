import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BranchTableDialogComponent } from './branch-table-dialog.component';

describe('BranchTableDialogComponent', () => {
  let component: BranchTableDialogComponent;
  let fixture: ComponentFixture<BranchTableDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BranchTableDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BranchTableDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
