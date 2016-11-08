using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TG.Code;
using TG.TrustMetric;

namespace TG
{
	public class Program
	{

		public static int[] propDistances = { 1, 2, 3 };
		public static int[] topNValues = { 5, 10, 15, 20 };
		public static int[] topKValues = { 5, 10, 15, 20 };

		static void Main(string[] args)
		{
			while (true)
			{
				//Console.WriteLine("Choose your trust propagation metric");
				//Console.WriteLine("1 - Linear Trust Propagation");
				//Console.WriteLine("2 - Resource Allocation");

				//int metricInput = Convert.ToInt32(Console.ReadLine());

				//Console.WriteLine("Choose the propagation distance");

				//int propagationDistance = Convert.ToInt32(Console.ReadLine());

				//Console.WriteLine("Choose top N for trust propagation");
				//int topN = Convert.ToInt32(Console.ReadLine());

				//Console.WriteLine("Choose top K ecommendations");
				//int topK = Convert.ToInt32(Console.ReadLine());

				//EMetric metric = (EMetric)metricInput;

				//Console.WriteLine("New Execution!");
				//Console.WriteLine($"Metric: {metric.ToString()}");
				//Console.WriteLine($"TopN: { topN }");
				//Console.WriteLine($"TopK: {topK}");
				//Console.WriteLine($"Propagation Distance: {propagationDistance}");

				//Matrix<float> originalTrustMatrix = new Matrix<float>(Resources.trust_data_file);

				//Console.WriteLine("Building estimated trust matrix...");
				//Matrix<float> estimatedTrustMatrix = GetEstimatedTrustMatrix(originalTrustMatrix, metric, propagationDistance, topN);
				//Console.WriteLine("Estimated trust matrix built");

				//Matrix<int> ratingsMatrix = new Matrix<int>(Resources.rating_data_file);

				//Matrix<int> maxRatings = GetMaxRatings(ratingsMatrix);
				//maxRatings.WriteToFile("maxRatings");

				//RatingPredictor ratingPredictor = new RatingPredictor();

				//Dictionary<int, double> usersAverageRatings = GetUsersAverageRatings(ratingsMatrix, ratingPredictor);
				//ratingPredictor.setUsersAverageRatings(usersAverageRatings);

				//Stopwatch timer = new Stopwatch();
				//timer.Start();

				//Console.WriteLine("Running top k recommendation experiment...");
				//TopKRecommendation evaluator = new TopKRecommendation(topK);
				//evaluator.Evaluate(ratingsMatrix, estimatedTrustMatrix, maxRatings, ratingPredictor);

				//timer.Stop();
				//Console.WriteLine(timer.Elapsed);
				//Console.WriteLine();

				//Console.WriteLine($"Recall: {evaluator.Recall}");
				//Console.WriteLine($"Precision: {evaluator.Precision}");
				//Console.WriteLine();

				for (int propagationDistance = 2; propagationDistance <= 3; propagationDistance++)
				{
					for (int metricInput = 1; metricInput <= 2; metricInput++)
					{
						for (int n = 0; n < topNValues.Length; n++)
						{
							for (int k = 0; k < topKValues.Length; k++)									
							{
								int topN = topNValues[n];
								int topK = topKValues[k];

								EMetric metric = (EMetric)metricInput;

								Console.WriteLine("New Execution!");
								Console.WriteLine($"Metric: {metric.ToString()}");
								Console.WriteLine($"TopN: { topN }");
								Console.WriteLine($"TopK: {topK}");
								Console.WriteLine($"Propagation Distance: {propagationDistance}");

								Matrix<float> originalTrustMatrix = new Matrix<float>(Resources.trust_data_file);

								Console.WriteLine("Building estimated trust matrix...");
								Matrix<float> estimatedTrustMatrix = GetEstimatedTrustMatrix(originalTrustMatrix, metric, propagationDistance, topN);
								Console.WriteLine("Estimated trust matrix built");

								Matrix<int> originalRatingsMatrix = new Matrix<int>(Resources.rating_data_file);
								Matrix<int> ratingsMatrix = new Matrix<int>(Resources.rating_data_file);

								Matrix<int> maxRatings = GetMaxRatings(ratingsMatrix);
								maxRatings.WriteToFile("maxRatings");

								RatingPredictor ratingPredictor = new RatingPredictor();

								Dictionary<int, double> usersAverageRatings = GetUsersAverageRatings(ratingsMatrix, ratingPredictor);
								ratingPredictor.setUsersAverageRatings(usersAverageRatings);

								Stopwatch timer = new Stopwatch();
								timer.Start();

								Console.WriteLine("Running top k recommendation experiment...");
								TopKRecommendation evaluator = new TopKRecommendation(topK);
								evaluator.Evaluate(ratingsMatrix, estimatedTrustMatrix, maxRatings, ratingPredictor);

								timer.Stop();
								Console.WriteLine(timer.Elapsed);
								Console.WriteLine();

								Console.WriteLine($"Recall: {evaluator.Recall}");
								Console.WriteLine($"Precision: {evaluator.Precision}");
								//Console.WriteLine($"Mean Average Error: {Experiment.MeanAverageError(originalRatingsMatrix, estimatedTrustMatrix, ratingPredictor)}");
								Console.WriteLine();

								using (StreamWriter writer = new StreamWriter(Resources.results, true))
								{
									writer.WriteLine("-------------------------------------");
									writer.WriteLine($"Metric: {metric.ToString()}");
									writer.WriteLine($"TopN: { topN }");
									writer.WriteLine($"TopK: {topK}");
									writer.WriteLine($"Propagation Distance: {propagationDistance}");
									writer.WriteLine();
									writer.WriteLine($"Recall: {evaluator.Recall}");
									writer.WriteLine($"Precision: {evaluator.Precision}");
									writer.WriteLine($"Execution time: {timer.Elapsed}");
									writer.WriteLine("----------------------------------------");
									writer.WriteLine();
								}								
							}
						}
					}

				}
			}

		}

