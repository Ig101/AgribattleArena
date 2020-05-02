import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ActionPointsBlockComponent } from './action-points-block.component';

describe('ActionPointsBlockComponent', () => {
  let component: ActionPointsBlockComponent;
  let fixture: ComponentFixture<ActionPointsBlockComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ActionPointsBlockComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ActionPointsBlockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
