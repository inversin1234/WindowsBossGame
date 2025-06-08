// BossWindowGame – WinForms mini‑game, 10 fases, 60 s total
// C# 7.3  ·  .NET 4.7.2  –  COMPILABLE SIN ERRORES
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace BossWindowGame
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    //=======================================================================
    //  MAIN WINDOW (BOSS)
    //=======================================================================
    public partial class MainForm : Form
    {
        // CONFIG
        private const int FPS = 60, FRAME_MS = 1000 / FPS;
        private const int GAME_MS = 60_000, PHASES = 10, PHASE_MS = GAME_MS / PHASES;
        private const int BOSS_VEL = 9; // velocidad del jefe aumentada

        private readonly Timer _loop = new Timer();
        private readonly Label _hud = new Label();
        private readonly Stopwatch _sw = Stopwatch.StartNew();
        private readonly Random _rng = new Random();
        private readonly List<IEnemy> _enemies = new List<IEnemy>();
        private readonly List<IntPtr> _min = new List<IntPtr>();

        private int _dir = 1, _score;
        private int _lives = 3;  // sistema de vidas
        private bool _boostDone;
        private int cdChaser, cdFast, cdZig, cdRain, cdLaser;

        public MainForm()
        {
            Text = "BOSS WINDOW";
            BackColor = Color.Black;
            ForeColor = Color.Lime;
            FormBorderStyle = FormBorderStyle.None; // borderless for cleaner look
            StartPosition = FormStartPosition.Manual;
            Size = new Size(420, 120);
            TopMost = true;
            DoubleBuffered = true;

            _hud.Dock = DockStyle.Bottom;
            _hud.Height = 26;
            _hud.Font = new Font("Consolas", 11, FontStyle.Bold);
            _hud.ForeColor = Color.Lime;
            _hud.TextAlign = ContentAlignment.MiddleRight;
            Controls.Add(_hud);
            // rounded boss window corners
            this.Load += (s, e) => EnemyForm.ApplyRoundCorners(this, 20);

            Shown += (s, e) => { Intro(); MinimizeAll(); StartLoop(); };
        }

        private void Intro()
        {
            string[] lines = {
                "F1  0‑6s  : Chaser",
                "F2  6‑12s : +Fast",
                "F3 12‑18s : +ZigZag",
                "F4 18‑24s : +Rain",
                "F5 24‑30s : +Laser (Y cursor)",
                "F6 30‑36s : Velocidad ×1.2",
                "F7 36‑42s : Spam moderado",
                "F8 42‑48s : Doble láser",
                "F9 48‑54s : Spam intenso",
                "F10 54‑60s: Triple láser & caos",
                "",
                "¡Sobrevive 60 s para ganar!"};
            MessageBox.Show(string.Join(Environment.NewLine, lines), "Fases",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MinimizeAll()
        {
            IntPtr me = Handle;
            Native.EnumWindows((h, l) =>
            {
                if (h != me && Native.IsWindowVisible(h)) { _min.Add(h); Native.ShowWindow(h, Native.SW_MINIMIZE); }
                return true;
            }, IntPtr.Zero);
            BringToFront(); Activate();
        }

        private void StartLoop()
        {
            Left = (Screen.PrimaryScreen.Bounds.Width - Width) / 2;
            Top = 0;
            _loop.Interval = FRAME_MS;
            _loop.Tick += Tick;
            _loop.Start();
        }

        private void Tick(object sender, EventArgs e)
        {
            int phase = Math.Min(PHASES - 1, (int)(_sw.ElapsedMilliseconds / PHASE_MS));

            MoveBoss();
            Spawns(phase);
            UpdateEnemies();
            Hud(phase);
        }

        private void MoveBoss()
        {
            int nx = Left + _dir * BOSS_VEL;
            if (nx < 0) { nx = 0; _dir = 1; }
            if (nx + Width > Screen.PrimaryScreen.Bounds.Width) { nx = Screen.PrimaryScreen.Bounds.Width - Width; _dir = -1; }
            Left = nx;
        }

        private void Spawns(int p)
        {
            cdChaser += FRAME_MS; if (cdChaser >= 600) { Spawn(new ChaserEnemy(p)); cdChaser = 0; }
            if (p >= 1) { cdFast += FRAME_MS; if (cdFast >= 1000) { Spawn(new FastEnemy(p)); cdFast = 0; } }
            if (p >= 2) { cdZig += FRAME_MS; if (cdZig >= 1400) { Spawn(new ZigZagEnemy(p)); cdZig = 0; } }
            if (p >= 3) { cdRain += FRAME_MS; if (cdRain >= 1800) { Spawn(new RainEnemy(p)); cdRain = 0; } }
            if (p >= 4) { cdLaser += FRAME_MS; if (cdLaser >= 1500) { Spawn(new LaserBeam(p, Cursor.Position.Y)); cdLaser = 0; } }

            if (p == 4 && !_boostDone) { foreach (var e in _enemies) e.Boost(1.4f); _boostDone = true; }
            else if (p == 6)
            {
                if (_rng.NextDouble() < 0.10) Spawn(new ChaserEnemy(p));
                if (_rng.NextDouble() < 0.08) Spawn(new FastEnemy(p));
            }
            else if (p == 7)
            {
                Spawn(new LaserBeam(p, Cursor.Position.Y - 25));
                Spawn(new LaserBeam(p, Cursor.Position.Y + 25));
            }
            else if (p == 8)
            {
                if (_rng.NextDouble() < 0.18) Spawn(new ChaserEnemy(p));
                if (_rng.NextDouble() < 0.14) Spawn(new FastEnemy(p));
                if (_rng.NextDouble() < 0.14) Spawn(new ZigZagEnemy(p));
                if (_rng.NextDouble() < 0.10) Spawn(new RainEnemy(p));
            }
            else if (p == 9)
            {
                Spawn(new ChaserEnemy(p));
                Spawn(new FastEnemy(p));
                Spawn(new ZigZagEnemy(p));
                Spawn(new RainEnemy(p));
                Spawn(new LaserBeam(p, Cursor.Position.Y - 30));
                Spawn(new LaserBeam(p, Cursor.Position.Y));
                Spawn(new LaserBeam(p, Cursor.Position.Y + 30));
            }
        }

        private void Spawn(IEnemy e) { e.Spawn(this); _enemies.Add(e); }

        private void UpdateEnemies()
        {
            Point cur = Cursor.Position;
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                if (_enemies[i].Update(cur, FRAME_MS)) { _enemies[i].Dispose(); _enemies.RemoveAt(i); continue; }
                if (_enemies[i].Bounds.Contains(cur))
                {
                    _lives--;
                    _enemies[i].Dispose();
                    _enemies.RemoveAt(i);
                    if (_lives <= 0)
                    {
                        End(false);
                        return;
                    }
                    else
                    {
                        // breve indicación visual opcional; simplemente continúa
                        continue;
                    }
                }
            }
        }

        private void Hud(int phase)
        {
            int left = GAME_MS - (int)_sw.ElapsedMilliseconds;
            if (left <= 0) { End(true); return; }
            _score += FRAME_MS / 40;
            _hud.Text = $"Fase {phase + 1}/10   Tiempo {left / 1000}s   Vidas {_lives}   Score {_score}";
        }

        private void End(bool win)
        {
            _loop.Stop();
            foreach (var e in _enemies) e.Dispose();
            foreach (var h in _min) Native.ShowWindow(h, Native.SW_RESTORE);

            if (win)
            {
                MessageBox.Show("¡VICTORIA!", "Fin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                ShowFailFlood(); // inicia el flood de ventanitas
            }
        }

        private void ShowFailFlood()
        {
            for (int i = 0; i < 50; i++)
            {
                new FailForm().Show();
            }
        }

        private static class Native
        {
            public const int SW_MINIMIZE = 6, SW_RESTORE = 9;
            public delegate bool EnumWindowsProc(IntPtr h, IntPtr l);
            [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern bool EnumWindows(EnumWindowsProc cb, IntPtr lp);
            [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr h);
            [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr h, int cmd);
        }
    }

    //=======================================================================
    //  ENEMY BASE + INTERFACE
    //=======================================================================
    interface IEnemy : IDisposable
    {
        void Spawn(Form boss);
        bool Update(Point cursor, int dt);
        Rectangle Bounds { get; }
        void Boost(float factor);
    }

    abstract class EnemyForm : Form, IEnemy
    {
        protected int Life;
        protected float Speed;
        protected readonly Stopwatch Clock = Stopwatch.StartNew();
        public new Rectangle Bounds => new Rectangle(Left, Top, Width, Height);

        protected EnemyForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            DoubleBuffered = true;
            TopMost = true;
            Load += (s, e) => ApplyRoundCorners(this, 18);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (LinearGradientBrush br = new LinearGradientBrush(ClientRectangle,
                      Color.FromArgb(200, BackColor), BackColor, LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(br, ClientRectangle);
            }
        }

        public static void ApplyRoundCorners(Form frm, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
            path.AddArc(new Rectangle(frm.Width - radius, 0, radius, radius), -90, 90);
            path.AddArc(new Rectangle(frm.Width - radius, frm.Height - radius, radius, radius), 0, 90);
            path.AddArc(new Rectangle(0, frm.Height - radius, radius, radius), 90, 90);
            path.CloseFigure();
            frm.Region = new Region(path);
        }

        public virtual void Spawn(Form boss)
        {
            StartPosition = FormStartPosition.Manual;
            Location = new Point(boss.Left + (boss.Width - Width) / 2, boss.Top + boss.Height);
            Show();
        }

        public abstract bool Update(Point cursor, int dt);
        public virtual void Boost(float factor) => Speed *= factor;
    }

    //-----------------------------------------------------------------------
    //  CHASER
    //-----------------------------------------------------------------------
    class ChaserEnemy : EnemyForm
    {
        public ChaserEnemy(int ph)
        {
            Life = 5000;
            Speed = 6 + ph * 1.5f;
            Size = new Size(110, 60);
            BackColor = Color.DarkRed;
            Controls.Add(new Label { Text = "CHASE", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.White });
        }
        public override bool Update(Point c, int dt)
        {
            if (Clock.ElapsedMilliseconds > Life) return true;
            int dx = c.X - (Left + Width / 2), dy = c.Y - (Top + Height / 2);
            double d = Math.Sqrt(dx * dx + dy * dy); if (d == 0) return false;
            Left += (int)(Speed * dx / d);
            Top += (int)(Speed * dy / d);
            return false;
        }
    }

    //-----------------------------------------------------------------------
    //  FAST
    //-----------------------------------------------------------------------
    class FastEnemy : EnemyForm
    {
        private PointF dir = PointF.Empty;
        public FastEnemy(int ph)
        {
            Life = 3500;
            Speed = 10 + ph * 3;
            Size = new Size(100, 55);
            BackColor = Color.Gold;
            Controls.Add(new Label { Text = "FAST", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Black });
        }
        public override bool Update(Point c, int dt)
        {
            if (Clock.ElapsedMilliseconds > Life) return true;
            if (dir.Equals(PointF.Empty))
            {
                int dx = c.X - (Left + Width / 2), dy = c.Y - (Top + Height / 2);
                double d = Math.Sqrt(dx * dx + dy * dy);
                if (d != 0) dir = new PointF((float)(dx / d), (float)(dy / d));
            }
            Left += (int)(dir.X * Speed);
            Top += (int)(dir.Y * Speed);
            return false;
        }
    }

    //-----------------------------------------------------------------------
    //  ZIGZAG
    //-----------------------------------------------------------------------
    class ZigZagEnemy : EnemyForm
    {
        private double ang;
        public ZigZagEnemy(int ph)
        {
            Life = 6000;
            Speed = 5 + ph * 2;
            Size = new Size(120, 70);
            BackColor = Color.MediumPurple;
            Controls.Add(new Label { Text = "ZIG", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.White });
        }
        public override bool Update(Point c, int dt)
        {
            if (Clock.ElapsedMilliseconds > Life) return true;
            ang += 0.3;
            int dx = c.X - (Left + Width / 2), dy = c.Y - (Top + Height / 2);
            double d = Math.Sqrt(dx * dx + dy * dy); if (d == 0) return false;
            double vx = dx / d, vy = dy / d;
            double px = -vy, py = vx;
            double off = Math.Sin(ang) * 10;
            Left += (int)Math.Round(vx * Speed + px * off * 0.3);
            Top += (int)Math.Round(vy * Speed + py * off * 0.3);
            return false;
        }
    }

    //-----------------------------------------------------------------------
    //  RAIN
    //-----------------------------------------------------------------------
    class RainEnemy : EnemyForm
    {
        private static readonly Random rng = new Random();
        public RainEnemy(int ph)
        {
            Life = 4000;
            Speed = 8 + ph * 2;
            Size = new Size(100, 60);
            BackColor = Color.CadetBlue;
            Controls.Add(new Label { Text = "RAIN", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.White });
            StartPosition = FormStartPosition.Manual;
            Left = rng.Next(0, Screen.PrimaryScreen.Bounds.Width - Width);
            Top = -Height;
            Show();
        }
        public override void Spawn(Form boss) { /* ya posicionada */ }
        public override bool Update(Point c, int dt)
        {
            if (Clock.ElapsedMilliseconds > Life) return true;
            Top += (int)Speed;
            return false;
        }
    }

    //-----------------------------------------------------------------------
    //  LASER
    //-----------------------------------------------------------------------
    class LaserBeam : EnemyForm
    {
        private readonly int dir;
        private readonly int scrW = Screen.PrimaryScreen.Bounds.Width;
        public LaserBeam(int ph, int y)
        {
            Life = 4000;
            Speed = 12 + ph * 4;
            Height = 12;
            Width = scrW;
            BackColor = Color.LimeGreen;
            dir = (_rngStatic.Next(0, 2) == 0) ? 1 : -1;
            StartPosition = FormStartPosition.Manual;
            Top = y - Height / 2;
            Left = (dir == 1) ? -scrW : scrW;
            Show();
        }
        private static readonly Random _rngStatic = new Random();
        public override void Spawn(Form boss) { /* ya */ }
        public override bool Update(Point c, int dt)
        {
            if (Clock.ElapsedMilliseconds > Life) return true;
            Left += (int)(Speed * dir);
            return false;
        }
    }
    //--------------------------------------------------------------------
    // FAIL FORM (ventanita roja que se mueve)
    //--------------------------------------------------------------------
    class FailForm : Form
    {
        private readonly Timer _tm = new Timer();
        private static readonly Random _rnd = new Random();
        public FailForm()
        {
            Size = new Size(180, 60);
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            BackColor = Color.DarkRed;
            Controls.Add(new Label { Text = "perdiste pinche pendejo xddddd", Dock = DockStyle.Fill, ForeColor = Color.White, Font = new Font("Consolas", 16, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter });

            var b = Screen.PrimaryScreen.Bounds;
            StartPosition = FormStartPosition.Manual;
            Left = _rnd.Next(b.Width - Width);
            Top = _rnd.Next(b.Height - Height);

            _tm.Interval = 30;
            _tm.Tick += (s, e) => { Left += _rnd.Next(-5, 6); Top += _rnd.Next(-5, 6); };
            _tm.Start();
        }
    }
}
