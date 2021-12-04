export interface Reststop {
  name?: string;
  category: string;
  description?: string;
  location: number[];
  tags: { [id: string]: string };
  detour_seconds: number;
}
