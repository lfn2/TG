using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.DataBuild;
using TG.Pre_Processing;
using TG.Recommendation;

namespace TG
{
	public class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Choose the neighbourhood distance");

			int neighbourhoodDistance = Convert.ToInt32(Console.ReadLine());

			Stopwatch timer = new Stopwatch();
			timer.Start();

			//Matrix<int> userItemMatrix = DataBuilder.BuildMatrix3<int>(Resources.rating_data_file);
			Matrix<float> trustMatrix = DataBuilder.BuildMatrix3<float>("trust_data.txt");

			Matrix<float> estimatedTrustMatrix = TrustMatrixBuilder.BuildEstimatedTrustMatrix(trustMatrix, neighbourhoodDistance);	

			WriteToTextFile("estimate_trust_" + neighbourhoodDistance + ".txt", trustMatrix);


			//float prediction = BasicRS.PredictRating(userItemMatrix, trustMatrix, 1, 101);

			//Console.WriteLine("Rating predicted: " + prediction);

			timer.Stop();
			Console.WriteLine("Time: " + timer.Elapsed);

		}

		public static void WriteToTextFile(string filePath, Matrix<float> matrix)
		{
			StringBuilder sb = new StringBuilder();
			using (StreamWriter sw = new StreamWriter(filePath))
			{
				foreach (int user in matrix.Rows)
					foreach (int trustedUser in matrix[user])
					{
						sb.Append(user);
						sb.Append(" ");
						sb.Append(trustedUser);
						sb.Append(" ");
						sb.Append(matrix[user, trustedUser]);
						sb.AppendLine();
					}

				sw.Write(sb);
			}
				
		}

	}
}
