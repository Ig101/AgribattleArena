import { AnimationDeclaration } from './animations/animation-declaration.model';
import { AnimationFrame } from './animations/animation-frame.model';

export interface ActionAnimation {
  generateDeclarations: (x: number, y: number, animation: ActionAnimation) => AnimationFrame[];
}
