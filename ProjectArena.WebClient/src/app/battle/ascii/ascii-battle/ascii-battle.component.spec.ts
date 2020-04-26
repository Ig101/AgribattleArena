import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AsciiBattleComponent } from './ascii-battle.component';

describe('AsciiBattleComponent', () => {
  let component: AsciiBattleComponent;
  let fixture: ComponentFixture<AsciiBattleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AsciiBattleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AsciiBattleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
