using System;
using System.Collections.Generic;
using System.IO;
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

		public Matrix(string filePath)
			: this()
		{
			ReadFromFile(filePath);
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

		public IEnumerable<int> this[int row]
		{
			get
			{
				return _Matrix[row].Keys;
			}
		}

		public void Set(int row, int col, T value)
		{
			if (!_Matrix.ContainsKey(row))
				_Matrix[row] = new Dictionary<int, T>();

			_Matrix[row][col] = value;
		}

		public void remove(int row, int col)
		{
			if (_Matrix.ContainsKey(row))
				if (_Matrix[row].ContainsKey(col))
					_Matrix[row].Remove(col);
		}

		public T Get(int row, int col)
		{
			return _Matrix[row][col];
		}

		public IEnumerable<int> GetRowColumns(int row)
		{
			return _Matrix[row].Keys;
		}

		public IEnumerable<int> Rows
		{
			get
			{
				return _Matrix.Keys;
			}
		}

		private void ReadFromFile(string filePath)
		{			
			var lines = File.ReadLines(filePath);
			foreach (var line in lines)
			{
				string[] info = line.Split(' ');

				int row = Int32.Parse(info[0]);
				int column = Int32.Parse(info[1]);
				T value = (T)Convert.ChangeType(info[2], typeof(T));

				this[row, column] = value;
			}
		}

		public void WriteToFile(string filePath)
		{
			StringBuilder sb = new StringBuilder();
			using (StreamWriter sw = new StreamWriter(filePath))
			{
				foreach (int user in this.Rows)
					foreach (int trustedUser in this[user])
					{
						sb.Append(user);
						sb.Append(" ");
						sb.Append(trustedUser);
						sb.Append(" ");
						sb.Append(this[user, trustedUser]);
						sb.AppendLine();
					}

				sw.Write(sb);
			}
		}

	}
}
