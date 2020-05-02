import { AnimationDeclaration } from './animations/animation-declaration.model';
import { AnimationFrame } from './animations/animation-frame.model';

export interface ActionAnimation {
  generateDeclarations: (x: number, y: number, targetX: number, targetY: number, animation: ActionAnimation) => AnimationFrame[];
}
