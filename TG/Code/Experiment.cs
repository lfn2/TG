using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG
{
	public class Experiment
	{

		public static double MeanAverageError(Matrix<int> ratingsMatrix, Matrix<float> weightsMatrix)
		{

			int ratings = 0;
			double mae = 0;

			foreach (int user in ratingsMatrix.Rows)
			{
				List<int> userRatedItems = ratingsMatrix[user].ToList<int>();
				foreach (int item in userRatedItems)
				{
					int originalRating = ratingsMatrix[user, item];
					ratingsMatrix.remove(user, item);

					int predictedRating = RatingPredictor.PredictRating(ratingsMatrix, weightsMatrix, user, item);				
					
					ratings++;

					mae = (mae * ((double)(ratings - 1) / ratings)) + ((double)predictedRating / ratings);

					ratingsMatrix[user, item] = originalRating;
				}
			}
			
			return mae;
		}

	}
}
