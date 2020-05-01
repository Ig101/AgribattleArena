import { SyncSkill } from './sync-skill.model';
import { SyncBuff } from './sync-buff.model';
import { TagSynergy } from '../tag-synergy.model';

export interface SyncActor {
  id: number;
  externalId?: number;
  nativeId: string;
  attackingSkill: SyncSkill;
  strength: number;
  willpower: number;
  constitution: number;
  speed: number;
  skills: SyncSkill[];
  actionPointsIncome: number;
  buffs: SyncBuff[];
  initiativePosition: number;
  health: number;
  ownerId?: string;
  isAlive: boolean;
  x: number;
  y: number;
  z: number;
  maxHealth: number;
  actionPoints: number;
  skillPower: number;
  attackPower: number;
  initiative: number;
  armor: TagSynergy[];
  attackModifiers: TagSynergy[];
  canMove: boolean;
  canAct: boolean;
}