		private static Dictionary<int, double> GetUsersAverageRatings(Matrix<int> ratingsMatrix, RatingPredictor ratingPredictor)
		{
			string averageRatingsFile = Resources.users_average_ratings;
			Dictionary<int, double> averageRatings;

			if (!File.Exists(averageRatingsFile))
			{
				averageRatings = ratingPredictor.CalculateUsersAverageRatings(ratingsMatrix);
				SaveDictionary(averageRatings, averageRatingsFile);
			}
			else
				averageRatings = ReadDictionary(averageRatingsFile);

			return averageRatings;
		}

		private static void SaveDictionary(Dictionary<int, double> dict, string file)
		{
			StringBuilder sb = new StringBuilder();

			foreach (int i in dict.Keys)
				sb.AppendLine($"{i} {dict[i]}");

			using (StreamWriter sw = new StreamWriter(file))
				sw.Write(sb.ToString());
		}

		private static Dictionary<int, double> ReadDictionary(string file)
		{
			Dictionary<int, double> dict = new Dictionary<int, double>();

			var lines = File.ReadLines(file);
			foreach (var line in lines)
			{
				string[] info = line.Split(' ');

				int user = Int32.Parse(info[0]);
				double averageRating = Double.Parse(info[1]);

				dict[user] = averageRating;
			}

			return dict;
		}

		private static Matrix<int> GetMaxRatings(Matrix<int> ratingsMatrix)
		{
			Matrix<int> maxRatings = new Matrix<int>();
			Random random = new Random();
			int countAll = 0;
			int count = 0;

			foreach (int user in ratingsMatrix.Rows)
			{
				List<int> userRatedItems = ratingsMatrix[user].ToList<int>();
				foreach (int item in userRatedItems)
					if (ratingsMatrix[user, item] == 5)
					{
						ratingsMatrix.remove(user, item);
						countAll++;
						int r = random.Next(100);
						if (r == 2)
						{
							
							maxRatings[user, item] = 5;
							count++;
						}
					}
			}


			return maxRatings;
		}

		private static Matrix<float> GetEstimatedTrustMatrix(Matrix<float> originalTrustMatrix, EMetric metric, int propagationDistance, int topN)
		{
			Matrix<float> estimatedTrustMatrix;

			string matrixFile = GetMatrixFile(metric, propagationDistance, topN);

			if (File.Exists(matrixFile))
				estimatedTrustMatrix = new Matrix<float>(matrixFile);
			else
			{
				estimatedTrustMatrix = TrustMatrixBuilderFactory.GetMatrixBuilder(metric, originalTrustMatrix, topN).BuildMatrix(propagationDistance);
				estimatedTrustMatrix.WriteToFile(matrixFile);
			}

			return estimatedTrustMatrix;
		}

		private static string GetMatrixFile(EMetric metric, int propagationDistance, int topN)
		{
			string matrixFile = string.Empty;

			switch (metric)
			{
				case EMetric.LinearTrustPropagation:
					matrixFile = String.Format(Resources.linear_trust_matrix, propagationDistance);
					break;
				case EMetric.ResourceAllocation:
					matrixFile = String.Format(Resources.resource_allocation, propagationDistance, topN);
					break;
			}

			return matrixFile;
		}

	}
}
