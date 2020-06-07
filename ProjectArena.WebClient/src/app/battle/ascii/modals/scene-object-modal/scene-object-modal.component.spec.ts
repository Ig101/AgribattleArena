import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SceneObjectModalComponent } from './scene-object-modal.component';

describe('SceneObjectModalComponent', () => {
  let component: SceneObjectModalComponent;
  let fixture: ComponentFixture<SceneObjectModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SceneObjectModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SceneObjectModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
