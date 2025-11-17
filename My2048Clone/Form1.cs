using static My2048Clone.NumbersGame;

namespace My2048Clone
{
	internal class BoardGraphics
	{
		private Rectangle _rectangle;
		private readonly int _borderWidth;
		private readonly int _cellSize;
		private readonly Size _borderRadius;
		private readonly Rectangle[] _cellRectangles = new Rectangle[16];

		private readonly Brush _backcolorBrush = new SolidBrush(Color.FromArgb(155, 136, 120));
		private readonly Brush _emptyCellBrush = new SolidBrush(Color.FromArgb(189, 172, 151));

		private readonly Dictionary<int, Color> _tileBackground = new()
		{
			{2, Color.FromArgb(238, 228, 218)},
			{4, Color.FromArgb(237, 224, 200)},
			{8, Color.FromArgb(242, 177, 121)},
			{16, Color.FromArgb(245, 149, 99)},
			{32, Color.FromArgb(246, 124, 95)},
			{64, Color.FromArgb(246, 94, 59)},
			{128, Color.FromArgb(237, 207, 114)},
			{256, Color.FromArgb(237, 204, 97)},
			{512, Color.FromArgb(237, 200, 80)},
			{1024, Color.FromArgb(237, 197, 63)},
			{2048, Color.FromArgb(237, 194, 46)}
		};

		public BoardGraphics(int x, int y, int size, int borderWidth)
		{
			_rectangle = new Rectangle(x, y, size, size);
			_borderWidth = borderWidth;

			// we have 3 borders between and 2 och each side
			_cellSize = (size - (borderWidth * 5)) / 4;

			// used for the rounded corners
			_borderRadius = new(_cellSize / 4, _cellSize / 4);

			// Rectangle for the 4x4 grid
			for (int j = 0; j < 4; j++)
			{
				for (int i = 0; i < 4; i++)
				{
					int index = j * 4 + i;
					int x0 = _rectangle.X + _borderWidth + i * (_cellSize + _borderWidth);
					int y0 = _rectangle.Y + _borderWidth + j * (_cellSize + _borderWidth);
					_cellRectangles[index] = new Rectangle(x0, y0, _cellSize, _cellSize);
				}
			}
		}

		public void DrawBackground(Graphics g)
		{
			g.FillRoundedRectangle(_backcolorBrush, _rectangle, _borderRadius);
			foreach (var rect in _cellRectangles)
			{
				g.FillRoundedRectangle(_emptyCellBrush, rect, _borderRadius);
			}
		}

		public void DrawTile(Graphics g, int index, int value)
		{
			Rectangle tileRect = _cellRectangles[index];
			Color textColor = value <= 4 ? Color.FromArgb(119, 110, 101) : Color.FromArgb(249, 246, 242);
			var textSize = value > 64 ? 28 : 36;

			// if we dont end the game at 2048
			Color bgColor = value <= 2048 ? _tileBackground[value] : _tileBackground[2048];

			var brush = new SolidBrush(bgColor);
			g.FillRoundedRectangle(brush, tileRect, _borderRadius);

			var textBrush = new SolidBrush(textColor);
			var font = new Font("Segoe UI", textSize, FontStyle.Bold);

			// align horizontal and vertical
			var format = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};

			g.DrawString(value.ToString(), font, textBrush, tileRect, format);
		}
	}

	public partial class Form1 : Form
	{
		private readonly BoardGraphics _boardGraphics;

		private NumbersGame _game;

		public Form1()
		{
			InitializeComponent();
			Text = "2048 Clone";
			_game = new();
			_boardGraphics = new BoardGraphics(0, 0, 500, 10);
			BackColor = Color.FromArgb(247, 244, 234);
			Size = new(515, 535);
			Paint += Form1_Paint;
			DoubleBuffered = true;
			KeyDown += Form1_KeyDown;
		}

		private void OnMove(NumbersGame.Direction moveDirection)
		{
			if (_game.HasWon())
			{
				MessageBox.Show("You won!", "Congratulation");
				return;
			}

			List<Direction> moves = _game.GetValidMoves();

			if (moves.Count > 0)
			{
				if (moves.Contains(moveDirection))
				{
					_game.Move(moveDirection);
					_game.AddNewTile();
				}
			}
			else
			{
				MessageBox.Show("Game is over", "Fail");
				return;
			}
		}

		private void Form1_KeyDown(object? sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Up:
					OnMove(NumbersGame.Direction.Up);
					break;

				case Keys.Down:
					OnMove(NumbersGame.Direction.Down);
					break;

				case Keys.Left:
					OnMove(NumbersGame.Direction.Left);
					break;

				case Keys.Right:
					OnMove(NumbersGame.Direction.Right);
					break;
			}

			Invalidate();
		}

		private void Form1_Paint(object? sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			_boardGraphics.DrawBackground(g);
			for (int i = 0; i < 4 * 4; i++)
			{
				int value = _game.GetValueAt(i);
				if (value != 0)
				{
					_boardGraphics.DrawTile(g, i, value);
				}
			}
		}
	}
}