import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IssusWorkgroupComponent } from './issus-workgroup.component';

describe('IssusWorkgroupComponent', () => {
  let component: IssusWorkgroupComponent;
  let fixture: ComponentFixture<IssusWorkgroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IssusWorkgroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IssusWorkgroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
