using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using MyNamespace.Controllers;
using static MyNamespace.Controllers.EntityController;

namespace MyNamespace.Tests.Controllers
{
    public class EntityControllerTests
    {
        [Fact]
        public void UpdateEntity_RetryAndBackoff_Success()
        {
            // Arrange
            var controller = new EntityController();
            var entityToUpdate = new Entity
            {
                Id = "1",
                Addresses = new List<Address>
                {
                    new Address { AddressLine = "123 Main St", City = "City1", Country = "Country1" }
                },
                Dates = new List<Date>
                {
                    new Date { DateType = "Birth", DateValue = new DateTime(1990, 1, 1) }
                },
                Deceased = false,
                Gender = "Male",
                Names = new List<Name>
                {
                    new Name { FirstName = "John", LastName = "Doe" }
                }
            };
            var id = "1";

            // Mock database write failure for the first 2 attempts
            var attemptCount = 0;
            Mock<ILogger<EntityController>> loggerMock = new Mock<ILogger<EntityController>>();
loggerMock.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
    .Callback<LogLevel, EventId, object, Exception, Func<object, Exception, string>>((level, id, state, exception, formatter) =>
    {
        if (state != null && state.ToString()?.Contains("Retry attempt") == true)
        {
            attemptCount++;
            if (attemptCount <= 2)
            {
                throw new DatabaseWriteException("Simulated database write failure.");
            }
        }
    });

            // Act
            var result = controller.UpdateEntity(id, entityToUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
