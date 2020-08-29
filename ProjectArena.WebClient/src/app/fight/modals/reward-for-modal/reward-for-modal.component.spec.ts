import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RewardForModalComponent } from './reward-for-modal.component';

describe('RewardForModalComponent', () => {
  let component: RewardForModalComponent;
  let fixture: ComponentFixture<RewardForModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RewardForModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RewardForModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
