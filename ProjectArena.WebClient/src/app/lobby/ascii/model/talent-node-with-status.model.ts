import { CharacterClassEnum } from './enums/character-class.enum';
import { TalentNode } from './talent-node.model';
import { InaccessibilityReasonEnum } from './enums/inaccessibility-reason.enum';

export interface TalentNodeWithStatus extends TalentNode {
  initiallySelected: boolean;
  selected: boolean;
  accessible: boolean;
  inaccessibilityReason: InaccessibilityReasonEnum;
  char: string;
}
