import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TextureIconComponent } from './texture-icon.component';

describe('TextureIconComponent', () => {
  let component: TextureIconComponent;
  let fixture: ComponentFixture<TextureIconComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TextureIconComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TextureIconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
