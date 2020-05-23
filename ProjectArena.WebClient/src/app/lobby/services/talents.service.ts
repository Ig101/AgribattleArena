import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TalentNode } from '../ascii/model/talent-node.model';
import { UserService } from 'src/app/shared/services/user.service';
import { TalentNodeWithStatus } from '../ascii/model/talent-node-with-status.model';
import { talentChars } from '../ascii/natives/talents.natives';
import { InaccessibilityReasonEnum } from '../ascii/model/enums/inaccessibility-reason.enum';

@Injectable()
export class TalentsService {

  constructor(
    private userService: UserService
  ) { }

  getTalentsMap(): TalentNodeWithStatus[][] {
    const talents = this.userService.user.talentsMap;
    const map = new Array<TalentNodeWithStatus[]>(49);
    for (let i = 0; i < 49; i++) {
      map[i] = new Array<TalentNodeWithStatus>(25);
    }

    for (const talent of talents) {
      const newTalent = Object.assign(talent) as TalentNodeWithStatus;
      newTalent.initiallySelected = false;
      newTalent.selected = false;
      newTalent.accessible = false;
      newTalent.inaccessibilityReason = InaccessibilityReasonEnum.Unreachable;
      map[talent.x][talent.y] = newTalent;
      newTalent.char = talentChars[talent.id];
    }

    return map;
  }
}
