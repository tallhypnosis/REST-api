using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using static MyNamespace.Controllers.Entity;
using System.Globalization;

namespace MyNamespace.Controllers
{
    [Route("api/entities")]
    [ApiController]
    public class EntityController : ControllerBase
    {
        private readonly List<Entity> _entities;

        public EntityController()
        {
            // Initialize mock data
            _entities = new List<Entity>
            {
                new Entity
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
                },
                new Entity
                {
                    Id = "2",
                    Addresses = new List<Address>
                    {
                        new Address { AddressLine = "456 Elm St", City = "City2", Country = "Country2" }
                    },
                    Dates = new List<Date>
                    {
                        new Date { DateType = "Birth", DateValue = new DateTime(1985, 5, 10) }
                    },
                    Deceased = true,
                    Gender = "Female",
                    Names = new List<Name>
                    {
                        new Name { FirstName = "Alice", LastName = "Smith" }
                    }
                }
            };
        }

        [HttpGet]
        public IActionResult GetEntities([FromQuery] string search,
                                          [FromQuery] string gender,
                                          [FromQuery] DateTime? startDate,
                                          [FromQuery] DateTime? endDate,
                                          [FromQuery] string[] countries,
                                          [FromQuery] string sortBy,
                                          [FromQuery] string sortOrder,
                                          [FromQuery] int page,
                                          [FromQuery] int pageSize
                                          )
        {
            var filteredEntities = _entities;

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
{
    search = search.ToLower();
    filteredEntities = _entities.Where(entity =>
        (entity.Addresses?.Any(address =>
            (address?.Country?.ToLower().Contains(search) == true) ||
            (address?.AddressLine?.ToLower().Contains(search) == true)) ?? false) ||
        (entity.Names?.Any(name =>
            (name?.FirstName?.ToLower().Contains(search) == true) ||
            (name?.MiddleName?.ToLower().Contains(search) == true) ||
            (name?.LastName?.ToLower().Contains(search) == true)) ?? false)
    ).ToList();
}

            // Gender filter
            if (!string.IsNullOrWhiteSpace(gender))
            {
                filteredEntities = filteredEntities.Where(entity =>
                    string.Equals(entity.Gender, gender, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Date range filter
            if (startDate.HasValue)
            {
                filteredEntities = filteredEntities.Where(entity =>
                    entity.Dates.Any(date =>
                        date.DateValue != null && date.DateValue >= startDate.Value)
                ).ToList();
            }

            if (endDate.HasValue)
            {
                filteredEntities = filteredEntities.Where(entity =>
                    entity.Dates.Any(date =>
                        date.DateValue != null && date.DateValue <= endDate.Value)
                ).ToList();
            }

            // Country filter
            if (countries != null && countries.Any())
            {
                filteredEntities = filteredEntities.Where(entity =>
                    entity.Addresses?.Any(address =>
                        countries.Contains(address?.Country, StringComparer.OrdinalIgnoreCase)
                    ) == true
                ).ToList();
            }

            // Sorting
            var property = typeof(Entity).GetProperty(sortBy);
            if (property != null)
            {
                filteredEntities = sortOrder.ToLower() == "asc" ?
                                   filteredEntities.OrderBy(e => property.GetValue(e, null)?.ToString()).ToList():
                                   filteredEntities.OrderByDescending(e => property.GetValue(e, null)?.ToString()).ToList();
            }

            // Pagination
            var totalCount = filteredEntities.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var currentPageEntities = filteredEntities.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = page,
                Entities = currentPageEntities
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetEntityById(string id)
        {
            var entity = _entities.FirstOrDefault(e => e.Id == id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEntity(string id, Entity entity)
        {
            try
            {
                RetryWithBackoff(() =>
                {
                    var existingEntity = _entities.FirstOrDefault(e => e.Id == id);
                    if (existingEntity == null)
                    {
                        return NotFound();
                    }

                    // Simulate database write failure randomly
                    if (new Random().Next(10) < 3) // Simulate failure 30% of the time
                    {
                        throw new DatabaseWriteException("Database write failed.");
                    }

                    // Update entity
                    existingEntity.Addresses = entity.Addresses;
                    existingEntity.Dates = entity.Dates;
                    existingEntity.Deceased = entity.Deceased;
                    existingEntity.Gender = entity.Gender;
                    existingEntity.Names = entity.Names;

                    return NoContent();
                });
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
            return NoContent();
        }

        private void RetryWithBackoff(Func<IActionResult> action)
        {
            int maxRetries = 3;
            int retryDelayMilliseconds = 1000; // Initial delay
            int maxRetryDelayMilliseconds = 10000; // Maximum delay
            Random random = new Random();

            for (int retryAttempt = 1; retryAttempt <= maxRetries; retryAttempt++)
            {
                try
                {
                    // Execute the action
                    var result = action();

                    // If successful, exit the loop;
                    if (result is StatusCodeResult || result is NoContentResult)
                    {
                        return;
                    }
                }
                catch (DatabaseWriteException ex)
                {
                    if (retryAttempt == maxRetries)
                    {
                        // Reached maximum
                                            // retry attempts, rethrow the exception
                        throw;
                    }

                    Console.WriteLine($"An error occurred: {ex.Message}");

                    // Calculate delay using exponential backoff strategy
                    int delayMilliseconds = Math.Min(retryDelayMilliseconds * (int)Math.Pow(2, retryAttempt - 1), maxRetryDelayMilliseconds);
                    // Add jitter to delay to prevent synchronized retries
                    delayMilliseconds += random.Next(0, delayMilliseconds / 2);

                    // Log delay before next retry
                    Console.WriteLine($"Delaying {delayMilliseconds / 1000} seconds before retrying...");

                    // Wait for the calculated delay before retrying 
                    System.Threading.Thread.Sleep(delayMilliseconds);
                }
            }
        }

        public class DatabaseWriteException : Exception
        {
            public DatabaseWriteException(string message) : base(message) { }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEntity(string id)
        {
            var entityToRemove = _entities.FirstOrDefault(e => e.Id == id);
            if (entityToRemove == null)
            {
                return NotFound();
            }
            _entities.Remove(entityToRemove);
            return NoContent();
        }
    }
}
