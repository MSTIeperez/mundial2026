import { Component, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { GroupStandings, KnockoutSlot, TeamInfo } from '../../models/models';
import { FlagUrlPipe } from '../../pipes/flag-url.pipe';
import { CommonModule } from '@angular/common';

interface EditState {
  slot: KnockoutSlot;
  homeTeamId: number | null;
  awayTeamId: number | null;
}

@Component({
  selector: 'app-eliminatorias',
  standalone: true,
  imports: [FlagUrlPipe, FormsModule,CommonModule],
  template: `
    <div class="page">
      <header class="page-header">
        <div>
          <h1 class="page-title">FASE <span class="text-gold">ELIMINATORIA</span></h1>
          <p class="text-muted">Fases eliminatorias — Basado en resultados de la fase de grupos</p>
        </div>
        <div class="legend-pills">
          <span class="badge badge-green">✓ Clasificado</span>
          <span class="badge badge-gold">? Por definir</span>
          <span class="badge badge-muted">— Pendiente</span>
        </div>
      </header>

      <div class="rules-banner card">
        <h3 style="font-size:14px;font-weight:700;margin-bottom:8px;color:var(--c-accent)">
          📋 REGLAS FIFA 2026 — FASE ELIMINATORIA
        </h3>
        <div class="rules-grid">
          <div class="rule-item"><strong>32 equipos</strong> en Ronda de 32 (16 encuentros)</div>
          <div class="rule-item"><strong>16 equipos</strong> en Octavos de Final (8 encuentros)</div>
          <div class="rule-item"><strong>8 equipos</strong> en Cuartos de Final (4 encuentros)</div>
          <div class="rule-item"><strong>4 equipos</strong> en Semifinales + Final (3 encuentros)</div>
        </div>
      </div>

      @if (loading()) {
        <div class="spinner"></div>
      } @else {

        <!-- Resumen grupos -->
        <section class="section">
          <h2 class="section-title">📊 RESUMEN POR GRUPOS</h2>
          <div class="groups-summary-grid">
            @for (group of groupEntries(); track group[0]) {
              <div class="group-summary card">
                <div class="group-badge">Grupo {{ group[0] }}</div>
                @for (s of group[1]; track s.team.id; let i = $index) {
                  <div class="summary-row" [class.qualified]="i < 2" [class.third]="i === 2" [class.eliminated]="i > 2">
                    <span class="pos">{{ i + 1 }}</span>
                    <img class="flag-img flag-sm" [src]="s.team.code | flagUrl" [alt]="s.team.name" loading="lazy">
                    <span class="team-sm">{{ s.team.code }}</span>
                    <span class="pts-sm">{{ s.points }}pts</span>
                    @if (i < 2) { <span class="badge-sm green">✓</span> }
                    @if (i === 2) { <span class="badge-sm gold">?</span> }
                    @if (i > 2) { <span class="badge-sm red">✗</span> }
                  </div>
                }
              </div>
            }
          </div>
        </section>

        <!-- Ronda de 32 -->
        <section class="section">
          <h2 class="section-title">🥊 RONDA DE 32 — 16 ENCUENTROS</h2>
          <p class="text-muted" style="margin-bottom:20px;font-size:13px">
            Los emparejamientos se actualizan según los resultados de grupos.
            @if (canEdit()) { <span class="text-gold"> Haz clic en ✏️ para corregir manualmente un emparejamiento.</span> }
          </p>
          <div class="bracket-grid">
            @for (slot of getSlotsByRound(32); track slot.matchNumber) {
              <ng-container *ngTemplateOutlet="bracketCard; context: { $implicit: slot }"></ng-container>
            }
          </div>
        </section>

        <!-- Octavos -->
        <section class="section">
          <h2 class="section-title">🏆 OCTAVOS DE FINAL — 8 ENCUENTROS</h2>
          <p class="text-muted" style="margin-bottom:20px;font-size:13px">
            Se actualizan según resultados de Ronda de 32.
          </p>
          <div class="bracket-grid">
            @for (slot of getSlotsByRound(16); track slot.matchNumber) {
              <ng-container *ngTemplateOutlet="bracketCard; context: { $implicit: slot }"></ng-container>
            }
          </div>
        </section>

        <!-- Cuartos -->
        <section class="section">
          <h2 class="section-title">⚡ CUARTOS DE FINAL — 4 ENCUENTROS</h2>
          <p class="text-muted" style="margin-bottom:20px;font-size:13px">
            Se actualizan según resultados de Octavos.
          </p>
          <div class="bracket-grid">
            @for (slot of getSlotsByRound(8); track slot.matchNumber) {
              <ng-container *ngTemplateOutlet="bracketCard; context: { $implicit: slot }"></ng-container>
            }
          </div>
        </section>

        <!-- Semifinales -->
        <section class="section">
          <h2 class="section-title">⭐ SEMIFINALES — 2 ENCUENTROS</h2>
          <div class="bracket-grid">
            @for (slot of getSlotsByRound(4); track slot.matchNumber) {
              <ng-container *ngTemplateOutlet="bracketCard; context: { $implicit: slot }"></ng-container>
            }
          </div>
        </section>

        <!-- 3er lugar -->
        <section class="section">
          <h2 class="section-title">🥉 TERCER LUGAR</h2>
          <div class="bracket-grid">
            @for (slot of getSlotsByRound(1); track slot.matchNumber) {
              @if (slot.matchNumber === 103) {
                <ng-container *ngTemplateOutlet="bracketCard; context: { $implicit: slot }"></ng-container>
              }
            }
          </div>
        </section>

        <!-- Final -->
        <section class="section">
          <h2 class="section-title">👑 FINAL</h2>
          <div class="bracket-grid">
            @for (slot of getSlotsByRound(1); track slot.matchNumber) {
              @if (slot.matchNumber === 104) {
                <ng-container *ngTemplateOutlet="bracketCard; context: { $implicit: slot }"></ng-container>
              }
            }
          </div>
        </section>

        <!-- Mejores 3eros -->
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
                    <th>#</th><th>Selección</th><th>Grupo</th>
                    <th>PJ</th><th>Pts</th><th>DG</th><th>GF</th><th>Estado</th>
                  </tr>
                </thead>
                <tbody>
                  @for (s of thirdPlaces(); track s.team.id; let i = $index) {
                    <tr>
                      <td>{{ i + 1 }}</td>
                      <td>
                        <div class="team-cell">
                          <img class="flag-img flag-sm" [src]="s.team.code | flagUrl" [alt]="s.team.name" loading="lazy">
                          {{ s.team.name }}
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
                        @if (i < 8) { <span class="badge badge-green">✓ Clasificaría</span> }
                        @else { <span class="badge badge-red">✗ Fuera</span> }
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

    <!-- ─── Bracket card template ────────────────────────────────────────────── -->
    <ng-template #bracketCard let-slot>
      <div class="bracket-match card" [class.resolved]="slot.resolved" [class.partial]="slot.homeTeam || slot.awayTeam">
        <div class="bracket-num">Partido {{ slot.matchNumber }}</div>
        <div class="bracket-label">{{ slot.slotLabel }}</div>

        <div class="bracket-teams">
          <!-- Local -->
          <div class="bracket-team" [class.tbd]="!slot.homeTeam">
            @if (slot.homeTeam) {
              <img class="flag-img flag-sm" [src]="slot.homeTeam.code | flagUrl" [alt]="slot.homeTeam.name" loading="lazy">
              <span>{{ slot.homeTeam.name }}</span>
              <span class="code">{{ slot.homeTeam.code }}</span>
            } @else {
              <span class="tbd-flag">❓</span>
              <span class="text-muted">Por definir</span>
            }
          </div>
          <div class="vs-divider">vs</div>
          <!-- Visitante -->
          <div class="bracket-team away" [class.tbd]="!slot.awayTeam">
            @if (slot.awayTeam) {
              <span class="code">{{ slot.awayTeam.code }}</span>
              <span>{{ slot.awayTeam.name }}</span>
              <img class="flag-img flag-sm" [src]="slot.awayTeam.code | flagUrl" [alt]="slot.awayTeam.name" loading="lazy">
            } @else {
              <span class="text-muted">Por definir</span>
              <span class="tbd-flag">❓</span>
            }
          </div>
        </div>

        <div class="bracket-footer">
          <span class="badge" [class]="slot.resolved ? 'badge-green' : (slot.homeTeam || slot.awayTeam) ? 'badge-gold' : 'badge-muted'"
                style="font-size:10px">
            {{ slot.resolved ? 'Definido' : (slot.homeTeam || slot.awayTeam) ? 'Parcial' : 'Pendiente' }}
          </span>
          @if (canEdit()) {
            <button class="btn-edit" (click)="openEdit(slot)" title="Corregir equipos manualmente">
              ✏️ Editar
            </button>
          }
        </div>
      </div>
    </ng-template>

    <!-- ─── Modal de edición de equipos ─────────────────────────────────────── -->
    @if (editState()) {
      <div class="modal-overlay" (click)="closeEdit()">
        <div class="modal-box card" (click)="$event.stopPropagation()">
          <div class="modal-header">
            <h2 class="modal-title">✏️ EDITAR PARTIDO</h2>
            <button class="modal-close" (click)="closeEdit()">✕</button>
          </div>

          <div class="modal-slot-label">{{ editState()!.slot.slotLabel }}</div>

          @if (loadingTeams()) {
            <div class="spinner" style="margin:24px auto"></div>
          } @else {
            <!-- Selector equipo local -->
            <div class="form-group">
              <label>🏠 EQUIPO LOCAL</label>
              <select class="form-control" [(ngModel)]="editHomeId">
                <option [ngValue]="null">— Por definir —</option>
                @for (t of allTeams(); track t.id) {
                  <option [ngValue]="t.id">{{ t.name }} ({{ t.code }})</option>
                }
              </select>
            </div>

            <!-- Selector equipo visitante -->
            <div class="form-group">
              <label>✈️ EQUIPO VISITANTE</label>
              <select class="form-control" [(ngModel)]="editAwayId">
                <option [ngValue]="null">— Por definir —</option>
                @for (t of allTeams(); track t.id) {
                  <option [ngValue]="t.id">{{ t.name }} ({{ t.code }})</option>
                }
              </select>
            </div>

            <div class="hint-box">
              💡 Puedes dejar un equipo como <em>Por definir</em> si el oponente aún no está confirmado.
              El resultado automático de la fase de grupos seguirá calculándose en paralelo.
            </div>

            <div class="modal-actions">
              <button class="btn btn-ghost" (click)="closeEdit()">Cancelar</button>
              <button class="btn btn-primary" [disabled]="saving()" (click)="saveTeams()">
                {{ saving() ? 'Guardando...' : '✓ Guardar cambios' }}
              </button>
            </div>
          }
        </div>
      </div>
    }

    <!-- Toast -->
    @if (toast()) {
      <div class="toast" [class]="toastType()">{{ toast() }}</div>
    }
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
    .rule-item { font-size: 13px; color: var(--c-muted); padding: 8px 12px; background: var(--c-surface); border-radius: var(--radius); }

    .section { margin-bottom: 40px; }
    .section-title { font-size: 20px; margin-bottom: 20px; }

    /* Groups summary */
    .groups-summary-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 12px; }
    .group-summary { padding: 14px; }
    .group-badge { font-family: var(--font-display); font-size: 16px; color: var(--c-accent); margin-bottom: 12px; }
    .summary-row {
      display: flex; align-items: center; gap: 6px;
      padding: 5px 0; border-bottom: 1px solid rgba(42,52,72,.4); font-size: 13px;
    }
    .summary-row:last-child { border-bottom: none; }
    .pos { width: 14px; text-align: center; color: var(--c-muted); font-size: 11px; }

    /* Flags */
    .flag-img { width: 80px; height: 53px; object-fit: cover; border-radius: 3px; box-shadow: 0 1px 4px rgba(0,0,0,.5); flex-shrink: 0; display: block; }
    .flag-img.flag-sm { width: 26px; height: 20px; }
    .team-sm { flex: 1; font-weight: 600; font-family: var(--font-mono); font-size: 12px; }
    .pts-sm { color: var(--c-muted); font-size: 11px; }
    .badge-sm { font-size: 10px; font-weight: 700; }
    .badge-sm.green { color: var(--c-green); }
    .badge-sm.gold  { color: var(--c-accent); }
    .badge-sm.red   { color: var(--c-accent2); }

    /* Bracket cards */
    .bracket-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 14px; }
    .bracket-match { padding: 16px; position: relative; transition: border-color .2s; }
    .bracket-match.resolved { border-color: rgba(45,198,83,.4); }
    .bracket-match.partial  { border-color: rgba(200,162,39,.3); }

    .bracket-num {
      position: absolute; top: -8px; left: 16px;
      background: var(--c-bg); padding: 0 6px;
      font-size: 10px; font-weight: 700; color: var(--c-muted); font-family: var(--font-mono);
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
    .bracket-team .tbd-flag { font-size: 20px; }
    .bracket-team .code { font-family: var(--font-mono); font-size: 11px; color: var(--c-muted); }
    .vs-divider { text-align: center; color: var(--c-muted); font-size: 11px; font-weight: 700; }

    .bracket-footer { display: flex; justify-content: space-between; align-items: center; }

    .btn-edit {
      background: none; border: 1px solid var(--c-border);
      color: var(--c-muted); border-radius: var(--radius);
      padding: 4px 10px; font-size: 11px; cursor: pointer;
      transition: all .2s;
    }
    .btn-edit:hover { border-color: var(--c-accent); color: var(--c-accent); }

    .team-cell { display: flex; align-items: center; gap: 8px; }
    .badge-red { background: rgba(230,57,70,.1); color: var(--c-accent2); }

    /* Modal */
    .modal-overlay {
      position: fixed; inset: 0; z-index: 1000;
      background: rgba(0,0,0,.7); backdrop-filter: blur(4px);
      display: flex; align-items: center; justify-content: center; padding: 16px;
      animation: fadeIn .2s ease;
    }
    @keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }

    .modal-box {
      width: 100%; max-width: 520px;
      max-height: 90vh; overflow-y: auto;
      animation: slideUp .25s ease;
    }
    @keyframes slideUp { from { transform: translateY(24px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }

    .modal-header {
      display: flex; justify-content: space-between; align-items: center; margin-bottom: 6px;
    }
    .modal-title { font-size: 20px; }
    .modal-close {
      background: none; border: none; color: var(--c-muted);
      font-size: 18px; cursor: pointer; padding: 4px 8px; border-radius: 4px;
    }
    .modal-close:hover { color: var(--c-text); background: var(--c-border); }

    .modal-slot-label {
      font-family: var(--font-mono); font-size: 11px; color: var(--c-accent);
      text-transform: uppercase; letter-spacing: .08em;
      margin-bottom: 20px; padding: 6px 10px;
      background: rgba(200,162,39,.08); border-radius: var(--radius);
    }

    .team-preview {
      display: flex; align-items: center; gap: 10px;
      margin-top: 8px; padding: 8px 12px;
      background: var(--c-surface); border-radius: var(--radius);
      font-size: 13px; font-weight: 600;
    }

    .hint-box {
      background: rgba(200,162,39,.07); border: 1px solid rgba(200,162,39,.2);
      border-radius: var(--radius); padding: 10px 14px;
      font-size: 12px; color: var(--c-muted); margin-bottom: 20px; line-height: 1.6;
    }

    .modal-actions { display: flex; gap: 10px; justify-content: flex-end; }

    select option, select optgroup { background: var(--c-surface); color: var(--c-text); }

    @media (max-width: 768px) {
      .rules-grid { grid-template-columns: 1fr; }
      .groups-summary-grid { grid-template-columns: repeat(2, 1fr); }
      .bracket-grid { grid-template-columns: 1fr; }
    }
  `]
})
export class EliminatoriasComponent implements OnInit {
  loading      = signal(true);
  loadingTeams = signal(false);
  saving       = signal(false);
  toast        = signal('');
  toastType    = signal('success');

