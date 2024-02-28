using Northwind.Shared;

namespace Northwind.Common.UnitTestsXUnit
{
    public class EntityModelTests
    {
        [Fact]
        public void DatabaseConnectTest()
        {
            using (NorthwindContext db = new())
            {
                Assert.True(db.Database.CanConnect());
            }
        }

        [Fact]
        public void DatabaseUpdateTest()
        {
            using (NorthwindContext db = new())
            {
                int expected = 8;
                int actual = db.Categories.Count();

                Assert.Equal(expected, actual);
            }
        }
    }
}