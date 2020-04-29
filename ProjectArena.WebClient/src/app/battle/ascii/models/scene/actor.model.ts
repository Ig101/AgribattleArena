import { TagSynergy } from 'src/app/shared/models/battle/tag-synergy.model';
import { Buff } from './buff.model';
import { Skill } from './skill.model';
import { Visualization } from '../visualization.model';
import { Player } from '../player.model';
import { TwoPhaseActionAnimation } from '../two-phase-action-animation.model';

export interface Actor {
  id: number;

  name: string;
  description: string;
  defaultVisualization: Visualization;
  defaultMoveAction: TwoPhaseActionAnimation;
  visualization: Visualization;
  moveAction: TwoPhaseActionAnimation;

  externalId?: number;
  attackingSkill: Skill;
  strength: number;
  willpower: number;
  constitution: number;
  speed: number;
  skills: Skill[];
  actionPointsIncome: number;
  buffs: Buff[];
  initiativePosition: number;
  health: number;
  owner: Player;
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
}
