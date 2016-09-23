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

		public static SparseMatrix<float> BuildMatrix2(string file)
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
