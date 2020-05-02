import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HotkeyedButtonComponent } from './hotkeyed-button.component';

describe('HotkeyedButtonComponent', () => {
  let component: HotkeyedButtonComponent;
  let fixture: ComponentFixture<HotkeyedButtonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HotkeyedButtonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HotkeyedButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
