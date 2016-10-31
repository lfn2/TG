using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace TG
{
	public class Program
	{
		static void Main(string[] args)
		{
			string input = string.Empty;

			while (!input.Equals("9"))
			{
				Console.WriteLine("Menu: ");
				Console.WriteLine("3 - Run Experiments");
				Console.WriteLine("9 - Exit");
				Console.WriteLine();

				input = Console.ReadLine();

				switch(input)
				{
					case "3":
						RunExperiment();
						break;
				}
			}
		}

		private static int GetAlgorithm()
		{
			Console.WriteLine("Choose your algorithm");
			Console.WriteLine("1 - Basic Trust");
			Console.WriteLine("2 - RA Based Trust");
			Console.WriteLine("3 - CN Based Trust");
			Console.WriteLine("4 - Salton Based Trust");
			Console.WriteLine("5 = Jaccard Based Trust");
			Console.WriteLine("6 = Estimated Trust Rebuilt Matrix");

			return Int32.Parse(Console.ReadLine());
		}

		

		private static void RunExperiment()
		{
			int neighbourhoodDistance = GetNeighbourhoodDistance();
			int algorithm = GetAlgorithm();

			Matrix<int> ratingsMatrix = GetRatingsMatrix();

			Matrix<float> matrix = null;
			switch (algorithm)
			{
				case 1:
					 matrix = GetEstimatedTrustMatrix(neighbourhoodDistance);
					break;
				case 2:
					matrix = GetRABasedTrustMatrix(neighbourhoodDistance);
					break;
				case 3:
					matrix = GetCNBasedTrustMatrix(neighbourhoodDistance);
					break;
				case 4:
					matrix = GetSaltonBasedTrustMatrix(neighbourhoodDistance);
					break;
				case 5:
					matrix = GetJaccardBasedTrustMatrix(neighbourhoodDistance);
					break;
				case 6:
					matrix = GetEstimatedTrustRebuiltMatrix(ratingsMatrix, neighbourhoodDistance);
					break;
			}			
						
			switch (1)
			{
				case 1:
					Console.WriteLine("Running Mean Average Error Experiment...");
					double meanAverageError = Experiment.MeanAverageError(ratingsMatrix, matrix);
					Console.WriteLine($"Mean Average Error = {meanAverageError}");
					break;
				case 2:
					Console.WriteLine("Running Mean Absolute User Error Experiment...");
					double meanAbsoluteUserError = Experiment.MeanAbsoluteUserError(ratingsMatrix, matrix);
					Console.WriteLine($"Mean Absolute User Error = {meanAbsoluteUserError}");
					break;
			}			
		}

		private static int GetNeighbourhoodDistance()
		{
			Console.WriteLine("Choose the neighbourhood distance");
			int neighbourhoodDistance = Convert.ToInt32(Console.ReadLine());

			return neighbourhoodDistance;
		}

		private static Matrix<float> GetCNBasedTrustMatrix(int neighbourhoodDistace)
		{
			Matrix<float> CNBasedTrustMatrix;
			string CNBasedTrustFile = String.Format(Resources.cn_based_trust, neighbourhoodDistace);
			if (File.Exists(CNBasedTrustFile))
			{
				Console.WriteLine("Reading CN based trust data...");
				CNBasedTrustMatrix = new Matrix<float>(CNBasedTrustFile);
				Console.WriteLine("CN based trust data read");
			}
			else
			{
				Console.WriteLine("Couldn't find CN based trust data");
				CNBasedTrustMatrix = CreateCNBasedTrustMatrix(neighbourhoodDistace);

				SaveMatrix(CNBasedTrustMatrix, String.Format(Resources.cn_based_trust, neighbourhoodDistace));
			}

			return CNBasedTrustMatrix;
		}

		private static Matrix<float> GetRABasedTrustMatrix(int neighbourhoodDistance)
		{
			Matrix<float> RABasedTrustMatrix;
			string raBasedTrustFile = String.Format(Resources.ra_based_trust, neighbourhoodDistance);
			if (File.Exists(raBasedTrustFile))
			{
				Console.WriteLine("Reading RA based trust data...");
				RABasedTrustMatrix = new Matrix<float>(raBasedTrustFile);
				Console.WriteLine("Proximity based trust data read");
			}
			else
			{
				Console.WriteLine("Couldn't find RA based trust data");
				RABasedTrustMatrix = CreateRABasedTrustMatrix(neighbourhoodDistance);

				SaveMatrix(RABasedTrustMatrix, String.Format(Resources.ra_based_trust, neighbourhoodDistance));
			}

			return RABasedTrustMatrix;
		}

		private static Matrix<float> GetSaltonBasedTrustMatrix(int neighbourhoodDistance)
		{
			Matrix<float> saltonBasedTrustMatrix;
			string saltonBasedTrustFile = String.Format(Resources.salton_based_trust, neighbourhoodDistance);
			if (File.Exists(saltonBasedTrustFile))
			{
				Console.WriteLine("Reading salton based trust data...");
				saltonBasedTrustMatrix = new Matrix<float>(saltonBasedTrustFile);
				Console.WriteLine("Salton based trust data read");
			}
			else
			{
				Console.WriteLine("Couldn't find salton based trust data");
				saltonBasedTrustMatrix = CreateSaltonBasedTrustMatrix(neighbourhoodDistance);

				SaveMatrix(saltonBasedTrustMatrix, String.Format(Resources.salton_based_trust, neighbourhoodDistance));
			}

			return saltonBasedTrustMatrix;
		}

		private static Matrix<float> GetJaccardBasedTrustMatrix(int neighbourhoodDistance)
		{
			Matrix<float> jaccardBasedTrustMatrix;
			string jaccardBasedTrustFile = String.Format(Resources.jaccard_based_trust, neighbourhoodDistance);
			if (File.Exists(jaccardBasedTrustFile))
			{
				Console.WriteLine("Reading jaccard based trust data...");
				jaccardBasedTrustMatrix = new Matrix<float>(jaccardBasedTrustFile);
				Console.WriteLine("Jaccard based trust data read");
			}
			else
			{
				Console.WriteLine("Couldn't find salton based trust data");
				jaccardBasedTrustMatrix = CreateJaccardBasedTrustMatrix(neighbourhoodDistance);

				SaveMatrix(jaccardBasedTrustMatrix, String.Format(Resources.jaccard_based_trust, neighbourhoodDistance));
			}

			return jaccardBasedTrustMatrix;
		}

		private static Matrix<float> GetEstimatedTrustRebuiltMatrix(Matrix<int> ratingsMatrix, int neighbourHoodDistance)
		{
			Matrix<float> estimatedTrustMatrix;
			string rebuiltTrustMatrixFile = String.Format(Resources.rebuild_trust_matrix, neighbourHoodDistance);

			if (File.Exists(rebuiltTrustMatrixFile))
			{
				Console.WriteLine("Reading rebuild trust matrix data...");
				estimatedTrustMatrix = new Matrix<float>(rebuiltTrustMatrixFile);
				Console.WriteLine("Rebuilt trust matrix data read");
			}
			else
			{
				Console.WriteLine("Creating rebuilt trust matrix...");

				Matrix<float> trustMatrix = GetOriginalTrustMatrix();
				Matrix<double> correlationMatrix = GetCorrelationMatrix(trustMatrix, ratingsMatrix);
				trustMatrix = RebuildTrustMatrix(trustMatrix, correlationMatrix);

				estimatedTrustMatrix = EstimatedTrustMatrixBuilder.BuildEstimatedTrustMatrix(trustMatrix, neighbourHoodDistance);

				Console.WriteLine("Rebuilt trust matrix created");

				SaveMatrix(estimatedTrustMatrix, rebuiltTrustMatrixFile);
			}

			return estimatedTrustMatrix;
		}

		private static Matrix<float> RebuildTrustMatrix(Matrix<float> trustMatrix, Matrix<double> correlationMatrix)
		{
			List<int> users = trustMatrix.Rows.ToList<int>();
			foreach (int user in users)
			{
				List<int> neighbours = trustMatrix[user].ToList<int>();
				foreach (int neighbour in neighbours)
				{
					if (correlationMatrix[user, neighbour] < 0.5)
					{
						trustMatrix.remove(user, neighbour);

						if (trustMatrix[user].Count() == 0)
							trustMatrix.remove(user);
					}
				}
			}

			return trustMatrix;
		}

		private static Matrix<double> GetCorrelationMatrix(Matrix<float> trustMatrix, Matrix<int> ratingsMatrix)
		{
			Matrix<double> correlationMatrix = new Matrix<double>();

			foreach (int user in trustMatrix.Rows)
			{
				foreach (int neighbour in trustMatrix[user])
				{
					correlationMatrix[user, neighbour] = GetCorrelation(ratingsMatrix, user, neighbour);
				}
			}

			return correlationMatrix;
		}

		private static double GetCorrelation(Matrix<int> matrix, int a, int u)
		{
			double correlation = 0;

			HashSet<int> commonItems = GetCommonItems(matrix, a, u);
			
			if (commonItems.Count >= 2)
			{
				double aAverageRating = GetAverageRating(matrix, a);
				double uAverageRating = GetAverageRating(matrix, u);
				double num = 0;
				double den1 = 0;
				double den2 = 0;

				foreach (int item in commonItems)
				{
					double r1 = matrix[a, item] - aAverageRating;
					double r2 = matrix[u, item] - uAverageRating;

					num += r1 * r2;
					den1 += r1 * r1;
					den2 += r2 * r2;
				}

				correlation = num / Math.Sqrt(den1 * den2);
			}

			return correlation;
		}

		private static HashSet<int> GetCommonItems(Matrix<int> matrix, int a, int u)
		{
			HashSet<int> set = new HashSet<int>();

			if (matrix.Rows.Contains(a) && matrix.Rows.Contains(u))
				foreach (int item in matrix[a])
					if (matrix[u].Contains(item))
						set.Add(item);

			return set;
		}

		private static double GetAverageRating(Matrix<int> matrix, int user)
		{
			double avg = 0;

			if (matrix.HasRow(user))
			{
				int sum = matrix[user].Sum();
				avg = (double) sum / matrix[user].Count();
			}

			return avg;
		}

		private static Matrix<float> GetEstimatedTrustMatrix(int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix;
			string estimatedTrustDataFile = String.Format(Resources.estimated_trust_data_file, neighbourhoodDistance);
			if (File.Exists(estimatedTrustDataFile))
			{
				Console.WriteLine("Reading estimated trust data...");
				estimatedTrustMatrix = new Matrix<float>(estimatedTrustDataFile);
				Console.WriteLine("Estimated trust data read");
			}
			else
			{
				Console.WriteLine("Couldn't find estimated trust data");
				estimatedTrustMatrix = CreateEstimatedTrustMatrix(neighbourhoodDistance);

				SaveMatrix(estimatedTrustMatrix, String.Format(Resources.estimated_trust_data_file, neighbourhoodDistance));
			}

			return estimatedTrustMatrix;
		}

		private static Matrix<int> GetRatingsMatrix()
		{
			Console.WriteLine("Reading rating data...");
			Matrix<int> ratingsMatrix = new Matrix<int>(Resources.rating_data_file);
			Console.WriteLine("Reading data read");

			return ratingsMatrix;
		}

		private static Matrix<float> GetOriginalTrustMatrix()
		{
			Console.WriteLine("Reading trust data");
			Matrix<float> trustMatrix = new Matrix<float>(Resources.trust_data_file);
			Console.WriteLine("Trust data read");

			return trustMatrix;
		}

		private static Matrix<float> CreateCNBasedTrustMatrix(int neighbourhoodDistance)
		{
			Matrix<float> trustMatrix = GetOriginalTrustMatrix();

			Console.WriteLine("Creating CN based trust matrix...");
			Matrix<float> CNBasedTrustMatrix = EstimatedTrustMatrixBuilder.BuildCommonNeighboursBasedTrust(trustMatrix, neighbourhoodDistance);
			Console.WriteLine("CN based trust matrix created");

			return CNBasedTrustMatrix;
		}

		private static Matrix<float> CreateRABasedTrustMatrix(int neighbourhoodDistance)
		{
			Matrix<float> trustMatrix = GetOriginalTrustMatrix();

			Console.WriteLine("Creating RA based trust matrix...");
			Matrix<float> RABasedTrustMatrix = EstimatedTrustMatrixBuilder.BuildRABasedTrust(trustMatrix, neighbourhoodDistance);
			Console.WriteLine("RA based trust matrix created");

			return RABasedTrustMatrix;
		}

		private static Matrix<float> CreateSaltonBasedTrustMatrix(int neighbourhoodDistance)
		{
			Matrix<float> trustMatrix = GetOriginalTrustMatrix();

			Console.WriteLine("Creating Salton based trust matrix...");
			Matrix<float> SaltonBasedTrustMatrix = EstimatedTrustMatrixBuilder.BuildSaltonIndexTrustMatrix(trustMatrix, neighbourhoodDistance);
			Console.WriteLine("Salton based trust matrix created");

			return SaltonBasedTrustMatrix;
		}

		private static Matrix<float> CreateJaccardBasedTrustMatrix(int neighbourhoodDistance)
		{
			Matrix<float> trustMatrix = GetOriginalTrustMatrix();

			Console.WriteLine("Creating Jaccard based trust matrix...");
			Matrix<float> jaccardBasedTrustMatrix = EstimatedTrustMatrixBuilder.BuildJaccardIndexTrustMatrix(trustMatrix, neighbourhoodDistance);
			Console.WriteLine("Jaccard based trust matrix created");

			return jaccardBasedTrustMatrix;
		}

		private static Matrix<float> CreateEstimatedTrustMatrix(int neighbourhoodDistance)
		{
			Matrix<float> trustMatrix = GetOriginalTrustMatrix();

			Console.WriteLine("Creating estimated trust matrix...");
			Matrix<float> estimatedTrustMatrix =  EstimatedTrustMatrixBuilder.BuildEstimatedTrustMatrix(trustMatrix, neighbourhoodDistance);
			Console.WriteLine("Estimate trust matrix created");

			return estimatedTrustMatrix;
		}

		private static void SaveMatrix<T>(Matrix<T> matrix, string filename)
		{
			Console.WriteLine("Saving matrix...");
			matrix.WriteToFile(filename);
			Console.WriteLine("Matrix saved");			
		}

		

	}
}
