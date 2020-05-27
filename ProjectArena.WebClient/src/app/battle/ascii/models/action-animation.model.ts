import { AnimationDeclaration } from './animations/animation-declaration.model';
import { AnimationFrame } from './animations/animation-frame.model';
import { Scene } from './scene/scene.model';

export interface ActionAnimation {
  generateDeclarations: (x: number, y: number, scene: Scene, animation: ActionAnimation) => AnimationFrame[];
}
