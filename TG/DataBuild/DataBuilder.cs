using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.DataBuild
{
	public class DataBuilder
	{


		public static Matrix<T> BuildMatrix3<T>(string file)
		{
			Matrix<T> matrix = new Matrix<T>();
			var lines = File.ReadLines(file);
			foreach (var line in lines)
			{
				string[] info = line.Split(' ');

				int row = Int32.Parse(info[0]);
				int column = Int32.Parse(info[1]);
				T value = (T)Convert.ChangeType(info[2], typeof(T));

				matrix[row, column] = value;
			}

			return matrix;
		}

		public static Matrix<T> BuildMatrix2<T>(string file)
		{
			Matrix<T> matrix = new Matrix<T>();
			using (StreamReader sr = new StreamReader(file))
			{
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();
					string[] info = line.Split(' ');

					int row = Int32.Parse(info[0]);
					int column = Int32.Parse(info[1]);
					T value = (T) Convert.ChangeType(info[2], typeof(T));					

					matrix[row, column] = value;
				}
			}

			return matrix;
		}

		public static Dictionary<int, Dictionary<int, float>> BuildMatrix(string file)
		{
			Dictionary<int, Dictionary<int, float>> matrix = new Dictionary<int, Dictionary<int, float>>();
			using (StreamReader sr = new StreamReader(file))
			{
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();
					string[] info = line.Split(' ');

					int row = Int32.Parse(info[0]);
					int column = Int32.Parse(info[1]);
					int value = Int32.Parse(info[2]);

					if (!matrix.ContainsKey(row))
						matrix[row] = new Dictionary<int, float>();

					matrix[row][column] = value;
				}
			}

			return matrix;
		}

		public static SparseMatrix<float> BuildSparseMatrix(string file)
		{
			SparseMatrix<float> matrix = new SparseMatrix<float>();
			using (StreamReader sr = new StreamReader(file))
			{
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();
					string[] info = line.Split(' ');

					int row = Int32.Parse(info[0]);
					int column = Int32.Parse(info[1]);
					int value = Int32.Parse(info[2]);

					matrix.SetAt(row, column, value);
				}
			}

			return matrix;
		}

	}
}
