import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TargetChooseModalComponent } from './target-choose-modal.component';

describe('TargetChooseModalComponent', () => {
  let component: TargetChooseModalComponent;
  let fixture: ComponentFixture<TargetChooseModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TargetChooseModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TargetChooseModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
