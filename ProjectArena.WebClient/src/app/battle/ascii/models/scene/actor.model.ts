import { TagSynergy } from 'src/app/shared/models/battle/tag-synergy.model';
import { Buff } from './buff.model';
import { Skill } from './skill.model';
import { Visualization } from '../visualization.model';
import { Player } from '../player.model';
import { TwoPhaseActionAnimation } from '../two-phase-action-animation.model';
import { Color } from 'src/app/shared/models/color.model';

export interface Actor {
  id: number;

  name: string;
  description: string;
  defaultVisualization: Visualization;
  visualization: Visualization;

  externalId?: string;
  attackingSkill: Skill;
  skills: Skill[];
  buffs: Buff[];
  initiativePosition: number;
  health: number;
  owner: Player;
  x: number;
  y: number;
  z: number;
  maxHealth: number;
  actionPoints: number;
  initiative: number;
  canMove: boolean;
  canAct: boolean;
}
