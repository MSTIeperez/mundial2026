import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GroupStandings, KnockoutSlot, Match, TeamInfo } from '../models/models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly API = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  // ─── Matches ────────────────────────────────────────────────────────
  getAllGroupMatches() {
    return this.http.get<Match[]>(`${this.API}/matches/groups`);
  }

  getGroupMatches(group: string) {
    return this.http.get<Match[]>(`${this.API}/matches/groups/${group}`);
  }

  updateScore(matchId: number, homeScore: number, awayScore: number) {
    return this.http.put(`${this.API}/matches/${matchId}/score`, { homeScore, awayScore });
  }

  // Actualiza los equipos de un partido eliminatorio (uno o ambos pueden ser null)
  updateKnockoutTeams(matchId: number, homeTeamId: number | null, awayTeamId: number | null) {
    return this.http.put(`${this.API}/matches/${matchId}/teams`, { homeTeamId, awayTeamId });
  }

  // Lista todos los equipos (para el selector del modal)
  getAllTeams() {
    return this.http.get<TeamInfo[]>(`${this.API}/matches/teams`);
  }
  
  getKnockoutMatches(phase: string) {
    return this.http.get<Match[]>(`${this.API}/matches/knockout/${phase}`);
  }

   getAllKnockoutMatches() {
    return this.http.get<Match[]>(`${this.API}/matches/knockout`);
  }

  // ─── Standings ───────────────────────────────────────────────────────
  getAllStandings() {
    return this.http.get<GroupStandings>(`${this.API}/standings`);
  }

  getAllKnockoutSlots() {
    return this.http.get<KnockoutSlot[]>(`${this.API}/standings/knockout-slots`);
  }
}
