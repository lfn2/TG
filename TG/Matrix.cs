using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG
{
	[Serializable()]
	public class Matrix<T>
	{
		private Dictionary<int, Dictionary<int, T>> _Matrix;

		public Matrix()
		{
			_Matrix = new Dictionary<int, Dictionary<int, T>>();
		}

		public T this[int row, int col]
		{
			get
			{
				return _Matrix[row][col];
			}
			set
			{
				if (!_Matrix.ContainsKey(row))
					_Matrix[row] = new Dictionary<int, T>();

				_Matrix[row][col] = value;
			}
		}

		public void Set(int row, int col, T value)
		{
			if (!_Matrix.ContainsKey(row))
				_Matrix[row] = new Dictionary<int, T>();

			_Matrix[row][col] = value;
		}

		public T Get(int row, int col)
		{
			return _Matrix[row][col];
		}

		public IEnumerable<int> GetRowColumns(int row)
		{
			return _Matrix[row].Keys;
		}

		public IEnumerable<int> this[int row]
		{
			get
			{
				return _Matrix[row].Keys;
			}
		}

		public IEnumerable<int> Rows
		{
			get
			{
				return _Matrix.Keys;
			}
		}

	}
}
