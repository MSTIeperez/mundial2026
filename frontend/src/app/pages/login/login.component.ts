import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="login-shell">
      <div class="login-bg"></div>
      <div class="login-card card">

        <div class="login-header">
          <div class="trophy-big">🏆</div>
          <h1>MUNDIAL <span class="text-gold">2026</span></h1>
          <p class="text-muted">Fixture Predictor — USA · CAN · MEX</p>
        </div>

        <div class="tab-bar">
          <button class="tab-btn" [class.active]="mode() === 'login'" (click)="mode.set('login')">
            Iniciar Sesión
          </button>
          <button class="tab-btn" [class.active]="mode() === 'register'" (click)="mode.set('register')">
            Registrarse
          </button>
        </div>

        @if (error()) {
          <div class="error-box">⚠️ {{ error() }}</div>
        }

        @if (mode() === 'login') {
          <div class="form-group">
            <label>Usuario o correo</label>
            <input class="form-control" [(ngModel)]="username" placeholder="usuario o email" (keyup.enter)="submit()">
          </div>
          <div class="form-group">
            <label>Contraseña</label>
            <input class="form-control" type="password" [(ngModel)]="password" placeholder="••••••••" (keyup.enter)="submit()">
          </div>
          <button class="btn btn-primary" style="width:100%" [disabled]="loading()" (click)="submit()">
            {{ loading() ? 'Ingresando...' : 'Entrar al Fixture' }}
          </button>
          <p class="hint-text">Admin por defecto: <code>admin</code> / <code>Admin123!</code></p>
        } @else {
          <div class="form-group">
            <label>Nombre de usuario</label>
            <input class="form-control" [(ngModel)]="username" placeholder="mi_usuario">
          </div>
          <div class="form-group">
            <label>Correo electrónico</label>
            <input class="form-control" type="email" [(ngModel)]="email" placeholder="correo@ejemplo.com">
          </div>
          <div class="form-group">
            <label>Contraseña</label>
            <input class="form-control" type="password" [(ngModel)]="password" placeholder="Mínimo 8 caracteres">
          </div>
          <button class="btn btn-primary" style="width:100%" [disabled]="loading()" (click)="submit()">
            {{ loading() ? 'Creando cuenta...' : 'Crear Cuenta' }}
          </button>
        }

      </div>
    </div>
  `,
  styles: [`
    .login-shell {
      min-height: 100vh;
      display: flex; align-items: center; justify-content: center;
      position: relative; overflow: hidden;
      background: var(--c-bg);
    }
    .login-bg {
      position: absolute; inset: 0;
      background:
        radial-gradient(ellipse at 20% 50%, rgba(200,162,39,.12) 0%, transparent 60%),
        radial-gradient(ellipse at 80% 50%, rgba(230,57,70,.08) 0%, transparent 60%);
    }
    .login-card {
      position: relative; z-index: 1;
      width: 100%; max-width: 400px;
      padding: 40px;
      animation: fadeUp .4s ease;
    }
    @keyframes fadeUp {
      from { opacity: 0; transform: translateY(20px); }
      to   { opacity: 1; transform: translateY(0); }
    }
    .login-header { text-align: center; margin-bottom: 28px; }
    .trophy-big { font-size: 52px; margin-bottom: 8px; }
    .login-header h1 { font-size: 36px; margin-bottom: 4px; }

    .tab-bar {
      display: flex; background: var(--c-surface);
      border-radius: var(--radius); padding: 4px;
      margin-bottom: 24px;
    }
    .tab-btn {
      flex: 1; padding: 8px; border: none; background: none;
      color: var(--c-muted); border-radius: calc(var(--radius) - 2px);
      cursor: pointer; font-weight: 600; font-size: 13px;
      transition: all .2s;
    }
    .tab-btn.active { background: var(--c-accent); color: #0a0e1a; }

    .error-box {
      background: rgba(230,57,70,.1); border: 1px solid var(--c-accent2);
      border-radius: var(--radius); padding: 10px 14px;
      color: var(--c-accent2); font-size: 13px; margin-bottom: 16px;
    }
    .hint-text {
      text-align: center; margin-top: 12px;
      font-size: 12px; color: var(--c-muted);
    }
    code { background: var(--c-surface); padding: 1px 5px; border-radius: 4px; }
  `]
})
export class LoginComponent {
  mode = signal<'login' | 'register'>('login');
  loading = signal(false);
  error = signal('');

  username = '';
  email = '';
  password = '';

  constructor(private auth: AuthService, private router: Router) {}

  submit() {
    this.error.set('');
    if (!this.username || !this.password) {
      this.error.set('Completa todos los campos');
      return;
    }
    this.loading.set(true);

    const obs = this.mode() === 'login'
      ? this.auth.login(this.username, this.password)
      : this.auth.register(this.username, this.email, this.password);

    obs.subscribe({
      next: () => this.router.navigate(['/grupos']),
      error: (e) => {
        this.error.set(e.error?.error ?? 'Error de conexión');
        this.loading.set(false);
      },
      complete: () => this.loading.set(false)
    });
  }
}
