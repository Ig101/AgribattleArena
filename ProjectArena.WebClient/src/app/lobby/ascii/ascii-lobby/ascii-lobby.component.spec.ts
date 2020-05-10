import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AsciiLobbyComponent } from './ascii-lobby.component';

describe('AsciiLobbyComponent', () => {
  let component: AsciiLobbyComponent;
  let fixture: ComponentFixture<AsciiLobbyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AsciiLobbyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AsciiLobbyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
