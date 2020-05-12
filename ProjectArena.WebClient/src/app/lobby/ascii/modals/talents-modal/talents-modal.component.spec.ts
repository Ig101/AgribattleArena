import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TalentsModalComponent } from './talents-modal.component';

describe('TalentsModalComponent', () => {
  let component: TalentsModalComponent;
  let fixture: ComponentFixture<TalentsModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TalentsModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TalentsModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
