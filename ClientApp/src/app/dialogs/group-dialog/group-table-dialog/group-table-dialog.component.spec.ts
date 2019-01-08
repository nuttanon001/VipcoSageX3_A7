import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupTableDialogComponent } from './group-table-dialog.component';

describe('GroupTableDialogComponent', () => {
  let component: GroupTableDialogComponent;
  let fixture: ComponentFixture<GroupTableDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GroupTableDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupTableDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
