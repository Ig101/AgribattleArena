import { TileStub } from './tile-stub.model';
import { Log } from './log.model';

export interface ChangeDefinition {
  time: number;
  tileStubs: TileStub[];
  logs: Log[];
  action: () => void;
}
