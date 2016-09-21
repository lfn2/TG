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

		public static Dictionary<int, Dictionary<int, double>> BuildMatrix(string file)
		{
			Dictionary<int, Dictionary<int, double>> matrix = new Dictionary<int, Dictionary<int, double>>();
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
						matrix[row] = new Dictionary<int, double>();

					matrix[row][column] = value;
				}
			}

			return matrix;
		}

	}
}
