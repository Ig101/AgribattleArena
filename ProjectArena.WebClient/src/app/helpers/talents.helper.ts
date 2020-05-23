import { TalentNode } from '../lobby/ascii/model/talent-node.model';
import { CharacterClassEnum } from '../lobby/ascii/model/enums/character-class.enum';

export function getNativeIdByTalents(characterTalents: TalentNode[]): string {
  const classPoints: { class: CharacterClassEnum, points: number }[] = [];
  const classes = Object.keys(CharacterClassEnum).filter(x => typeof CharacterClassEnum[x] === 'number').map(x => CharacterClassEnum[x]);
  for (const item of classes) {
    classPoints.push({
      class: item,
      points: characterTalents.filter(x => x.class === item).reduce((a, b) => a + b.classPoints, 0)
    });
  }
  let maxPoints = 0;
  const bestClass = classPoints.reduce((a, b) => {
    if (a === undefined || maxPoints < b.points) {
      maxPoints = b.points;
      return b;
    } else if (maxPoints === b.points) {
      return null;
    }
    return a;
  }, undefined);
  switch (bestClass?.class) {
    case CharacterClassEnum.Architect:
      return 'architect';
    case CharacterClassEnum.Bloodletter:
      return 'bloodletter';
    case CharacterClassEnum.Enchanter:
      return 'enchanter';
    case CharacterClassEnum.Fighter:
      return 'fighter';
    case CharacterClassEnum.Mistcaller:
      return 'mistcaller';
    case CharacterClassEnum.Ranger:
      return 'ranger';
    default:
      return 'adventurer';
  }
}
