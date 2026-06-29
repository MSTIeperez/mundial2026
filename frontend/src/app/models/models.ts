export interface User {
  id: number;
  username: string;
  email: string;
  role: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface TeamInfo {
  id: number;
  name: string;
  code: string;
  flagEmoji: string;
}

export interface Match {
  id: number;
  matchNumber: number;
  phase: string;
  group: string;
  matchDay: number;
  homeTeam: TeamInfo;
  awayTeam: TeamInfo;
  homeScore: number | null;
  awayScore: number | null;
  played: boolean;
  matchDate: string;
  venue: string;
  slotLabel?: string;
}

export interface Standing {
  position: number;
  team: TeamInfo;
  group: string;
  played: number;
  won: number;
  drawn: number;
  lost: number;
  goalsFor: number;
  goalsAgainst: number;
  goalDifference: number;
  points: number;
}

export interface KnockoutSlot {
  matchId: number;        // ID real del partido en la BD (para el PUT)
  matchNumber: number;    // Número FIFA del partido (73-88, 89-96...)
  slotLabel: string;
  homeTeam: TeamInfo | null;
  awayTeam: TeamInfo | null;
  resolved: boolean;
}

export type GroupStandings = Record<string, Standing[]>;
