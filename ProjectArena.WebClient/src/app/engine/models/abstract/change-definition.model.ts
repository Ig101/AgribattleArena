import { TileStub } from './tile-stub.model';

export interface ChangeDefinition {
  time: number;
  tileStubs: TileStub[];
  action: () => void;
}
