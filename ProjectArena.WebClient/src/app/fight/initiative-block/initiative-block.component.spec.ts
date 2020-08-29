import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InitiativeBlockComponent } from './initiative-block.component';

describe('InitiativeBlockComponent', () => {
  let component: InitiativeBlockComponent;
  let fixture: ComponentFixture<InitiativeBlockComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InitiativeBlockComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InitiativeBlockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
