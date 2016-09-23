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
			Stopwatch timer = new Stopwatch();
			timer.Start();
			//Dictionary<int, Dictionary<int, float>> userRatingsMatrix = DataBuilder.BuildMatrix(Resources.rating_data_file);
			Dictionary<int, Dictionary<int, float>> trustMatrix = DataBuilder.BuildMatrix(Resources.trust_data_file);


			Dictionary<int, Dictionary<int, float>> estimatedTrust = TrustMetric.EstimateTrust(trustMatrix, 3);
			//SparseMatrix<float> sparseMatrix = DataBuilder.BuildMatrix2(Resources.trust_data_file);

			//SparseMatrix<float> estimateMatrix = TrustMetric.EstimateTrust2(sparseMatrix, 3);

			//using (StreamWriter sw = new StreamWriter("estimate_trust_3_2.txt"))
			//	foreach (int user in estimateMatrix.GetRows().Keys)
			//		foreach (int trustedUser in estimateMatrix.GetRows()[user].Keys)
			//			sw.WriteLine((user) + " " + (trustedUser) + " " + estimateMatrix.GetAt(user, trustedUser));

			using (StreamWriter sw = new StreamWriter("estimate_trust_3_2.txt"))
				foreach (int user in estimatedTrust.Keys)
					foreach (int trustedUser in estimatedTrust[user].Keys)
						sw.WriteLine((user) + " " + (trustedUser) + " " + estimatedTrust[user][trustedUser]);

			timer.Stop();
			Console.WriteLine("Time: " + timer.Elapsed);
		}		
	}
}
