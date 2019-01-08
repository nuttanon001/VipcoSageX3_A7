import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PoTableComponent } from './po-table.component';

describe('PoTableComponent', () => {
  let component: PoTableComponent;
  let fixture: ComponentFixture<PoTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PoTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PoTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
