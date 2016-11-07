using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TG.Code
{
	public class TopKRecommendation
	{
		private readonly int moviesCount = 139739;
		private readonly int testRatings = 301053;

		private int topK;

		public double Recall { get; private set; }

		public double Precision { get; private set; }

		public TopKRecommendation(int k)
		{
			this.topK = k;
		}

		public void Evaluate(Matrix<int> ratingsMatrix, Matrix<float> weightsMatrix, Matrix<int> maxRatings, RatingPredictor ratingPredictor)
		{
			long hits = 0;

			// First type parameter is the type of the source elements
			// Second type parameter is the type of the thread-local variable (partition subtotal)

			Parallel.ForEach(maxRatings.Rows, // source collection
										() => 0, // method to initialize the local variable
										(user, loop, localHits) => // method invoked by the loop on each iteration
										{
											foreach (int item in maxRatings[user])
											{
												List<KeyValuePair<int, double>> predictedRatings = PredictRatings(ratingsMatrix, weightsMatrix, ratingPredictor, user, item);

												for (int i = 0; i < this.topK; i++)
													if (predictedRatings[i].Key == item)
														hits++;
											}

											return localHits; // value to be passed to next iteration
										},
										// Method to be executed when each partition has completed.
										// finalResult is the final value of subtotal for a particular partition.
										(finalResult) => Interlocked.Add(ref hits, finalResult)
										);


			//foreach (int user in maxRatings.Rows)
			//{
			//	Parallel.ForEach(maxRatings.GetRow(user), // source collection
			//							() => 0, // method to initialize the local variable
			//							(userRatings, loop, localHits) => // method invoked by the loop on each iteration
			//							{
			//								List<KeyValuePair<int, double>> predictedRatings = PredictRatings(ratingsMatrix, weightsMatrix, ratingPredictor, user, userRatings.Key);

			//								for (int i = 0; i < this.topK; i++)
			//									if (predictedRatings[i].Key == userRatings.Key)
			//										localHits++;

			//								return localHits; // value to be passed to next iteration
			//							},
			//							// Method to be executed when each partition has completed.
			//							// finalResult is the final value of subtotal for a particular partition.
			//							(finalResult) => Interlocked.Add(ref hits, finalResult)
			//							);
			//}

			Recall = (double) hits / this.testRatings;
			Precision = Recall / topK;
		}

		private int GetUserHits(Matrix<int> ratingsMatrix, Matrix<float> weightsMatrix, Matrix<int> maxRatings, RatingPredictor ratingPredictor)
		{
			int hits = 0;

			foreach (int user in maxRatings.Rows)
				foreach (int item in maxRatings[user])
				{
					List<KeyValuePair<int, double>> predictedRatings = PredictRatings(ratingsMatrix, weightsMatrix, ratingPredictor, user, item);

					for (int i = 0; i < this.topK; i++)
						if (predictedRatings[i].Key == i)
							hits++;
				}

			return hits;
		}

		private List<KeyValuePair<int, double>> PredictRatings(Matrix<int> ratingsMatrix, Matrix<float> weightsMatrix, RatingPredictor ratingPredictor, int user, int item)
		{
			List<KeyValuePair<int, double>> predictions = new List<KeyValuePair<int, double>>();
			HashSet<int> randomItems = new HashSet<int>();

			randomItems.Add(item);

			Random random = new Random();
			while (randomItems.Count < 1001)
			{
				int randomMovie = random.Next(this.moviesCount);
				if (!ratingsMatrix[user].Contains(randomMovie))
					randomItems.Add(randomMovie);
			}

			foreach (int i in randomItems)
				predictions.Add(new KeyValuePair<int, double>(i, ratingPredictor.PredictRating(ratingsMatrix, weightsMatrix, user, i)));

			List<KeyValuePair<int, double>> orderedPredictions = predictions.OrderByDescending(x => x.Value).ToList();

			return orderedPredictions;
		}

	}
}
