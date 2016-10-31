using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG
{
	public class Experiment
	{

		public static double MeanAbsoluteUserError(Matrix<int> ratingsMatrix, Matrix<float> weightsMatrix)
		{
			double usersMAE = 0;

			foreach (int user in ratingsMatrix.Rows)
			{
				double totalErrors = 0;
				int ratings = 0;
				List<int> userRatedItems = ratingsMatrix[user].ToList<int>();

				foreach (int item in userRatedItems)
				{
					int originalRating = ratingsMatrix[user, item];
					ratingsMatrix.remove(user, item);

					double predictedRating = Math.Round(RatingPredictor.PredictRating(ratingsMatrix, weightsMatrix, user, item));

					if (predictedRating > 0)
					{
						if (predictedRating > 5)
							predictedRating = 5;
						else if (predictedRating < 1)
							predictedRating = 1;

						ratings++;
						totalErrors += Math.Abs(predictedRating - originalRating);
					}

					ratingsMatrix[user, item] = originalRating;
				}

				if (ratings > 0)
					usersMAE += totalErrors / ratings;
			}

			double maue = usersMAE / ratingsMatrix.Rows.Count();

			return maue;
		}

		public static double MeanAverageError(Matrix<int> ratingsMatrix, Matrix<float> weightsMatrix)
		{

			int ratings = 0;
			double totalErrors = 0;
			double mae = 0;

			int predictedRatings = 0;
			int totalRatings = 0;

			foreach (int user in ratingsMatrix.Rows)
			{
				List<int> userRatedItems = ratingsMatrix[user].ToList<int>();
				foreach (int item in userRatedItems)
				{
					int originalRating = ratingsMatrix[user, item];
					ratingsMatrix.remove(user, item);
					totalRatings++;

					double predictedRating = Math.Round(RatingPredictor.PredictRating(ratingsMatrix, weightsMatrix, user, item));				
					
					if (predictedRating > 0)
					{
						if (predictedRating > 5)
							predictedRating = 5;
						else if (predictedRating < 1)
							predictedRating = 1;

						ratings++;
						predictedRatings++;
						totalErrors += Math.Abs(predictedRating - originalRating);
					}

					ratingsMatrix[user, item] = originalRating;
				}
			}

			mae = totalErrors / ratings;

			double coverage = (double)predictedRatings / totalRatings;
			Console.WriteLine($"Coverage: {coverage}");

			return mae;
		}

	}
}
