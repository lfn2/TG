using System;
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
				Console.WriteLine("1 - Build estimated trust matrix");
				Console.WriteLine("2 - Predict rating for a user");
				Console.WriteLine("3 - Run Experiments");
				Console.WriteLine("4 - Get experiment results");
				Console.WriteLine("9 - Exit");
				Console.WriteLine();

				input = Console.ReadLine();

				switch(input)
				{
					case "1":
						BuildEstimatedTrustMatrix();
						break;
					case "2":
						PredictRating();
						break;
					case "3":
						RunExperiment();
						break;
				}

				//float prediction = BasicRS.PredictRating(userItemMatrix, trustMatrix, 1, 101);

				//Console.WriteLine("Rating predicted: " + prediction);
			}
		}

		

		private static void BuildEstimatedTrustMatrix()
		{
			int neighbourhoodDistance = GetNeighbourhoodDistance();

			Matrix<float> estimatedTrustMatrix = CreateEstimatedTrustMatrix(neighbourhoodDistance);

			SaveMatrix(estimatedTrustMatrix, neighbourhoodDistance);
		}

		private static void PredictRating()
		{
			int neighbourhoodDistance = GetNeighbourhoodDistance();

			Matrix<int> ratingsMatrix = GetRatingsMatrix();

			Matrix<float> estimatedTrustMatrix = GetEstimatedTrustMatrix(neighbourhoodDistance);
						
			string predict = "y";
			while (predict.Equals("y", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.WriteLine("Choose the user");
				int user = Convert.ToInt32(Console.ReadLine());

				Console.WriteLine("Choose the item");
				int item = Convert.ToInt32(Console.ReadLine());

				Console.WriteLine(String.Format("Predicting rating for user {0} and item {1}...", user, item));
				double predictedRating = RatingPredictor.PredictRating(ratingsMatrix, estimatedTrustMatrix, user, item);
				Console.WriteLine($"Predicted Rating: {predictedRating}");

				Console.WriteLine("Predict another rating? (Y/N)");
				predict = Console.ReadLine();				
			}
					
		}

		private static void RunExperiment()
		{
			int neighboudhoodDistance = GetNeighbourhoodDistance();

			Console.WriteLine("Choose your experiment");
			Console.WriteLine("1 - Mean Average Error");
			Console.WriteLine("2 - Mean Absolute User Error");

			int experiment = Int32.Parse(Console.ReadLine());

			Matrix<int> ratingsMatrix = GetRatingsMatrix();
			Matrix<float> estimatedTrustMatrix = GetEstimatedTrustMatrix(neighboudhoodDistance);
						
			switch (experiment)
			{
				case 1:
					Console.WriteLine("Running Mean Average Error Experiment...");
					double meanAverageError = Experiment.MeanAverageError(ratingsMatrix, estimatedTrustMatrix);
					Console.WriteLine($"Mean Average Error = {meanAverageError}");
					break;
				case 2:
					Console.WriteLine("Running Mean Absolute User Error Experiment...");
					double meanAbsoluteUserError = Experiment.MeanAbsoluteUserError(ratingsMatrix, estimatedTrustMatrix);
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

				Console.WriteLine("Save estimated trust matrix? (Y/N)");
				if (Console.ReadLine().Equals("y", StringComparison.InvariantCultureIgnoreCase))
					SaveMatrix(estimatedTrustMatrix, neighbourhoodDistance);
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

		private static Matrix<float> CreateEstimatedTrustMatrix(int neighbourhoodDistance)
		{
			Console.WriteLine("Reading trust data");
			Matrix<int> trustMatrix = new Matrix<int>(Resources.trust_data_file);
			Console.WriteLine("Trust data read");

			Console.WriteLine("Creating estimated trust matrix...");
			Matrix<float> estimatedTrustMatrix =  EstimatedTrustMatrixBuilder.BuildEstimatedTrustMatrix(trustMatrix, neighbourhoodDistance);
			Console.WriteLine("Estimate trust matrix created");

			return estimatedTrustMatrix;
		}

		private static void SaveMatrix<T>(Matrix<T> matrix, int neighbourhoodDistance)
		{
			Console.WriteLine("Saving matrix...");
			matrix.WriteToFile(string.Format(Resources.estimated_trust_data_file, neighbourhoodDistance));
			Console.WriteLine("Matrix saved");
		}

		

	}
}
