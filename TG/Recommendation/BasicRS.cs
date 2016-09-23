using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG
{
	public class BasicRS
	{

		public float PredictRating(Dictionary<int, Dictionary<int, int>> userItemMatrix, Dictionary<int, Dictionary<int, float>> weightsMatrix, int user, int item)
		{

			float userAverageRatings = getUserAverageRatings(userItemMatrix, user);

			float numerator = 0;
			float denominator = 0;
			foreach (int neighbour in weightsMatrix[user].Keys)
			{
				float weight = weightsMatrix[user][neighbour];
				float neighbourAverageRating = getUserAverageRatings(userItemMatrix, neighbour);
				int neighbourItemRating = userItemMatrix[neighbour][item];

				numerator += weight * (neighbourItemRating - neighbourAverageRating);
				denominator += weight;
			}

			float predictedRating = userAverageRatings + (numerator / denominator);

			return predictedRating;
		}

		

		public float getUserAverageRatings(Dictionary<int, Dictionary<int, int>> userItemMatrix, int user)
		{
			float averageRating = 0;

			foreach (int item in userItemMatrix[user].Keys)
				averageRating += userItemMatrix[user][item];

			averageRating /= userItemMatrix[user].Count;

			return averageRating;
		}

		

	}
}
