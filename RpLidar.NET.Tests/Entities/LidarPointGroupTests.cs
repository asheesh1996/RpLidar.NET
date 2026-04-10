using RpLidar.NET.Entities;
using Xunit;

namespace RpLidar.NET.Tests.Entities
{
    public class LidarPointGroupTests
    {
        private static LidarPoint MakePoint(float angle, float distance) =>
            new LidarPoint { Angle = angle, Distance = distance };

        [Fact]
        public void Count_AfterAddingFourPoints_IsFour()
        {
            var group = new LidarPointGroup(0, 0);
            group.Add(MakePoint(0f, 1000f));
            group.Add(MakePoint(90f, 2000f));
            group.Add(MakePoint(180f, 3000f));
            group.Add(MakePoint(270f, 4000f));

            Assert.Equal(4, group.Count);
        }

        [Fact]
        public void Indexer_ByIntAngle_ReturnsCorrectDistance()
        {
            var group = new LidarPointGroup(0, 0);
            group.Add(MakePoint(0f, 1000f));
            group.Add(MakePoint(90f, 2000f));
            group.Add(MakePoint(180f, 3000f));
            group.Add(MakePoint(270f, 4000f));

            var item90 = group[90];
            Assert.NotNull(item90);
            Assert.Equal(2000f, item90.Distance, precision: 0);

            var item0 = group[0];
            Assert.NotNull(item0);
            Assert.Equal(1000f, item0.Distance, precision: 0);
        }

        [Fact]
        public void GetPoints_ReturnsFourPoints()
        {
            var group = new LidarPointGroup(0, 0);
            group.Add(MakePoint(0f, 1000f));
            group.Add(MakePoint(90f, 2000f));
            group.Add(MakePoint(180f, 3000f));
            group.Add(MakePoint(270f, 4000f));

            var points = group.GetPoints();

            Assert.Equal(4, points.Count);
        }

        [Fact]
        public void Add_DuplicateAngle_KeepsNearestDistance()
        {
            var group = new LidarPointGroup(0, 0);
            group.Add(MakePoint(45f, 3000f));
            group.Add(MakePoint(45f, 1500f));

            var item = group[45];
            Assert.NotNull(item);
            Assert.Equal(1500f, item.Distance, precision: 0);
        }

        [Fact]
        public void Add_DuplicateAngle_DoesNotReplaceWithFartherDistance()
        {
            var group = new LidarPointGroup(0, 0);
            group.Add(MakePoint(45f, 1500f));
            group.Add(MakePoint(45f, 3000f));

            var item = group[45];
            Assert.NotNull(item);
            Assert.Equal(1500f, item.Distance, precision: 0);
        }

        [Fact]
        public void Add_InvalidPoint_IsIgnored()
        {
            var group = new LidarPointGroup(0, 0);
            group.Add(MakePoint(45f, 0f));

            Assert.Equal(0, group.Count);
        }

        [Fact]
        public void Add_NullPoint_IsIgnored()
        {
            var group = new LidarPointGroup(0, 0);
            group.Add(null!);

            Assert.Equal(0, group.Count);
        }

        [Fact]
        public void Indexer_NonExistentAngle_ReturnsNull()
        {
            var group = new LidarPointGroup(0, 0);
            group.Add(MakePoint(90f, 2000f));

            Assert.Null(group[45]);
        }

        [Fact]
        public void AddRange_AddsAllValidPoints()
        {
            var group = new LidarPointGroup(0, 0);
            var points = new[]
            {
                MakePoint(10f, 500f),
                MakePoint(20f, 600f),
                MakePoint(30f, 700f),
            };

            group.AddRange(points);

            Assert.Equal(3, group.Count);
        }
    }
}
