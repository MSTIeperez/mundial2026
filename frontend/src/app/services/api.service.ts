import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GroupStandings, KnockoutSlot, Match } from '../models/models';

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

  getKnockoutMatches(phase: string) {
    return this.http.get<Match[]>(`${this.API}/knockout/${phase}`);
  }
  // ─── Standings ───────────────────────────────────────────────────────
  getAllStandings() {
    return this.http.get<GroupStandings>(`${this.API}/standings`);
  }

  getAllKnockoutSlots() {
    return this.http.get<KnockoutSlot[]>(`${this.API}/standings/knockout-slots`);
  }
}
