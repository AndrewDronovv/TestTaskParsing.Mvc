using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TestTaskParsing.Domain;
using Xunit;

namespace TestTaskParsing.Tests
{
    public class EmployeeControllerTests
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly EmployeeController _controller;

        public EmployeeControllerTests()
        {
            _mockContext = new Mock<AppDbContext>();
            _controller = new EmployeeController(_mockContext.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult_WithListOfEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, Surname = "Smith", DateOfBirth = new DateTime(1980, 1, 1) },
                new Employee { Id = 2, Surname = "Jones", DateOfBirth = new DateTime(1990, 5, 5) }
            }.AsQueryable();

            var mockSet = employees.AsMockDbSet();
            _mockContext.Setup(c => c.Employees).Returns(mockSet.Object);

            // Act
            var result = _controller.Index(null, null, null) as ViewResult;

            // Assert
            var model = Assert.IsAssignableFrom<IEnumerable<Employee>>(result.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Edit_ValidId_ReturnsViewResult_WithEmployee()
        {
            // Arrange
            var employee = new Employee { Id = 1, Surname = "Smith" };
            var mockSet = new List<Employee> { employee }.AsMockDbSet();
            _mockContext.Setup(c => c.Employees).Returns(mockSet.Object);
            _mockContext.Setup(c => c.Employees.Find(1)).Returns(employee);

            // Act
            var result = _controller.Edit(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(employee, result.Model);
        }

        [Fact]
        public void DeletePost_ValidId_DeletesEmployee()
        {
            // Arrange
            var employee = new Employee { Id = 1, Surname = "Smith" };
            var mockSet = new List<Employee> { employee }.AsMockDbSet();
            _mockContext.Setup(c => c.Employees).Returns(mockSet.Object);
            _mockContext.Setup(c => c.Employees.Find(1)).Returns(employee);

            // Act
            var result = _controller.DeletePost(1) as RedirectToActionResult;

            // Assert
            _mockContext.Verify(m => m.Employees.Remove(employee), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
            Assert.Equal("Index", result.ActionName);
        }
    }

    public static class DbSetMockingExtensions
    {
        public static Mock<DbSet<T>> AsMockDbSet<T>(this IEnumerable<T> data) where T : class
        {
            var queryableData = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());
            return mockSet;
        }
    }
}
