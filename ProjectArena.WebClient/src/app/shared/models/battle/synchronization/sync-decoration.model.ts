import { TagSynergy } from '../tag-synergy.model';

export interface SyncDecoration {
  id: number;
  nativeId: string;
  mod: number;
  initiativePosition: number;
  health: number;
  ownerId?: string;
  isAlive: boolean;
  x: number;
  y: number;
  z: number;
  maxHealth: number;
  armor: TagSynergy[];
}
