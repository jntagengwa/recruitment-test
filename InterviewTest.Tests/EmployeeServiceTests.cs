

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using InterviewTest.Server.Data;
using InterviewTest.Server.DTOs;
using InterviewTest.Server.Mappings;
using InterviewTest.Server.Model;
using InterviewTest.Server.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace InterviewTest.Tests
{
    public class EmployeeServiceTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            _db = new AppDbContext(options);
            _db.Database.EnsureCreated();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _service = new EmployeeService(_db, _mapper, NullLogger<EmployeeService>.Instance);

            Seed();
        }

        private void Seed()
        {
            _db.Employees.AddRange(new List<Employee>
            {
                new() { Name = "Alice", Value = 4000 },
                new() { Name = "Bob", Value = 4000 },
                new() { Name = "Charlie", Value = 4000 },
                new() { Name = "Eddie", Value = 10 },
                new() { Name = "Gina", Value = 20 },
                new() { Name = "Zara", Value = 30 },
            });
            _db.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSeededEmployees()
        {
            var result = await _service.GetAllAsync();
            Assert.True(result.Count >= 6);
        }

        [Fact]
        public async Task CreateAsync_CreatesEmployee()
        {
            var dto = new EmployeeCreateDto { Name = "New Guy", Value = 123 };
            var created = await _service.CreateAsync(dto);
            Assert.True(created.Id > 0);
            Assert.Equal("New Guy", created.Name);
            Assert.Equal(123, created.Value);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesEmployee()
        {
            var first = await _db.Employees.AsNoTracking().FirstAsync();
            var newName = first.Name + " Updated";
            var newValue = first.Value + 123;

            var ok = await _service.UpdateAsync(first.Id, new EmployeeUpdateDto { Name = newName, Value = newValue });
            Assert.True(ok);

            var updated = await _db.Employees.AsNoTracking().FirstAsync(e => e.Id == first.Id);
            Assert.Equal(newName, updated.Name);
            Assert.Equal(newValue, updated.Value);
        }

        [Fact]
        public async Task DeleteAsync_DeletesEmployee()
        {
            var first = await _db.Employees.FirstAsync();
            var ok = await _service.DeleteAsync(first.Id);
            Assert.True(ok);

            var deleted = await _db.Employees.FindAsync(first.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task IncrementValuesAndGetAbcSumAsync_ReturnsExpectedTotal()
        {
            var before = await _db.Employees.SumAsync(e => e.Name.StartsWith("A") || e.Name.StartsWith("B") || e.Name.StartsWith("C") ? e.Value : 0);
            Assert.Equal(12000, before);

            var total = await _service.IncrementValuesAndGetAbcSumAsync();
            Assert.True(total.HasValue);
            Assert.Equal(12300, total.Value);
        }

        public void Dispose()
        {
            _db.Dispose();
            _connection.Close();
            _connection.Dispose();
        }
    }
}