using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.DataBuilder
{
	public class TrustDataBuilder
	{

		public int[,] BuildData(int[,] trustMatrix)
		{
			using (StreamReader sr = new StreamReader(Resources.trust_data_file))
			{				
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();
					string[] info = line.Split(' ');

					int sourceUser = Int32.Parse(info[0]);
					int targetUser = Int32.Parse(info[1]);

					trustMatrix[sourceUser, targetUser] = 1;
				}
			}

			return trustMatrix;
		}

	}
}