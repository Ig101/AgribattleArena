import { AnimationDeclaration } from './animations/animation-declaration.model';
import { AnimationFrame } from './animations/animation-frame.model';

export interface TwoPhaseActionAnimation {
  generateIssueDeclarations: (x: number, y: number, targetX: number, targetY: number, animation: TwoPhaseActionAnimation)
                              => AnimationFrame[];
  generateSyncDeclarations: (x: number, y: number, targetX: number, targetY: number, animation: TwoPhaseActionAnimation)
                             => AnimationFrame[];
}
