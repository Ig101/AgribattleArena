export interface SyncSpecEffect {
  id: number;
  ownerId?: string;
  isAlive: boolean;
  x: number;
  y: number;
  z: number;
  duration?: number;
  nativeId: string;
}
