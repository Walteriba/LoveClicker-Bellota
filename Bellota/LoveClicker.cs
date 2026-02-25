using Gma.System.MouseKeyHook;
using System.Runtime.InteropServices;

namespace Bellota
{
    public partial class LoveClicker : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left; public int Top; public int Right; public int Bottom;
        }

        static readonly IntPtr HWND_TOPMOST = new(-1);
        static readonly IntPtr HWND_BOTTOM = new(1);
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOACTIVATE = 0x0010;

        private IKeyboardMouseEvents _globalHook;
        private int _keyCount = 0;
        private readonly Random _random = new();
        private PictureBox _pictureBox;
        private System.Windows.Forms.Timer _fullscreenTimer;

        private const int ImageSize = 150;
        private const int FormSize = 300;

        private bool _dragging = false;
        private Point _dragCursorPoint;
        private Point _dragFormPoint;

        private string[] _images;
        private int _currentImageIndex = -1;

        public LoveClicker()
        {
            InitializeComponent();
            SetupMinimalistWindow();
            CreateUI();
            SubscribeToInput();
            EnableDrag(this);
            EnableDrag(_pictureBox!);
            StartFullscreenWatcher();
        }

        private void SetupMinimalistWindow()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.Size = new Size(FormSize, FormSize);
            this.StartPosition = FormStartPosition.Manual;
            this.DoubleBuffered = true;

            Color transparentColor = Color.LightPink;
            this.BackColor = transparentColor;
            this.TransparencyKey = transparentColor;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Rectangle workingArea = Screen.FromControl(this).WorkingArea;
            int margin = 5;
            this.Location = new Point(
                workingArea.Right - this.Width - margin,
                workingArea.Bottom - this.Height - margin
            );
        }

        private void CreateUI()
        {
            _pictureBox = new PictureBox
            {
                Size = new Size(ImageSize, ImageSize),
                Location = new Point(
                    (this.Width - ImageSize) / 2,
                    this.Height - ImageSize - 10
                ),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            _images =
            [
                "cherry_blossom1.gif",
                "cherry_blossom2.gif",
                "cherry_blossom3.gif",
                "cherry_blossom4.gif",
                "cherry_blossom5.gif"
            ];

            LoadImage();

            _pictureBox.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        ChangeImage();
                        return;
                    }

                    ShowLoveMessage("🤍");
                }

                if (e.Button == MouseButtons.Right)
                    this.Close();
            };

            this.Controls.Add(_pictureBox);
        }

        private void LoadImage()
        {
            if (_images == null || _images.Length == 0)
                return;

            if (_currentImageIndex == -1)
            {
                _currentImageIndex = _random.Next(_images.Length);
            }
            else
            {
                _currentImageIndex++;
                if (_currentImageIndex >= _images.Length)
                    _currentImageIndex = 0;
            }

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _images[_currentImageIndex]);

            _pictureBox.Image?.Dispose();
            _pictureBox.Image = Image.FromFile(path);
        }

        private void ChangeImage()
        {
            LoadImage();
        }

        private void SubscribeToInput()
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyPress += (s, e) =>
            {
                _keyCount++;
                BeatAnimation();

                if (_keyCount >= 50)
                {
                    ShowLoveMessage();
                    _keyCount = 0;
                }
            };
        }

        private void EnableDrag(Control control)
        {
            control.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    _dragging = true;
                    _dragCursorPoint = Cursor.Position;
                    _dragFormPoint = this.Location;
                }
            };
            control.MouseMove += (s, e) =>
            {
                if (_dragging)
                {
                    Point diff = Point.Subtract(Cursor.Position, new Size(_dragCursorPoint));
                    this.Location = Point.Add(_dragFormPoint, new Size(diff));
                }
            };
            control.MouseUp += (s, e) => _dragging = false;
        }

        private async void BeatAnimation()
        {
            int scale = 4;
            Size originalSize = new(ImageSize, ImageSize);
            Point originalLoc = new((this.Width - ImageSize) / 2, this.Height - ImageSize - 10);

            _pictureBox.Size = new Size(ImageSize + scale, ImageSize + scale);
            _pictureBox.Location = new Point(originalLoc.X - (scale / 2), originalLoc.Y - (scale / 2));

            await Task.Delay(160);

            _pictureBox.Size = originalSize;
            _pictureBox.Location = originalLoc;
        }

        private async void ShowLoveMessage(string manualText = "")
        {
            string[] mensajes = {
                "Te amo",
                "Tomá agüita",
                "Sos linda",
                "¡Ánimo!",
                "Sos mi lugar favorito",
                "Todo es mejor con vos",
                "Gracias por existir",
                "Sos increíble",
                "Me hacés muy feliz",
                "Estoy orgulloso de vos",
                "Siempre estoy para vos",
                "Sos luz",
                "Tu sonrisa es mi paz",
                "Qué suerte coincidir con vos",
                "Te pienso",
                "Te elegiría mil veces",
                "Sos mi persona favorita",
                "Me encantás",
                "Sos magia",
                "Vas re bien",
                "Confío en vos",
                "Sos más fuerte de lo que creés",
                "Tu esfuerzo vale muchísimo",
                "No te olvides de descansar",
                "Estoy con vos",
                "Hoy también cuenta",
                "Me encanta compartir la vida con vos",
                "Tu risa me arregla el día",
                "Te abrazo fuerte",
                "Siempre nosotros",
                "Sos arte",
                "Tu forma de ser es hermosa",
                "Sos única",
                "Me hacés bien",
                "Qué lindo es amarte",
                "Un besito virtual 💕",
                "Sos hermosa",
                "Mini pausa para sonreír", "Un mimo a la distancia",
                "Te extraño siempre",
                "Estoy orgulloso de quién sos",
                "Gracias por tanto",
                "Sos mi casualidad favorita",
                "Yo elijo quedarme a tu lado",
                "Sos mi lugar seguro",
                "Me encanta tu forma de ser",
                "¿Ya me enviaste mi fotito diaria?"
            };

            string texto = string.IsNullOrEmpty(manualText) ? mensajes[_random.Next(mensajes.Length)] : manualText;

            Label msg = new()
            {
                Text = texto,
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold | FontStyle.Italic),
                BackColor = Color.Transparent
            };

            this.Controls.Add(msg);
            msg.BringToFront();
            msg.PerformLayout();

            msg.Left = _pictureBox.Left + (_pictureBox.Width / 2) - (msg.Width / 2);
            msg.Top = _pictureBox.Top - msg.Height - 5;

            for (int i = 0; i < 30; i++)
            {
                msg.Top -= 2;
                if (i > 15)
                {
                    int alpha = Math.Max(0, 255 - ((i - 15) * 17));
                    msg.ForeColor = Color.FromArgb(alpha, msg.ForeColor);
                }
                await Task.Delay(40);
            }

            this.Controls.Remove(msg);
            msg.Dispose();
        }

        private void StartFullscreenWatcher()
        {
            _fullscreenTimer = new System.Windows.Forms.Timer { Interval = 1500 };
            _fullscreenTimer.Tick += (s, e) => CheckFullscreen();
            _fullscreenTimer.Start();
        }

        private void CheckFullscreen()
        {
            IntPtr handle = GetForegroundWindow();
            if (handle == IntPtr.Zero || handle == this.Handle) return;

            GetWindowRect(handle, out RECT rect);
            Rectangle windowRect = new(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            Rectangle screenRect = Screen.PrimaryScreen!.Bounds;

            bool isFullscreen = windowRect.Width >= screenRect.Width && windowRect.Height >= screenRect.Height;

            if (isFullscreen)
            {
                SetWindowPos(
                    this.Handle,
                    HWND_BOTTOM,
                    0, 0, 0, 0,
                    SWP_NOMOVE |
                    SWP_NOSIZE |
                    SWP_NOACTIVATE);

                this.TopMost = false;
            }
            else
            {
                SetWindowPos(
                    this.Handle,
                    HWND_TOPMOST,
                    0, 0, 0, 0,
                    SWP_NOMOVE |
                    SWP_NOSIZE |
                    SWP_NOACTIVATE);

                this.TopMost = true;
            }
        }

        protected override bool ShowWithoutActivation => true;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result =
                    MessageBox.Show(
                        "¿Querés cerrarlo o lo dejamos cuidándote un ratito más?\n\nTe amo 💕",
                        "Mensaje importante para mi Bellota",
                        MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            _globalHook?.Dispose();
            base.OnFormClosing(e);
        }
    }
}