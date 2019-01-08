import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BomTableDialogComponent } from './bom-table-dialog.component';

describe('BomTableDialogComponent', () => {
  let component: BomTableDialogComponent;
  let fixture: ComponentFixture<BomTableDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BomTableDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BomTableDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