  slots        = signal<KnockoutSlot[]>([]);
  standingsMap = signal<GroupStandings>({});
  groupEntries = signal<[string, any[]][]>([]);
  thirdPlaces  = signal<any[]>([]);
  allTeams     = signal<TeamInfo[]>([]);

  // Modal state
  editState  = signal<EditState | null>(null);
  editHomeId: number | null = null;
  editAwayId: number | null = null;

  teamsByGroup = computed(() => {
    const map = new Map<string, TeamInfo[]>();
    for (const t of this.allTeams()) {
      // Agrupa por primera letra del code o por el grupo que viene del backend
      // Como TeamInfo no trae Group, los agrupamos alfabéticamente por nombre
      const key = t.name[0].toUpperCase();
      if (!map.has(key)) map.set(key, []);
      map.get(key)!.push(t);
    }
    // Regresamos agrupados A-Z para el select
    return Array.from(map.entries())
      .sort((a, b) => a[0].localeCompare(b[0]))
      .map(([group, teams]) => ({ group, teams }));
  });

  constructor(private api: ApiService, public auth: AuthService) {}

  canEdit = computed(() => this.auth.isAdmin());

  ngOnInit() {
    this.api.getAllStandings().subscribe(s => {
      this.standingsMap.set(s);
      this.groupEntries.set(Object.entries(s).sort());
      const thirds = Object.entries(s)
        .filter(([, standings]) => standings.length >= 3)
        .map(([, standings]) => standings[2])
        .sort((a, b) =>
          b.points - a.points ||
          b.goalDifference - a.goalDifference ||
          b.goalsFor - a.goalsFor);
      this.thirdPlaces.set(thirds);
    });

    this.api.getAllKnockoutSlots().subscribe({
      next: s => this.slots.set(s),
      complete: () => this.loading.set(false)
    });
  }

