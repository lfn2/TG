using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG
{
	public class BasicRS
	{

		public static float PredictRating(Matrix<int> userItemMatrix, Matrix<float> weightsMatrix, int user, int item)
		{

			float userAverageRatings = getUserAverageRatings(userItemMatrix, user);

			float numerator = 0;
			float denominator = 0;
			foreach (int neighbour in weightsMatrix[user])
			{
				if (userItemMatrix.Rows.Contains(neighbour) && userItemMatrix[neighbour].Contains(item))
				{
					float weight = weightsMatrix[user, neighbour];
					float neighbourAverageRating = getUserAverageRatings(userItemMatrix, neighbour);
					int neighbourItemRating = userItemMatrix[neighbour, item];

					numerator += weight * (neighbourItemRating - neighbourAverageRating);
					denominator += weight;
				}
				
			}
			float predictedRating = 0;

			if (denominator > 0)
				predictedRating = userAverageRatings + (numerator / denominator);

			if (predictedRating > 5)
				predictedRating = 5;
			else if (predictedRating < 1)
				predictedRating = 0;

			return predictedRating;
		}

		

		public static float getUserAverageRatings(Matrix<int> userItemMatrix, int user)
		{
			float averageRating = 0;

			foreach (int item in userItemMatrix[user])
				averageRating += userItemMatrix[user, item];

			averageRating /= userItemMatrix[user].Count();

			return averageRating;
		}

		

	}
}
