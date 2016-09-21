using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.DataBuilder
{
	public class RatingDataBuilder
	{	

		public void BuildData(int[,] ratingsMatrix)
		{
			using (StreamReader sr = new StreamReader(Resources.rating_data_file))
			{
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();
					string[] info = line.Split(' ');

					int user = Int32.Parse(info[0]);
					int item = Int32.Parse(info[1]);
					int rating = Int32.Parse(info[2]);

					ratingsMatrix[user, item] = rating;
				}
			}
		}

	}
}