  getSlotsByRound(round: number): KnockoutSlot[] {
    return this.slots().filter(s => this.getRound(s.matchNumber) === round);
  }

  private getRound(matchNumber: number): number {
    if (matchNumber < 89)  return 32;
    if (matchNumber < 97)  return 16;
    if (matchNumber < 101) return 8;
    if (matchNumber < 103) return 4;
    return 1; // 103 = 3er lugar, 104 = Final
  }

  // ─── Modal ────────────────────────────────────────────────────────────────
  openEdit(slot: KnockoutSlot) {
    this.editHomeId = slot.homeTeam?.id ?? null;
    this.editAwayId = slot.awayTeam?.id ?? null;
    this.editState.set({ slot, homeTeamId: this.editHomeId, awayTeamId: this.editAwayId });

    // Cargar equipos la primera vez
    if (this.allTeams().length === 0) {
      this.loadingTeams.set(true);
      this.api.getAllTeams().subscribe({
        next: teams => this.allTeams.set(teams),
        complete: () => this.loadingTeams.set(false)
      });
    }
  }

  closeEdit() {
    this.editState.set(null);
  }

  saveTeams() {
    const state = this.editState();
    if (!state) return;

    this.saving.set(true);
    this.api.updateKnockoutTeams(state.slot.matchId, this.editHomeId, this.editAwayId).subscribe({
      next: () => {
        // Actualizar el slot localmente sin recargar todo
        this.slots.update(slots => slots.map(s => {
          if (s.matchId !== state.slot.matchId) return s;
          const home = this.editHomeId !== null ? this.getTeamById(this.editHomeId) ?? null : null;
          const away = this.editAwayId !== null ? this.getTeamById(this.editAwayId) ?? null : null;
          return { ...s, homeTeam: home, awayTeam: away, resolved: home !== null && away !== null };
        }));
        this.saving.set(false);
        this.closeEdit();
        this.showToast('✅ Partido actualizado', 'success');
      },
      error: () => {
        this.saving.set(false);
        this.showToast('❌ Error al guardar', 'error');
      }
    });
  }

  getTeamById(id: number | null): TeamInfo | undefined {
    if (id === null) return undefined;
    return this.allTeams().find(t => t.id === id);
  }

  showToast(msg: string, type: string) {
    this.toast.set(msg);
    this.toastType.set(type);
    setTimeout(() => this.toast.set(''), 3000);
  }
}
