import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DecorationModalComponent } from './decoration-modal.component';

describe('DecorationModalComponent', () => {
  let component: DecorationModalComponent;
  let fixture: ComponentFixture<DecorationModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DecorationModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DecorationModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
