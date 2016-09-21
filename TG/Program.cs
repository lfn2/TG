using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.DataBuild;
using TG.Recommendation;

namespace TG
{
	public class Program
	{
		static void Main(string[] args)
		{
			bool[,] m = new bool[49290, 139738];

			Stopwatch timer = new Stopwatch();
			timer.Start();
			Dictionary<int, Dictionary<int, double>> userRatingsMatrix = DataBuilder.BuildMatrix(Resources.rating_data_file);
			Dictionary<int, Dictionary<int, double>> trustMatrix = DataBuilder.BuildMatrix(Resources.trust_data_file);

			Dictionary<int, Dictionary<int, double>> estimateMatrix = TrustMetric.EstimateTrust(trustMatrix, 2);

			timer.Stop();
			Console.WriteLine(trustMatrix[37460][47390]);
			Console.WriteLine("Time: " + timer.Elapsed);
		}		
	}
}
