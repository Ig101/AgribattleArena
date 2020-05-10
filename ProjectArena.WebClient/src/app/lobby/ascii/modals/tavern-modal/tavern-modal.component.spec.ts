import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TavernModalComponent } from './tavern-modal.component';

describe('TavernModalComponent', () => {
  let component: TavernModalComponent;
  let fixture: ComponentFixture<TavernModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TavernModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TavernModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
