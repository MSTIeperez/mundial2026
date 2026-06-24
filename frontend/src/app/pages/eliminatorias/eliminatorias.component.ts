import { Component, OnInit, signal } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { GroupStandings, KnockoutSlot } from '../../models/models';

@Component({
  selector: 'app-eliminatorias',
  standalone: true,
  template: `
    <div class="page">
      <header class="page-header">
        <div>
          <h1 class="page-title">FASE <span class="text-gold">ELIMINATORIA</span></h1>
          <p class="text-muted">Ronda de 32 — Basado en resultados de la fase de grupos</p>
        </div>
        <div class="legend-pills">
          <span class="badge badge-green">✓ Clasificado</span>
          <span class="badge badge-gold">? Por definir</span>
          <span class="badge badge-muted">— Pendiente</span>
        </div>
      </header>

      <div class="rules-banner card">
        <h3 style="font-size:14px;font-weight:700;margin-bottom:8px;color:var(--c-accent)">
          📋 REGLAS FIFA 2026 — FASE DE GRUPOS
        </h3>
        <div class="rules-grid">
          <div class="rule-item">
            <strong>48 equipos</strong> organizados en <strong>12 grupos</strong> de 4
          </div>
          <div class="rule-item">
            Los <strong>primeros 2</strong> de cada grupo clasifican directamente (24 equipos)
          </div>
          <div class="rule-item">
            Los <strong>8 mejores terceros</strong> también avanzan → Total <strong>32 equipos</strong>
          </div>
          <div class="rule-item">
            Criterio de desempate: Puntos → DG → GF → Fairplay → Sorteo
          </div>
        </div>
      </div>

      @if (loading()) {
        <div class="spinner"></div>
      } @else {
        <!-- All groups summary -->
        <section class="section">
          <h2 class="section-title">📊 RESUMEN POR GRUPOS</h2>
          <div class="groups-summary-grid">
            @for (group of groupEntries(); track group[0]) {
              <div class="group-summary card">
                <div class="group-badge">Grupo {{ group[0] }}</div>
                @for (s of group[1]; track s.team.id; let i = $index) {
                  <div class="summary-row" [class.qualified]="i < 2" [class.third]="i === 2">
                    <span class="pos">{{ i + 1 }}</span>
                    <span class="flag-sm">{{ s.team.flagEmoji }}</span>
                    <span class="team-sm">{{ s.team.code }}</span>
                    <span class="pts-sm">{{ s.points }}pts</span>
                    @if (i < 2) { <span class="badge-sm green">✓</span> }
                    @if (i === 2) { <span class="badge-sm gold">?</span> }
                  </div>
                }
              </div>
            }
          </div>
        </section>

        <!-- Round of 32 bracket -->
        <section class="section">
          <h2 class="section-title">🥊 RONDA DE 32 — ENCUENTROS</h2>
          <p class="text-muted" style="margin-bottom:20px;font-size:13px">
            Los emparejamientos se actualizan en tiempo real según los resultados de la fase de grupos.
            Los 3er lugares restantes se asignan tras conocer los 8 mejores.
          </p>
          <div class="bracket-grid">
            @for (slot of slots(); track slot.slotLabel; let i = $index) {
              <div class="bracket-match card" [class.resolved]="slot.resolved">
                <div class="bracket-num">{{ i + 1 }}</div>
                <div class="bracket-label">{{ slot.slotLabel }}</div>
                <div class="bracket-teams">
                  <div class="bracket-team" [class.tbd]="!slot.homeTeam">
                    @if (slot.homeTeam) {
                      <span class="flag">{{ slot.homeTeam.flagEmoji }}</span>
                      <span>{{ slot.homeTeam.name }}</span>
                      <span class="code">{{ slot.homeTeam.code }}</span>
                    } @else {
                      <span class="tbd-flag">❓</span>
                      <span class="text-muted">Por definir</span>
                    }
                  </div>
                  <div class="vs-divider">vs</div>
                  <div class="bracket-team away" [class.tbd]="!slot.awayTeam">
                    @if (slot.awayTeam) {
                      <span class="code">{{ slot.awayTeam.code }}</span>
                      <span>{{ slot.awayTeam.name }}</span>
                      <span class="flag">{{ slot.awayTeam.flagEmoji }}</span>
                    } @else {
                      <span class="text-muted">Por definir</span>
                      <span class="tbd-flag">❓</span>
                    }
                  </div>
                </div>
                <div class="bracket-status">
                  @if (slot.resolved) {
                    <span class="badge badge-green" style="font-size:10px">Definido</span>
                  } @else {
                    <span class="badge badge-muted" style="font-size:10px">Pendiente resultados</span>
                  }
                </div>
              </div>
            }
          </div>
        </section>

        <!-- Best 3rd place tracker -->
        <section class="section">
          <h2 class="section-title">🏅 SEGUIMIENTO MEJORES 3ros LUGARES</h2>
          <div class="card">
            @if (thirdPlaces().length > 0) {
              <p class="text-muted" style="font-size:12px;margin-bottom:16px">
                Clasifican los 8 mejores de entre los 12 grupos. Ordenados por puntos, diferencia de goles y goles a favor.
              </p>
              <table class="standings-table">
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Selección</th>
                    <th>Grupo</th>
                    <th>PJ</th>
                    <th>Pts</th>
                    <th>DG</th>
                    <th>GF</th>
                    <th>Estado</th>
                  </tr>
                </thead>
                <tbody>
                  @for (s of thirdPlaces(); track s.team.id; let i = $index) {
                    <tr>
                      <td>{{ i + 1 }}</td>
                      <td>
                        <div class="team-cell">
                          {{ s.team.flagEmoji }} {{ s.team.name }}
                        </div>
                      </td>
                      <td><span class="badge badge-muted">{{ s.group }}</span></td>
                      <td class="text-muted">{{ s.played }}</td>
                      <td><strong class="text-mono">{{ s.points }}</strong></td>
                      <td [class]="s.goalDifference >= 0 ? 'text-green' : ''">
                        {{ s.goalDifference > 0 ? '+' : '' }}{{ s.goalDifference }}
                      </td>
                      <td>{{ s.goalsFor }}</td>
                      <td>
                        @if (i < 8) {
                          <span class="badge badge-green">✓ Clasificaría</span>
                        } @else {
                          <span class="badge badge-red">✗ Fuera</span>
                        }
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            } @else {
              <div class="empty-state" style="text-align:center;padding:32px;color:var(--c-muted)">
                <div style="font-size:36px;margin-bottom:8px">⏳</div>
                <p>Aún no hay resultados de grupos para calcular los mejores terceros</p>
              </div>
            }
          </div>
        </section>
      }
    </div>
  `,
  styles: [`
    .page { padding: 24px; max-width: 1400px; margin: 0 auto; }

    .page-header {
      display: flex; justify-content: space-between; align-items: flex-start;
      margin-bottom: 24px; flex-wrap: wrap; gap: 16px;
    }
    .page-title { font-size: 32px; margin-bottom: 4px; }
    .legend-pills { display: flex; gap: 8px; flex-wrap: wrap; }

    .rules-banner { margin-bottom: 28px; border-color: rgba(200,162,39,.3); }
    .rules-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; }
    .rule-item {
      font-size: 13px; color: var(--c-muted);
      padding: 8px 12px; background: var(--c-surface);
      border-radius: var(--radius);
    }

    .section { margin-bottom: 40px; }
    .section-title { font-size: 20px; margin-bottom: 20px; }

    /* Groups Summary */
    .groups-summary-grid {
      display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 12px;
    }
    .group-summary { padding: 14px; }
    .group-badge {
      font-family: var(--font-display); font-size: 16px;
      color: var(--c-accent); margin-bottom: 12px;
    }
    .summary-row {
      display: flex; align-items: center; gap: 6px;
      padding: 5px 0; border-bottom: 1px solid rgba(42,52,72,.4);
      font-size: 13px;
    }
    .summary-row:last-child { border-bottom: none; }
    .summary-row.qualified { background: transparent; }
    .pos { width: 14px; text-align: center; color: var(--c-muted); font-size: 11px; }
    .flag-sm { font-size: 18px; }
    .team-sm { flex: 1; font-weight: 600; font-family: var(--font-mono); font-size: 12px; }
    .pts-sm { color: var(--c-muted); font-size: 11px; }
    .badge-sm { font-size: 10px; font-weight: 700; }
    .badge-sm.green { color: var(--c-green); }
    .badge-sm.gold  { color: var(--c-accent); }

    /* Bracket */
    .bracket-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 14px; }
    .bracket-match { padding: 16px; position: relative; transition: border-color .2s; }
    .bracket-match.resolved { border-color: rgba(45,198,83,.4); }

    .bracket-num {
      position: absolute; top: -8px; left: 16px;
      background: var(--c-bg); padding: 0 6px;
      font-size: 10px; font-weight: 700;
      color: var(--c-muted); font-family: var(--font-mono);
    }
    .bracket-label {
      font-size: 10px; color: var(--c-muted); margin-bottom: 12px;
      font-family: var(--font-mono); text-transform: uppercase; letter-spacing: .06em;
    }
    .bracket-teams { display: flex; flex-direction: column; gap: 6px; margin-bottom: 12px; }
    .bracket-team {
      display: flex; align-items: center; gap: 10px;
      padding: 8px 12px; background: var(--c-surface);
      border-radius: var(--radius); font-size: 13px; font-weight: 500;
    }
    .bracket-team.away { flex-direction: row-reverse; }
    .bracket-team.tbd { opacity: .6; }
    .bracket-team .flag { font-size: 22px; }
    .bracket-team .tbd-flag { font-size: 20px; }
    .bracket-team .code { font-family: var(--font-mono); font-size: 11px; color: var(--c-muted); }
    .vs-divider { text-align: center; color: var(--c-muted); font-size: 11px; font-weight: 700; }
    .bracket-status { display: flex; justify-content: flex-end; }

    .team-cell { display: flex; align-items: center; gap: 8px; }
    .badge-red { background: rgba(230,57,70,.1); color: var(--c-accent2); }

    @media (max-width: 768px) {
      .rules-grid { grid-template-columns: 1fr; }
      .groups-summary-grid { grid-template-columns: repeat(2, 1fr); }
      .bracket-grid { grid-template-columns: 1fr; }
    }
  `]
})
export class EliminatoriasComponent implements OnInit {
  loading = signal(true);
  slots = signal<KnockoutSlot[]>([]);
  standingsMap = signal<GroupStandings>({});

  groupEntries = signal<[string, any[]][]>([]);
  thirdPlaces = signal<any[]>([]);

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getAllStandings().subscribe(s => {
      this.standingsMap.set(s);
      this.groupEntries.set(Object.entries(s).sort());
      // Collect all 3rd place teams
      const thirds = Object.entries(s)
        .filter(([, standings]) => standings.length >= 3)
        .map(([, standings]) => standings[2])
        .sort((a, b) =>
          b.points - a.points ||
          b.goalDifference - a.goalDifference ||
          b.goalsFor - a.goalsFor);
      this.thirdPlaces.set(thirds);
    });

    this.api.getRoundOf32().subscribe({
      next: s => this.slots.set(s),
      complete: () => this.loading.set(false)
    });
  }
}
