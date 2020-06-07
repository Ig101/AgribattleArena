import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalTabsComponentComponent } from './modal-tabs-component.component';

describe('ModalTabsComponentComponent', () => {
  let component: ModalTabsComponentComponent;
  let fixture: ComponentFixture<ModalTabsComponentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ModalTabsComponentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalTabsComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
