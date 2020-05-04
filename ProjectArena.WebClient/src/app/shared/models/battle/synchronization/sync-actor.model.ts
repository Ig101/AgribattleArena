import { SyncSkill } from './sync-skill.model';
import { SyncBuff } from './sync-buff.model';
import { TagSynergy } from '../tag-synergy.model';

export interface SyncActor {
  id: number;
  externalId?: number;
  nativeId: string;
  attackingSkill: SyncSkill;
  skills: SyncSkill[];
  buffs: SyncBuff[];
  initiativePosition: number;
  health: number;
  ownerId?: string;
  x: number;
  y: number;
  z: number;
  maxHealth: number;
  actionPoints: number;
  initiative: number;
  canMove: boolean;
  canAct: boolean;
}
