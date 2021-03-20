using System;
using FluentAssertions;
using GappedDateTime;
using NUnit.Framework;

namespace GappedDateTimeTests
{
    public class Tests
    {
        private GappedDateTimeCalculator _dtCalc;

        [SetUp]
        public void Setup()
        {
            _dtCalc = new GappedDateTimeCalculator(
                new GapDescriptor(DayOfWeek.Friday, 21),
                new TimeSpan(46, 0, 0)
            );
        }

        [Test]
        public void Test1()
        {
            // ARRANGE
            var dt = DateTime.Parse("2021-03-19 20:59:55");
            
            // ACT
            var r = _dtCalc.Add(dt, TimeSpan.FromSeconds(10));
            
            // ASSERT
            r.Should().Be(DateTime.Parse("2021-03-21 19:00:05"));
        }
        
        [Test]
        public void Test2()
        {
            // ARRANGE
            var dt = DateTime.Parse("2021-03-19 23:59:55"); 
            
            // ACT
            var r = _dtCalc.Add(dt, new TimeSpan(9, 0, 0, 10));
            
            // ASSERT
            r.Should().Be(DateTime.Parse("2021-04-01 17:00:10"));
        }

        [Test]
        public void Test3()
        {
            // ARRANGE
            var dt = DateTime.Parse("2021-03-19 20:59:55"); 
            
            // ACT
            var r = _dtCalc.Add(dt, new TimeSpan(9, 0, 0, 10));
            
            // ASSERT
            r.Should().Be(DateTime.Parse("2021-03-30 19:00:05"));
        }

    }
}