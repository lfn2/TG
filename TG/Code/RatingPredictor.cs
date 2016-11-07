using System;
using System.Collections.Generic;
using System.Linq;

namespace TG
{
	public class RatingPredictor
	{

		private Dictionary<int, double> usersAverageRatings;

		public RatingPredictor()
		{
			this.usersAverageRatings = new Dictionary<int, double>();
		}

		public double PredictRating(Matrix<int> ratingsMatrix, Matrix<float> weightsMatrix, int user, int item)
		{
			double userAverageRatings = getUserAverageRatings(ratingsMatrix, user);

			double numerator = 0;
			double denominator = 0;

			if (weightsMatrix.Rows.Contains(user))
			{
				foreach (int neighbour in weightsMatrix[user])
				{
					if (ratingsMatrix.Rows.Contains(neighbour) && ratingsMatrix[neighbour].Contains(item))
					{
						double weight = weightsMatrix[user, neighbour];
						double neighbourAverageRating = getUserAverageRatings(ratingsMatrix, neighbour);
						int neighbourItemRating = ratingsMatrix[neighbour, item];

						numerator += weight * (neighbourItemRating - neighbourAverageRating);
						denominator += weight;
					}

				}
			}
			
			double predictedRating = 0;

			if (denominator > 0)
				predictedRating = userAverageRatings + (numerator / denominator);

			return predictedRating;
		}
		
		public Dictionary<int, double> CalculateUsersAverageRatings(Matrix<int> ratingsMatrix)
		{
			foreach (int user in ratingsMatrix.Rows)
				getUserAverageRatings(ratingsMatrix, user);

			Dictionary<int, double> ret = new Dictionary<int, double>();
			foreach (int i in usersAverageRatings.Keys)
				ret.Add(i, usersAverageRatings[i]);

			return ret;
		}		

		public void setUsersAverageRatings(Dictionary<int, double> averageRatings)
		{
			foreach (int i in averageRatings.Keys)
				this.usersAverageRatings[i] = averageRatings[i];
		}

		private double getUserAverageRatings(Matrix<int> ratingsMatrix, int user)
		{
			if (!this.usersAverageRatings.ContainsKey(user))
			{
				double averageRating = 0;

				if (ratingsMatrix.Rows.Contains(user) && ratingsMatrix[user].Count() > 0)
				{
					foreach (int item in ratingsMatrix[user])
						averageRating += ratingsMatrix[user, item];

					averageRating /= ratingsMatrix[user].Count();
				}

				this.usersAverageRatings[user] = averageRating;
			}

			return this.usersAverageRatings[user];
		}                   		

	}
}
