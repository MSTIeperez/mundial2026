import { Component, computed } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="app-shell">
      @if (user()) {
        <nav class="navbar">
          <div class="nav-brand">
            <span class="trophy">🏆</span>
            <span class="brand-text">MUNDIAL <em>2026</em></span>
          </div>
          <div class="nav-links">
            <a routerLink="/grupos" routerLinkActive="active" class="nav-link">
              ⚽ Grupos & Posiciones
            </a>
            <a routerLink="/eliminatorias" routerLinkActive="active" class="nav-link">
              🥊 Eliminatorias
            </a>
            <a routerLink="/partidos-eliminatorios" routerLinkActive="active" class="nav-link">
              ⚽ Partidos Eliminatorios
            </a>
          </div>
          <div class="nav-user">
            <span class="user-chip">
              <span class="user-avatar">{{ initials() }}</span>
              {{ user()?.username }}
              @if (auth.isAdmin()) {
                <span class="badge badge-gold" style="font-size:9px">ADMIN</span>
              }
            </span>
            <button class="btn btn-ghost" style="padding:6px 12px;font-size:12px" (click)="auth.logout()">
              Salir
            </button>
          </div>
        </nav>
      }
      <main class="main-content">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [`
    .app-shell { min-height: 100vh; display: flex; flex-direction: column; }

    .navbar {
      display: flex; align-items: center; gap: 24px;
      padding: 0 24px; height: 64px;
      background: var(--c-surface);
      border-bottom: 1px solid var(--c-border);
      position: sticky; top: 0; z-index: 100;
    }

    .nav-brand {
      display: flex; align-items: center; gap: 10px;
      font-family: var(--font-display); font-size: 22px;
      color: var(--c-text); text-decoration: none;
    }
    .trophy { font-size: 26px; }
    .brand-text em { color: var(--c-accent); font-style: normal; }

    .nav-links { display: flex; gap: 4px; flex: 1; justify-content: center; }
    .nav-link {
      padding: 8px 16px; border-radius: var(--radius);
      color: var(--c-muted); text-decoration: none;
      font-weight: 500; font-size: 13px;
      transition: all .2s;
    }
    .nav-link:hover { color: var(--c-text); background: var(--c-card); }
    .nav-link.active { color: var(--c-accent); background: rgba(200,162,39,.1); }

    .nav-user { display: flex; align-items: center; gap: 10px; margin-left: auto; }
    .user-chip {
      display: flex; align-items: center; gap: 8px;
      font-size: 13px; color: var(--c-muted);
    }
    .user-avatar {
      width: 28px; height: 28px;
      background: var(--c-accent); color: #0a0e1a;
      border-radius: 50%; display: flex; align-items: center; justify-content: center;
      font-size: 11px; font-weight: 700;
    }

    .main-content { flex: 1; }

    @media (max-width: 768px) {
      .navbar { flex-wrap: wrap; height: auto; padding: 12px; gap: 12px; }
      .nav-links { order: 3; width: 100%; justify-content: flex-start; }
      .nav-user { margin-left: 0; }
    }
  `]
})
export class AppComponent {
  constructor(public auth: AuthService) {}
  user = this.auth.currentUser;
  initials = computed(() => {
    const u = this.user()?.username ?? '';
    return u.slice(0, 2).toUpperCase();
  });
}
