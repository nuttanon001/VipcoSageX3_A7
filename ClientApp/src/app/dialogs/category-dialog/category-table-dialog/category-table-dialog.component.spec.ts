import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoryTableDialogComponent } from './category-table-dialog.component';

describe('CategoryTableDialogComponent', () => {
  let component: CategoryTableDialogComponent;
  let fixture: ComponentFixture<CategoryTableDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CategoryTableDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CategoryTableDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
