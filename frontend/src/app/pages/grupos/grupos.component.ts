import { Component, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { GroupStandings, Match } from '../../models/models';
import { FlagUrlPipe } from '../../pipes/flag-url.pipe';

interface ScoreEdit { homeScore: number; awayScore: number; }

@Component({
  selector: 'app-grupos',
  standalone: true,
  imports: [FormsModule, FlagUrlPipe, DatePipe],
  template: `
    <div class="page">
      <header class="page-header" [class.sticky]="true">
        <div>
          <h1 class="page-title">FASE DE <span class="text-gold">GRUPOS</span></h1>
          <p class="text-muted">FIFA World Cup 2026 · 48 selecciones · 12 grupos</p>
        </div>
        <div class="group-nav">
          @for (g of groupLetters; track g) {
            <button class="group-tab" [class.active]="activeGroup() === g" (click)="activeGroup.set(g)">
              {{ g }}
            </button>
          }
        </div>
      </header>

      @if (loading()) {
        <div class="spinner"></div>
      } @else {
        <div class="group-layout">
          <section class="matches-panel">
            <h2 class="panel-title">
              ⚽ GRUPO <span class="text-gold">{{ activeGroup() }}</span>
              <span class="badge badge-muted">{{ groupMatches().length }} partidos</span>
            </h2>

            @for (day of matchDays(); track day) {
              <div class="matchday-block">
                <div class="matchday-label">Jornada {{ day }}</div>

                @for (match of matchesByDay()[day]; track match.id) {
                  <div class="match-card" [class.played]="match.played" [class.live]="isLive(match.matchDate)">
                    <div class="match-meta">
                      <span class="text-muted" style="font-size:11px">{{ match.venue }}</span>
                      <span class="match-date">{{ match.matchDate | date:'dd/MM/yyyy hh:mm' }}</span>
                      @if (isLive(match.matchDate)) {
                        <span class="badge badge-live">🔴 En vivo</span>
                      } @else {
                        <span class="badge" [class]="match.played ? 'badge-green' : 'badge-muted'">
                          {{ match.played ? 'Jugado' : 'Pendiente' }}
                        </span>
                      }
                    </div>

                    <div class="match-body">
                      <div class="team-side home">
                        <img class="flag-img"
                          [src]="match.homeTeam.code | flagUrl"
                          [alt]="match.homeTeam.name"
                          loading="lazy">
                        <span class="team-name">{{ match.homeTeam.name }}</span>
                        <span class="team-code text-muted">{{ match.homeTeam.code }}</span>
                      </div>

                      <div class="score-area">
                        @if (editingId() === match.id) {
                          <input type="number" class="form-control score-input"
                            min="0" max="20"
                            [(ngModel)]="edits[match.id].homeScore">
                          <span class="score-sep">—</span>
                          <input type="number" class="form-control score-input"
                            min="0" max="20"
                            [(ngModel)]="edits[match.id].awayScore">
                        } @else {
                          <span class="score-display" [class.text-gold]="match.played">
                            {{ match.played ? match.homeScore : '·' }}
                          </span>
                          <span class="score-sep">–</span>
                          <span class="score-display" [class.text-gold]="match.played">
                            {{ match.played ? match.awayScore : '·' }}
                          </span>
                        }
                      </div>

                      <div class="team-side away">
                        <span class="team-code text-muted">{{ match.awayTeam.code }}</span>
                        <span class="team-name">{{ match.awayTeam.name }}</span>
                        <img class="flag-img"
                          [src]="match.awayTeam.code | flagUrl"
                          [alt]="match.awayTeam.name"
                          loading="lazy">
                      </div>
                    </div>

                    @if (canEdit()) {
                      <div class="match-actions">
                        @if (editingId() === match.id) {
                          <button class="btn btn-primary" style="font-size:12px;padding:6px 14px"
                            [disabled]="saving()" (click)="saveScore(match.id)">
                            {{ saving() ? '...' : '✓ Guardar' }}
                          </button>
                          <button class="btn btn-ghost" style="font-size:12px;padding:6px 14px"
                            (click)="editingId.set(null)">
                            Cancelar
                          </button>
                        } @else {
                          <button class="btn btn-ghost" style="font-size:12px;padding:6px 14px"
                            (click)="startEdit(match)">
                            ✏️ {{ match.played ? 'Editar' : 'Registrar' }}
                          </button>
                        }
                      </div>
                    }
                  </div>
                }
              </div>
            }
          </section>

          <section class="standings-panel">
            <h2 class="panel-title">📊 TABLA DE POSICIONES</h2>

            @if (currentStandings().length > 0) {
              <div class="card" style="overflow:hidden;padding:0">
                <table class="standings-table" >
                  <thead>
                    <tr>
                      <th style="width:32px">#</th>
                      <th>Selección</th>
                      <th title="Jugados">PJ</th>
                      <th title="Ganados">G</th>
                      <th title="Empatados">E</th>
                      <th title="Perdidos">P</th>
                      <th title="Goles a Favor">GF</th>
                      <th title="Goles en Contra">GC</th>
                      <th title="Diferencia de Goles">DG</th>
                      <th title="Puntos"><strong>Pts</strong></th>
                    </tr>
                  </thead>
                  <tbody>
                    @for (s of currentStandings(); track s.team.id) {
                      <tr [class]="getRowClass(s.position)">
                        <td>
                          <span class="pos-badge" [class]="getPosBadge(s.position)">
                            {{ s.position }}
                          </span>
                        </td>
                        <td>
                          <div class="team-cell">
                            <img class="flag-img flag-sm"
                              [src]="s.team.code | flagUrl"
                              [alt]="s.team.name"
                              loading="lazy">
                            <span>{{ s.team.name }}</span>
                          </div>
                        </td>
                        <td class="text-muted">{{ s.played }}</td>
                        <td class="text-green">{{ s.won }}</td>
                        <td class="text-muted">{{ s.drawn }}</td>
                        <td style="color:var(--c-accent2)">{{ s.lost }}</td>
                        <td>{{ s.goalsFor }}</td>
                        <td class="text-muted">{{ s.goalsAgainst }}</td>
                        <td [class]="s.goalDifference > 0 ? 'text-green' : ''">
                          {{ s.goalDifference > 0 ? '+' : '' }}{{ s.goalDifference }}
                        </td>
                        <td><strong class="text-mono">{{ s.points }}</strong></td>
                      </tr>
                    }
                  </tbody>
                </table>
                <div class="standings-legend">
                  <span class="legend-item green">▌ Avanza (1°-2°)</span>
                  <span class="legend-item gold">▌ Posible 3er lugar</span>
                </div>
              </div>
            } @else {
              <div class="card empty-state">
                <div style="font-size:40px;margin-bottom:8px">📋</div>
                <p>Registra resultados para ver la tabla</p>
              </div>
            }
          </section>
        </div>
      }
    </div>

    @if (toast()) {
      <div class="toast" [class]="toastType()">{{ toast() }}</div>
    }
  `,
  styles: [`
    .page { padding: 24px; max-width: 1400px; margin: 0 auto; }

    .page-header {
      display: flex; justify-content: space-between; align-items: flex-start;
      margin-bottom: 28px; flex-wrap: wrap; gap: 16px;
    }
    .page-header.sticky {
      position: sticky; top: var(--navbar-height, 60px); background: var(--c-bg); z-index: 100;
      padding: 16px 24px; margin-bottom: 16px; border-bottom: 1px solid var(--c-border);
      box-shadow: 0 2px 8px rgba(0,0,0,.1);
    }
    .page-title { font-size: 32px; margin-bottom: 4px; }
    
    .group-nav { display: flex; flex-wrap: wrap; gap: 6px; }
    .group-tab {
      width: 36px; height: 36px; border: 1px solid var(--c-border);
      background: var(--c-card); color: var(--c-muted);
      border-radius: var(--radius); font-weight: 700; font-size: 13px;
      cursor: pointer; transition: all .2s;
    }
    .group-tab.active,
    .group-tab:hover { border-color: var(--c-accent); color: var(--c-accent); background: rgba(200,162,39,.1); }

    .group-layout {
      display: grid; grid-template-columns: 1fr 430px; gap: 24px; align-items: start;
    }

    .panel-title {
      font-size: 18px; margin-bottom: 16px;
      display: flex; align-items: center; gap: 10px;
    }

    /* Flags — image-based, universal browser support */
    .flag-img {
      width: 80px; height: 53px;
      object-fit: cover;
      border-radius: 3px;
      box-shadow: 0 1px 4px rgba(0,0,0,.5);
      flex-shrink: 0;
      display: block;
    }
    .flag-img.flag-sm { width: 26px; height: 20px; }

    /* Matchday */
    .matchday-block { margin-bottom: 24px; }
    .matchday-label {
      font-size: 11px; font-weight: 700; text-transform: uppercase;
      letter-spacing: .1em; color: var(--c-muted);
      padding: 0 0 8px; margin-bottom: 10px;
      border-bottom: 1px solid var(--c-border);
    }

    .match-card {
      background: var(--c-card); border: 1px solid var(--c-border);
      border-radius: var(--radius-lg); padding: 16px;
      margin-bottom: 10px; transition: border-color .2s;
    }
    .match-card:hover { border-color: var(--c-accent); }
    .match-card.played { border-left: 3px solid var(--c-green); }
    .match-card.live { border-left: 3px solid #ff0000; animation: pulse 1.5s infinite; }
    
    @keyframes pulse {
      0%, 100% { opacity: 1; }
      50% { opacity: 0.7; }
    }

    .match-meta {
      display: flex; justify-content: space-between;
      margin-bottom: 12px; align-items: center; gap: 8px;
    }

    .match-body { display: flex; align-items: center; gap: 50px; padding: 15px 106px; }

    .team-side {
      flex: 1; display: flex; align-items: center; gap: 8px;
    }
    .team-side.home { justify-content: flex-start; }
    .team-side.away { justify-content: flex-end; }

    .team-name { font-weight: 600; font-size: 13px; }
    .team-code { font-family: var(--font-mono); font-size: 11px; }

    .score-area {
      display: flex; align-items: center; gap: 8px;
      min-width: 140px; justify-content: center;
    }
    .score-display {
      font-family: var(--font-mono); font-size: 37px; font-weight: 700;
      min-width: 36px; text-align: center;
    }
    .score-sep { color: var(--c-muted); font-size: 20px; }
    
    .score-input { font-size: 26px; text-align: -webkit-right; -webkit-appearance: none; margin: 0; -moz-appearance: textfield; }
    .score-input::-webkit-outer-spin-button,
    .score-input::-webkit-inner-spin-button { -webkit-appearance: none; margin: 0; }
    
    .match-actions { display: flex; gap: 8px; margin-top: 12px; justify-content: flex-end; }

    .team-cell { display: flex; align-items: center; gap: 10px; font-weight: 500; }
    .pos-badge {
      width: 22px; height: 22px; border-radius: 50%;
      display: flex; align-items: center; justify-content: center;
      font-size: 11px; font-weight: 700;
      background: var(--c-border); color: var(--c-muted);
    }
    .pos-badge.q { background: rgba(45,198,83,.2); color: var(--c-green); }
    .pos-badge.t { background: rgba(200,162,39,.2); color: var(--c-accent); }

    .tr-qualify td { background: rgba(45,198,83,.04); }
    .tr-third td   { background: rgba(200,162,39,.04); }
    .standings-table table {
      margin: -1px;;
    }
    .standings-legend {
      padding: 10px 16px; display: flex; gap: 16px;
      border-top: 1px solid var(--c-border); font-size: 11px;
    }
    .legend-item.green { color: var(--c-green); }
    .legend-item.gold  { color: var(--c-accent); }

    .badge-live { background: rgba(255,0,0,.2) !important; color: #ff0000 !important; }

    .empty-state { text-align: center; padding: 40px; color: var(--c-muted); font-size: 13px; }

    @media (max-width: 1024px) {
      .group-layout { grid-template-columns: 1fr; }
    }
    @media (max-width: 600px) {
      .team-name { display: none; }
      .match-body { gap: 6px; }
      .flag-img { width: 26px; height: 18px; }
    }
  `]
})
export class GruposComponent implements OnInit {
  groupLetters = 'ABCDEFGHIJKL'.split('');
  activeGroup = signal('A');
  loading = signal(true);
  saving = signal(false);
  editingId = signal<number | null>(null);
  toast = signal('');
  toastType = signal('success');

  allMatches = signal<Match[]>([]);
  standings = signal<GroupStandings>({});
  edits: Record<number, ScoreEdit> = {};

  constructor(private api: ApiService, public auth: AuthService) {}

  canEdit = computed(() => this.auth.isAdmin());

  groupMatches = computed(() =>
    this.allMatches().filter(m => m.group === this.activeGroup()));

  matchDays = computed(() =>
    [...new Set(this.groupMatches().map(m => m.matchDay))].sort());

  matchesByDay = computed(() => {
    const map: Record<number, Match[]> = {};
    for (const m of this.groupMatches()) {
      (map[m.matchDay] ??= []).push(m);
    }
    return map;
  });

  currentStandings = computed(() =>
    this.standings()[this.activeGroup()] ?? []);

  ngOnInit() {
    this.api.getAllGroupMatches().subscribe({
      next: m => this.allMatches.set(m),
      complete: () => this.loading.set(false)
    });
    this.api.getAllStandings().subscribe(s => this.standings.set(s));
  }

  isLive(matchDate: Date | string): boolean {
    const now = new Date();
    const match = new Date(matchDate);
    const diffMs = Math.abs(now.getTime() - match.getTime());
    const diffMinutes = Math.floor(diffMs / (1000 * 60));
    return diffMinutes < 120;
  }

  startEdit(match: Match) {
    this.edits[match.id] = {
      homeScore: match.homeScore ?? 0,
      awayScore: match.awayScore ?? 0
    };
    
    this.editingId.set(match.id);
    
    setTimeout(() => {
      const input = document.querySelector(`.match-card:has(input) .score-input`) as HTMLInputElement;
      input?.focus();
    });
  }

  saveScore(matchId: number) {
    const e = this.edits[matchId];
    this.saving.set(true);
    this.api.updateScore(matchId, e.homeScore, e.awayScore).subscribe({
      next: () => {
        this.allMatches.update(matches =>
          matches.map(m => m.id === matchId
            ? { ...m, homeScore: e.homeScore, awayScore: e.awayScore, played: true }
            : m));
        this.editingId.set(null);
        this.saving.set(false);
        this.showToast('✅ Resultado guardado', 'success');
        this.api.getAllStandings().subscribe(s => this.standings.set(s));
      },
      error: () => {
        this.saving.set(false);
        this.showToast('❌ Error al guardar', 'error');
      }
    });
  }

  getRowClass(pos: number): string {
    if (pos <= 2) return 'tr-qualify';
    if (pos === 3) return 'tr-third';
    return '';
  }

  getPosBadge(pos: number): string {
    if (pos <= 2) return 'q';
    if (pos === 3) return 't';
    return '';
  }

  showToast(msg: string, type: string) {
    this.toast.set(msg);
    this.toastType.set(type);
    setTimeout(() => this.toast.set(''), 3000);
  }
}
