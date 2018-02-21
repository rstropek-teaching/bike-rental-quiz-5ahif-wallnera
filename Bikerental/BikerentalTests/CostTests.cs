using Bikerental;
using Bikerental.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BikerentalTests
{
    [TestClass]
    public class CostTests
    {

        // Create a TestBike 
        public Bike TestBike = new Bike {
            BikeCategory = Category.Mountainbike,
            DateOfLastService = DateTime.Parse("1.9.2017"),
            Notes = "This is a Testbike",
            PurchaseDate = DateTime.Parse("1.3.2016"),
            RentalPriceAdditionalHour = 5,
            RentalPriceFirstHour = 3
        };
        public BikeRentalLogic logic = new BikeRentalLogic();

        // should be one hour + two additional hours
        [TestMethod]
        public void AdditionalHourTest()
        {
            Rental rental = new Rental
            {
                Bike = TestBike,
                RentalBegin = new DateTime(2018, 2, 14, 8, 15, 00),
                RentalEnd = new DateTime(2018, 2, 14, 10, 30, 00),
                Paid = false
            };
            
            double cost = logic.Calculate(rental);
            Assert.AreEqual(13, cost);
        }

        // should be only one hour
        [TestMethod]
        public void OnlyFirstHourTest()
        {
            Rental rental = new Rental
            {
                Bike = TestBike,
                RentalBegin = new DateTime(2018, 2, 14, 8, 15, 00),
                RentalEnd = new DateTime(2018, 2, 14, 8, 45, 00),
                Paid = false
            };

            double cost = logic.Calculate(rental);
            Assert.AreEqual(3, cost);
        }

        // should be free because it is less than 15 minutes
        [TestMethod]
        public void FreeTest()
        {
            Rental rental = new Rental
            {
                Bike = TestBike,
                RentalBegin = new DateTime(2018, 2, 14, 8, 15, 00),
                RentalEnd = new DateTime(2018, 2, 14, 8, 25, 00),
                Paid = false
            };

            double cost = logic.Calculate(rental);
            Assert.AreEqual(0, cost);
        }
    }
}
