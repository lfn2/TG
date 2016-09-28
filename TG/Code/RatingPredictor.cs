using System;
using System.Linq;

namespace TG
{
	public class RatingPredictor
	{
		public static int PredictRating(Matrix<int> ratingsMatrix, Matrix<float> weightsMatrix, int user, int item)
		{
			float userAverageRatings = getUserAverageRatings(ratingsMatrix, user);

			double numerator = 0;
			double denominator = 0;

			if (weightsMatrix.Rows.Contains(user))
			{
				foreach (int neighbour in weightsMatrix[user])
				{
					if (ratingsMatrix.Rows.Contains(neighbour) && ratingsMatrix[neighbour].Contains(item))
					{
						float weight = weightsMatrix[user, neighbour];
						float neighbourAverageRating = getUserAverageRatings(ratingsMatrix, neighbour);
						int neighbourItemRating = ratingsMatrix[neighbour, item];

						numerator += weight * (neighbourItemRating - neighbourAverageRating);
						denominator += weight;
					}

				}
			}
			
			double predictedRating = 0;

			if (denominator > 0)
				predictedRating = userAverageRatings + (numerator / denominator);

			return (int) predictedRating;
		}		

		public static float getUserAverageRatings(Matrix<int> ratingsMatrix, int user)
		{
			float averageRating = 0;

			if (ratingsMatrix.Rows.Contains(user) && ratingsMatrix[user].Count() > 0)
			{
				foreach (int item in ratingsMatrix[user])
					averageRating += ratingsMatrix[user, item];

				averageRating /= ratingsMatrix[user].Count();
			}

			return averageRating;
		}                   		

	}
}
