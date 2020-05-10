import { LobbyTileActivator } from './lobby-tile-activator.model';
import { Color } from 'src/app/shared/models/color.model';

export interface LobbyTile<T> {
  activator: LobbyTileActivator<T>;
  char: string;
  color: Color;
  backgroundColor: Color;
}
