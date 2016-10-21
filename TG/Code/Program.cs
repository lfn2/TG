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

			SaveMatrix(estimatedTrustMatrix, String.Format(Resources.resource_allocation_data_file, neighbourhoodDistance));
		}

		private static void BuildResourcesAllocationMatrix()
		{
			int neighbourhoodDistance = GetNeighbourhoodDistance();

			Matrix<float> resourcesAllocationMatrix = CreateResourceAllocationMatrix(neighbourhoodDistance);

			SaveMatrix(resourcesAllocationMatrix, String.Format(Resources.resource_allocation_data_file, neighbourhoodDistance));
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

		private static int GetAlgorithm()
		{
			Console.WriteLine("Choose your algorithm");
			Console.WriteLine("1 - Basic Trust");
			Console.WriteLine("2 - Resource Allocation Index");
			Console.WriteLine("3 - Proximity Based Trust");

			return Int32.Parse(Console.ReadLine());
		}

		

		private static void RunExperiment()
		{
			int neighbourhoodDistance = GetNeighbourhoodDistance();
			int algorithm = GetAlgorithm();

			Console.WriteLine("Choose your experiment");
			Console.WriteLine("1 - Mean Average Error");
			Console.WriteLine("2 - Mean Absolute User Error");

			int experiment = Int32.Parse(Console.ReadLine());

			Matrix<int> ratingsMatrix = GetRatingsMatrix();

			Matrix<float> matrix = null;
			switch (algorithm)
			{
				case 1:
					 matrix = GetEstimatedTrustMatrix(neighbourhoodDistance);
					break;
				case 2:
					matrix = GetResourceAllocationMatrix(neighbourhoodDistance);
					break;
				case 3:
					matrix = GetProximityBasedTrustMatrix(neighbourhoodDistance);
					break;
			}			
						
			switch (experiment)
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

		private static Matrix<float> GetProximityBasedTrustMatrix(int neighbourhoodDistance)
		{
			Matrix<float> proximityBasedTrustMatrix;
			string proximityBasedTrustFile = String.Format(Resources.proximity_based_trust, neighbourhoodDistance);
			if (File.Exists(proximityBasedTrustFile))
			{
				Console.WriteLine("Reading proximity based trust data...");
				proximityBasedTrustMatrix = new Matrix<float>(proximityBasedTrustFile);
				Console.WriteLine("Proximity based trust data read");
			}
			else
			{
				Console.WriteLine("Couldn't find proximity based trust data");
				proximityBasedTrustMatrix = CreateProximityBasedTrustMatrix(neighbourhoodDistance);

				SaveMatrix(proximityBasedTrustMatrix, String.Format(Resources.proximity_based_trust, neighbourhoodDistance));
			}

			return proximityBasedTrustMatrix;
		}

		private static Matrix<float> GetResourceAllocationMatrix(int neighbourhoodDistance)
		{
			Matrix<float> resourceAllocationMatrix;
			string resourceAllocationFile = String.Format(Resources.resource_allocation_data_file, neighbourhoodDistance);
			if (File.Exists(resourceAllocationFile))
			{
				Console.WriteLine("Reading resource allocation data...");
				resourceAllocationMatrix = new Matrix<float>(resourceAllocationFile);
				Console.WriteLine("Resource Allocation data read");
			}
			else
			{
				Console.WriteLine("Couldn't find resource allocation data");
				resourceAllocationMatrix = CreateResourceAllocationMatrix(neighbourhoodDistance);

				SaveMatrix(resourceAllocationMatrix, String.Format(Resources.resource_allocation_data_file, neighbourhoodDistance));
			}

			return resourceAllocationMatrix;
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

		private static Matrix<float> CreateResourceAllocationMatrix(int neighbourhoodDistance)
		{
			Matrix<float> trustMatrix = GetOriginalTrustMatrix();

			Console.WriteLine("Creating resource allocation matrix...");
			Matrix<float> resourceAllocationMatrix = EstimatedTrustMatrixBuilder.BuildResourceAllocationMatrix(trustMatrix, neighbourhoodDistance);
			Console.WriteLine("Resource allocation matrix created");

			return resourceAllocationMatrix;
		}

		private static Matrix<float> GetOriginalTrustMatrix()
		{
			Console.WriteLine("Reading trust data");
			Matrix<float> trustMatrix = new Matrix<float>(Resources.trust_data_file);
			Console.WriteLine("Trust data read");

			return trustMatrix;
		}

		private static Matrix<float> CreateProximityBasedTrustMatrix(int neighbourhoodDistance)
		{
			Matrix<float> trustMatrix = GetOriginalTrustMatrix();

			Console.WriteLine("Creating proximity based trust matrix...");
			Matrix<float> proximityBasedTrustMatrix = EstimatedTrustMatrixBuilder.BuildProximityBasedTrust(trustMatrix, neighbourhoodDistance);
			Console.WriteLine("Proximity based trust matrix created");

			return proximityBasedTrustMatrix;
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
