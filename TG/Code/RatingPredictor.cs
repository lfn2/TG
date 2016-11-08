using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TG
{
	public class RatingPredictor
	{

		private ConcurrentDictionary<int, double> usersAverageRatings;

		public RatingPredictor()
		{
			this.usersAverageRatings = new ConcurrentDictionary<int, double>();
		}

		public double PredictRating(Matrix<int> ratingsMatrix, Matrix<float> weightsMatrix, int user, int item, bool mae)
		{
			double userAverageRatings = getUserAverageRatings(ratingsMatrix, user, mae);

			double numerator = 0;
			double denominator = 0;

			if (weightsMatrix.Rows.Contains(user))
			{
				foreach (int neighbour in weightsMatrix[user])
				{
					if (ratingsMatrix.Rows.Contains(neighbour) && ratingsMatrix[neighbour].Contains(item))
					{
						double weight = weightsMatrix[user, neighbour];
						double neighbourAverageRating = getUserAverageRatings(ratingsMatrix, neighbour, mae);
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

		public static void DoubleAdd(ref double location1, double value1, ref double location2, double value2)
		{
			Add(ref location1, value1);
			Add(ref location2, value2);
		}

		public static double Add(ref double location1, double value)
		{
			double newCurrentValue = 0;
			while (true)
			{
				double currentValue = newCurrentValue;
				double newValue = currentValue + value;
				newCurrentValue = Interlocked.CompareExchange(ref location1, newValue, currentValue);
				if (newCurrentValue == currentValue)
					return newValue;
			}
		}

		public Dictionary<int, double> CalculateUsersAverageRatings(Matrix<int> ratingsMatrix)
		{
			foreach (int user in ratingsMatrix.Rows)
				getUserAverageRatings(ratingsMatrix, user, false);

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

		private double getUserAverageRatings(Matrix<int> ratingsMatrix, int user, bool mae)
		{
			if (mae || !this.usersAverageRatings.ContainsKey(user))
			{
				double averageRating = 0;

				if (ratingsMatrix.Rows.Contains(user) && ratingsMatrix[user].Count() > 0)
				{
					foreach (int item in ratingsMatrix[user])
						averageRating += ratingsMatrix[user, item];

					averageRating /= ratingsMatrix[user].Count();
				}

				if (mae)
					return averageRating;

				this.usersAverageRatings[user] = averageRating;
			}

			return this.usersAverageRatings[user];
		}                   		

	}
}
