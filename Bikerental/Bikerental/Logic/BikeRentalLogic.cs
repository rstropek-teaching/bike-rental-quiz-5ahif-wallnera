using Bikerental.Model;
using System;

namespace Bikerental
{
    public class BikeRentalLogic
    {
        /// <summary>
        /// calculates the costs 
        /// </summary>
        /// <param name="rent"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public double Calculate(Rental rent)
        {
            TimeSpan t = (DateTime)rent.RentalEnd - rent.RentalBegin;
            double min = t.TotalMinutes;
            double fee = 0.00d;

            if (min <= 15)
            {
                // free
                return 0.00d;
            }
            else
            {
                // pay
                fee += rent.Bike.RentalPriceFirstHour;
                min -= 60;

                while (min > 0)
                {
                    fee += rent.Bike.RentalPriceAdditionalHour;
                    min -= 60;
                }
                return fee;
            }
        }
    }
}
