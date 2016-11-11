using System;
using System.Collections.Concurrent;
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
		private readonly int randomRatingsCount = 100;

		private int topK;

		public double Recall { get; private set; }

		public double Precision { get; private set; }

		public double RatingsPredicted { get; private set; }

		public double TotalRatings { get; private set; }

		public int ItemsPredicted { get; private set; }

		public double Coverage { get; private set; }

		public TopKRecommendation(int k)
		{
			this.topK = k;
		}

		public void Evaluate(Matrix<int> ratingsMatrix, Matrix<float> weightsMatrix, Matrix<int> maxRatings, RatingPredictor ratingPredictor)
		{
			int testRatings = 0;
			long hits = 0;
			int ratings = 0;
			int ratingsPredicted = 0;
			int itemsPredicted = 0;

			// First type parameter is the type of the source elements
			// Second type parameter is the type of the thread-local variable (partition subtotal)

			Parallel.ForEach(maxRatings.Rows, // source collection
										() => new Tuple<long, int, int, int>(0, 0, 0, 0), // method to initialize the local variable
										(user, loop, tuple) => // method invoked by the loop on each iteration
										{
											long localHits = 0;
											int localTestRatings = 0;
											int localRatingsPredicted = 0;
											int localItemsPredicted = 0;
											foreach (int item in maxRatings[user])
											{											
												List<KeyValuePair<int, double>> predictedRatings = PredictRatings(ratingsMatrix, weightsMatrix, ratingPredictor, user, item);

												for (int i = 0; i < this.topK && i < predictedRatings.Count; i++)
													if (predictedRatings[i].Key == item && predictedRatings[i].Value != 0)
														localHits++;

												foreach (KeyValuePair<int, double> pair in predictedRatings)
													if (pair.Key == item && pair.Value != 0)
														localItemsPredicted++;												

												localTestRatings++;
											}

											return new Tuple<long, int, int, int>(tuple.Item1 + localHits, tuple.Item2 + localTestRatings, tuple.Item3 + localRatingsPredicted, tuple.Item4 + localItemsPredicted); // value to be passed to next iteration
										},
										// Method to be executed when each partition has completed.
										// finalResult is the final value of subtotal for a particular partition.
										(pair) =>
										 {
											 Interlocked.Add(ref hits, pair.Item1);
											 Interlocked.Add(ref testRatings, pair.Item2);
											 Interlocked.Add(ref ratings, 101 * pair.Item2);
											 Interlocked.Add(ref ratingsPredicted, pair.Item3);
											 Interlocked.Add(ref itemsPredicted, pair.Item4);									
										 } 
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

			RatingsPredicted = ratingsPredicted;
			TotalRatings = ratings;
			Coverage = RatingsPredicted / TotalRatings;
			ItemsPredicted = itemsPredicted;
			Recall = (double)hits / testRatings;
			Precision = Recall / topK;
		}

		private List<KeyValuePair<int, double>> PredictRatings(Matrix<int> ratingsMatrix, Matrix<float> weightsMatrix, RatingPredictor ratingPredictor, int user, int item)
		{
			List<KeyValuePair<int, double>> predictions = new List<KeyValuePair<int, double>>();
			HashSet<int> randomItems = new HashSet<int>();

			randomItems.Add(item);

			Random random = new Random();
			while (randomItems.Count < randomRatingsCount + 1)
			{
				int randomMovie = random.Next(this.moviesCount);
				if (!ratingsMatrix[user].Contains(randomMovie))
					randomItems.Add(randomMovie);
			}

			foreach (int i in randomItems)
			{
				double predictedRating = ratingPredictor.PredictRating(ratingsMatrix, weightsMatrix, user, i, false);
				if (predictedRating > 0)
					predictions.Add(new KeyValuePair<int, double>(i, predictedRating));
			}

			List<KeyValuePair<int, double>> orderedPredictions = predictions.OrderByDescending(x => x.Value).ToList();

			return orderedPredictions;
		}

	}
}
