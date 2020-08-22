﻿using AutoMapper;
using ITAcademy.TaskTwo.Data.Interfaces;
using ITAcademy.TaskTwo.Data.Models;
using ITAcademy.TaskTwo.Logic.Models.SubjectDTO;
using ITAcademy.TaskTwo.Logic.Profiles;
using ITAcademy.TaskTwo.Logic.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ITAcademy.TaskTwo.Tests
{
    public class SubjectServiceTests
    {
        [Theory]
        [InlineData("Химия")]
        [InlineData("химия")]
        [InlineData("Физика")]
        [InlineData("Математика")]
        public void ExistsName_ReturnsTrueResult(string testName)
        {
            var mockMap = new Mock<IMapper>();

            var mockRep = new Mock<ISubjectRepository>();
            mockRep.Setup(repo => repo.ExistsName(testName)).Returns(
                GetTestSubjects().Any(p => p.Name.Equals(testName, StringComparison.OrdinalIgnoreCase)));
            var mockUnit = new Mock<IUnitOfWork>();
            mockUnit.Setup(unit => unit.SubjectRepo).Returns(mockRep.Object);

            var service = new SubjectService(mockUnit.Object, mockMap.Object);

            // Act
            var result = service.ExistsName(testName);

            // Assert
            Assert.IsType<bool>(result);
            Assert.True(result);
        }

        [Theory]
        [InlineData("Химея")]
        [InlineData("ФизикаР")]
        [InlineData("Матем")]
        public void ExistsName_ReturnsFalseResult(string testName)
        {
            var mockMap = new Mock<IMapper>();

            var mockRep = new Mock<ISubjectRepository>();
            mockRep.Setup(repo => repo.ExistsName(testName)).Returns(
                GetTestSubjects().Any(p => p.Name == testName));
            var mockUnit = new Mock<IUnitOfWork>();
            mockUnit.Setup(unit => unit.SubjectRepo).Returns(mockRep.Object);

            var service = new SubjectService(mockUnit.Object, mockMap.Object);

            // Act
            var result = service.ExistsName(testName);

            // Assert
            Assert.IsType<bool>(result);
            Assert.False(result);
        }

        [Fact]
        public async Task GetEmployeesOfSubject_ReturnsSubjectWithEmployees()
        {
            var testId = 1;
            var model = GetTestSubjects().FirstOrDefault(p => p.Id == testId);
            var mockMap = new MapperConfiguration(cfg => cfg.AddProfiles(
                new List<Profile>() { new EmployeeDtoProfile(), new SubjectDtoProfile() }));
            var mapper = mockMap.CreateMapper();

            var mockRep = new Mock<IEmployeeRepository>();
            mockRep.Setup(repo => repo.GetAllAsync()).ReturnsAsync(GetTestEmployees());
            var mockUnit = new Mock<IUnitOfWork>();
            mockUnit.Setup(unit => unit.EmployeeRepo).Returns(mockRep.Object);

            var service = new SubjectService(mockUnit.Object, mapper);

            // Act
            var result = await service.GetEmployeesOfSubjectAsync(model);

            // Assert
            Assert.IsType<SubjectWithEmployees>(result);
            Assert.Equal(testId, result.Id);
            Assert.Equal("Химия", result.Name);
        }

        [Fact]
        public async Task GetAllSubjectsWithEmployees_ReturnsSubjectsWithEmployees()
        {
            var mockMap = new MapperConfiguration(cfg => cfg.AddProfiles(
                new List<Profile>() { new EmployeeDtoProfile(), new SubjectDtoProfile() }));
            var mapper = mockMap.CreateMapper();

            var mockRep = new Mock<ISubjectRepository>();
            mockRep.Setup(repo => repo.GetAllSubjectsWithEmployeesAsync()).ReturnsAsync(
                GetTestSubjects());
            var mockUnit = new Mock<IUnitOfWork>();
            mockUnit.Setup(unit => unit.SubjectRepo).Returns(mockRep.Object);

            var service = new SubjectService(mockUnit.Object, mapper);

            // Act
            var result = await service.GetAllSubjectsWithEmployeesAsync();

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Theory]
        [InlineData("Химия")]
        [InlineData("Физика")]
        [InlineData("Математика")]
        public async Task GetSubjectWithEmployees_ReturnsCorrectObjectTypeAndNameProperty(string testName)
        {
            var mockMap = new MapperConfiguration(cfg => cfg.AddProfiles(
                new List<Profile>() { new EmployeeDtoProfile(), new SubjectDtoProfile() }));
            var mapper = mockMap.CreateMapper();

            var mockRep = new Mock<ISubjectRepository>();
            mockRep.Setup(repo => repo.GetSubjectWithEmployeesAsync(testName)).ReturnsAsync(
                GetTestSubjects().FirstOrDefault(p => p.Name == testName));

            var mockUnit = new Mock<IUnitOfWork>();
            mockUnit.Setup(unit => unit.SubjectRepo).Returns(mockRep.Object);

            var service = new SubjectService(mockUnit.Object, mapper);

            // Act
            var result = await service.GetSubjectWithEmployeesAsync(testName);

            // Assert
            Assert.IsType<SubjectWithEmployeesDto>(result);
            Assert.Equal(testName, result.Name);
        }

        [Fact]
        public async Task GetSubjectWithEmployees_ReturnsSubjectWithEmployees()
        {
            var testName = "Химия";
            var mockMap = new MapperConfiguration(cfg => cfg.AddProfiles(
                new List<Profile>() { new EmployeeDtoProfile(), new SubjectDtoProfile() }));
            var mapper = mockMap.CreateMapper();

            var mockRep = new Mock<ISubjectRepository>();
            mockRep.Setup(repo => repo.GetSubjectWithEmployeesAsync(testName)).ReturnsAsync(
                GetTestSubjects().FirstOrDefault(p => p.Name == testName));
            var mockUnit = new Mock<IUnitOfWork>();
            mockUnit.Setup(unit => unit.SubjectRepo).Returns(mockRep.Object);

            var service = new SubjectService(mockUnit.Object, mapper);

            // Act
            var result = await service.GetSubjectWithEmployeesAsync(testName);

            // Assert            
            Assert.Equal(1, result.Id);
            Assert.Equal("Иванов", result.Employees.FirstOrDefault(aed => aed.Id == 2).SurName);
            Assert.Equal("Иван", result.Employees.FirstOrDefault(aed => aed.Id == 2).FirstName);
            Assert.Equal("Иванович", result.Employees.FirstOrDefault(aed => aed.Id == 2).SecondName);
            Assert.Equal("ivan@yandex.ru", result.Employees.FirstOrDefault(aed => aed.Id == 2).Email);
            Assert.Contains("+375331450000", result.Employees.FirstOrDefault(aed => aed.Id == 2).Phones);
            Assert.Contains("+79167882020", result.Employees.FirstOrDefault(aed => aed.Id == 2).Phones);
            Assert.Equal("Зужик", result.Employees.FirstOrDefault(aed => aed.Id == 3).SurName);
            Assert.Equal("Марфа", result.Employees.FirstOrDefault(ep => ep.Id == 3).FirstName);
            Assert.Equal("Гавриловна", result.Employees.FirstOrDefault(ep => ep.Id == 3).SecondName);
            Assert.Equal("zuwik@gmail.com", result.Employees.FirstOrDefault(ep => ep.Id == 3).Email);
            Assert.Contains("+375291234567", result.Employees.FirstOrDefault(aed => aed.Id == 3).Phones);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetDetails_ReturnsCorrectObjectTypeAndIdProperty(int testId)
        {
            var mockMap = new Mock<IMapper>();
            var mockRep = new Mock<ISubjectRepository>();
            mockRep.Setup(repo => repo.GetDetailsAsync(testId)).ReturnsAsync(
                GetTestSubjects().FirstOrDefault(p => p.Id == testId));
            var mockUnit = new Mock<IUnitOfWork>();
            mockUnit.Setup(unit => unit.SubjectRepo).Returns(mockRep.Object);

            var service = new SubjectService(mockUnit.Object, mockMap.Object);

            // Act
            var result = await service.GetDetailsAsync(testId);

            // Assert
            Assert.IsType<Subject>(result);
            Assert.Equal(testId, result.Id);
        }

        [Fact]
        public async Task GetDetails_ReturnsSubjectWithEmployees()
        {
            var testId = 1;
            var mockMap = new Mock<IMapper>();
            var mockRep = new Mock<ISubjectRepository>();
            mockRep.Setup(repo => repo.GetDetailsAsync(testId)).ReturnsAsync(
                GetTestSubjects().FirstOrDefault(p => p.Id == testId));
            var mockUnit = new Mock<IUnitOfWork>();
            mockUnit.Setup(unit => unit.SubjectRepo).Returns(mockRep.Object);

            var service = new SubjectService(mockUnit.Object, mockMap.Object);

            // Act
            var result = await service.GetDetailsAsync(testId);

            // Assert
            Assert.IsType<Subject>(result);
            Assert.Equal(testId, result.Id);
            Assert.Equal("Химия", result.Name);
            Assert.Equal("Иванов", result.Assignments.FirstOrDefault(ep => ep.EmployeeId == 2).Employee.SurName);
            Assert.Equal("Иван", result.Assignments.FirstOrDefault(ep => ep.EmployeeId == 2).Employee.FirstName);
            Assert.Equal("Иванович", result.Assignments.FirstOrDefault(ep => ep.EmployeeId == 2).Employee.SecondName);
            Assert.Equal("ivan@yandex.ru", result.Assignments.FirstOrDefault(ep => ep.EmployeeId == 2).Employee.Email);
            Assert.Equal("Зужик", result.Assignments.FirstOrDefault(ep => ep.EmployeeId == 3).Employee.SurName);
            Assert.Equal("Марфа", result.Assignments.FirstOrDefault(ep => ep.EmployeeId == 3).Employee.FirstName);
            Assert.Equal("Гавриловна", result.Assignments.FirstOrDefault(ep => ep.EmployeeId == 3).Employee.SecondName);
            Assert.Equal("zuwik@gmail.com", result.Assignments.FirstOrDefault(ep => ep.EmployeeId == 3).Employee.Email);
        }

        private List<Subject> GetTestSubjects()
        {
            var subjects = new List<Subject>()
            {
                new Subject
                {
                    Id = 1,
                    Name = "Химия",
                    Assignments = new List<EmployeeSubject>()
                    {
                        new EmployeeSubject { SubjectId = 1, EmployeeId = 2, Employee = new Employee
                        {
                            Id = 2,
                            SurName = "Иванов",
                            FirstName = "Иван",
                            SecondName = "Иванович",
                            Email = "ivan@yandex.ru",
                            Phones = new List<Phone>()
                            {
                                new Phone { Id = 1, Number = "+375331450000", EmployeeId = 2 },
                                new Phone { Id = 2, Number = "+79167882020", EmployeeId = 2 }
                            },                            
                        }},
                        new EmployeeSubject { SubjectId = 1, EmployeeId = 3, Employee = new Employee
                        {
                            Id = 3,
                            SurName = "Зужик",
                            FirstName = "Марфа",
                            SecondName = "Гавриловна",
                            Email = "zuwik@gmail.com",
                            Phones = new List<Phone>()
                            {
                                new Phone { Id = 3, Number = "+375291234567", EmployeeId = 3 }
                            },
                        }}
                    }
                },
                new Subject
                {
                    Id = 2,
                    Name = "Физика",
                    Assignments = new List<EmployeeSubject>()
                },
                new Subject
                {
                    Id = 3,
                    Name = "Математика",
                    Assignments = new List<EmployeeSubject>()
                }
            };
            return subjects;
        }

        private List<Employee> GetTestEmployees()
        {
            var employees = new List<Employee>()
            {
                new Employee
                {
                    Id = 1,
                    SurName = "Петров",
                    FirstName = "Петр",
                    SecondName = "Петрович",
                    Email = "big_boss@yandex.ru",
                },
                new Employee
                {
                    Id = 2,
                    SurName = "Иванов",
                    FirstName = "Иван",
                    SecondName = "Иванович",
                    Email = "ivan@tut.by",
                },
                new Employee
                {
                    Id = 3,
                    SurName = "Зужик",
                    FirstName = "Марфа",
                    SecondName = "Гавриловна",
                    Email = "zuwik@gmail.com",
                },
            };
            return employees;
        }
    }
}
