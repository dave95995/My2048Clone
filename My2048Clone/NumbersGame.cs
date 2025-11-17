namespace My2048Clone
{
	internal class NumbersGame
	{

		/*
		 * Unlike the common approach, I did not use the typical "rotate-left and reuse the same move logic" technique. 
		 * Instead, I implemented individual movement and merge logic for each direction. 
		 * One reason for this choice was that I wanted to record actions in a clear and structured way, 
		 * making it easier to support animations and other visual effects in future development.
		 * 
		 */

		public enum Direction
		{ Left, Right, Up, Down };

		private record MoveRecord(
			Action<int, int, int> Move,
			Action<int, int, int> Merge,
			int Start, int End, int Step);

		private readonly int[,] _grid;
		private readonly int Size = 4;
		private readonly Random _random = new Random();
		private NumbersGame numbersGame;

		public NumbersGame()
		{
			_grid = new int[Size, Size];
			AddNewTile();
		}

		private NumbersGame(NumbersGame from)
		{
			_grid = (int[,])from._grid.Clone();
		}

		public List<Direction> GetValidMoves()
		{
			List<Direction> moves = [];

			foreach (var dir in Enum.GetValues<Direction>())
			{
				NumbersGame tempGame = new NumbersGame(this);
				tempGame.Move(dir);

				if (EqualGrid(_grid, tempGame._grid) == false)
				{
					moves.Add(dir);
				}
			}
			return moves;
		}

		private bool EqualGrid(int[,] a, int[,] b)
		{
			for (int y = 0; y < Size; y++)
			{
				for (int x = 0; x < Size; x++)
				{
					if (a[y, x] != b[y, x])
						return false;
				}
			}
			return true;
		}

		public int GetValueAt(int index)
		{
			if (index >= 0 && index < Size * Size)
			{
				return _grid[index / Size, index % Size];
			}
			else
			{
				throw new ArgumentException("Invalid index");
			}
		}

		public void Move(Direction direction)
		{
			MoveRecord movement;
			switch (direction)
			{
				case Direction.Left:
					movement = new MoveRecord(MoveHorizontal, MergeHorizontal, 0, Size, 1);
					break;

				case Direction.Right:
					movement = new MoveRecord(MoveHorizontal, MergeHorizontal, Size - 1, -1, -1);
					break;

				case Direction.Up:
					movement = new MoveRecord(MoveVertical, MergeVertical, 0, Size, 1);
					break;

				default:
					movement = new MoveRecord(MoveVertical, MergeVertical, Size - 1, -1, -1);
					break;
			}

			movement.Move(movement.Start, movement.End, movement.Step);
			movement.Merge(movement.Start, movement.End - movement.Step, movement.Step);
			movement.Move(movement.Start, movement.End, movement.Step);
		}

		public void AddNewTile()
		{
			List<int> empty = GetEmptyCells();

			if (empty.Count == 0) return;

			// 90% chance of 2 and 10% chance of 4
			int value = _random.Next(100) < 90 ? 2 : 4;
			int index = _random.Next(empty.Count);
			int pos = empty[index];

			int x = pos % Size;
			int y = pos / Size;
			_grid[y, x] = value;
		}

		private List<int> GetEmptyCells()
		{
			List<int> empty = [];
			for (int i = 0; i < Size * Size; i++)
			{
				if (GetValueAt(i) == 0)
					empty.Add(i);
			}
			return empty;
		}

		public bool HasWon()
		{
			for (int i = 0; i < Size * Size; i++)
			{
				if (GetValueAt(i) == 2048)
					return true;
			}
			return false;
		}

		private void MoveVertical(int y0, int stopY, int stepY)
		{
			for (int x = 0; x < Size; x++)
			{
				// move all non empty tiles ot the same side
				int j = y0;
				for (int y = y0; y != stopY; y += stepY)
				{
					if (_grid[y, x] != 0)
					{
						_grid[j, x] = _grid[y, x];
						j += stepY;
					}
				}
				// fill the remaining with zeros
				for (int y = j; y != stopY; y += stepY)
				{
					_grid[y, x] = 0;
				}
			}
		}

		private void MoveHorizontal(int x0, int stopX, int stepX)
		{
			for (int y = 0; y < Size; y++)
			{
				// move all non empty tiles ot the same side
				int j = x0;
				for (int x = x0; x != stopX; x += stepX)
				{
					if (_grid[y, x] != 0)
					{
						_grid[y, j] = _grid[y, x];
						j += stepX;
					}
				}
				// fill the remaining with zeros
				for (int x = j; x != stopX; x += stepX)
				{
					_grid[y, x] = 0;
				}
			}
		}

		private void MergeVertical(int y0, int stopY, int stepY)
		{
			for (int x = 0; x < Size; x++)
			{
				// if we moved up then merge from top to bottom otherwise bottom to top
				for (int y = y0; y != stopY; y += stepY)
				{
					if (_grid[y, x] > 0 && _grid[y, x] == _grid[y + stepY, x])
					{
						_grid[y, x] *= 2;
						_grid[y + stepY, x] = 0;
					}
				}
			}
		}

		private void MergeHorizontal(int x0, int stopX, int stepX)
		{
			for (int y = 0; y < Size; y++)
			{
				// if we moved left then merge from left to right otherwise right to left
				for (int x = x0; x != stopX; x += stepX)
				{
					if (_grid[y, x] > 0 && _grid[y, x] == _grid[y, x + stepX])
					{
						_grid[y, x] *= 2;
						_grid[y, x + stepX] = 0;
					}
				}
			}
		}

		public void Display()
		{
			for (int y = 0; y < Size; y++)
			{
				for (int x = 0; x < Size; x++)
				{
					string letter = _grid[y, x] == 0 ? "- " : _grid[y, x] + " ";
					Console.Write(letter.PadLeft(4));
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}
	}
}