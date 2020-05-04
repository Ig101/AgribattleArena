import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TextInputComponent } from './components/text-input/text-input.component';
import { ButtonComponent } from './components/button/button.component';
import { LinkComponent } from './components/link/link.component';
import { FocusRemoverDirective } from './components/directives/focus-remover.directive';

@NgModule({
  declarations: [TextInputComponent, ButtonComponent, LinkComponent, FocusRemoverDirective],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
  ],
  exports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    TextInputComponent,
    ButtonComponent,
    LinkComponent,
    FocusRemoverDirective
  ]
})
export class SharedModule { }
