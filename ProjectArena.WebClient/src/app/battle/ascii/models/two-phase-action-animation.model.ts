import { AnimationDeclaration } from './animations/animation-declaration.model';
import { AnimationFrame } from './animations/animation-frame.model';
import { Actor } from './scene/actor.model';
import { Tile } from './scene/tile.model';

export interface TwoPhaseActionAnimation {
  generateIssueDeclarations: (issuer: Actor, tile: Tile, animation: TwoPhaseActionAnimation)
                              => AnimationFrame[];
  generateSyncDeclarations: (issuer: Actor, tile: Tile, animation: TwoPhaseActionAnimation)
                             => AnimationFrame[];
}
