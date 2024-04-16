using NUnit.Framework;
using Moq;
using Permissions.BL.Models;
using Permissions.BL.Repositories;
using Permissions.BL.Services.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Permissions.BL.Repositories.UnitOfWork;

namespace Permissions.API.Tests.UnitsTests
{
    [TestFixture]
    public class GetPermissionsServiceTests
    {
        [Test]
        public async Task GetPermissions_ReturnsListOfPermissions()
        {
            // Arrange
            var expectedPermissions = new List<Permission>
            {
                new Permission { Id = 1, EmployeeForename = "John", EmployeeSurname = "Doe", PermissionType = 1, PermissionDate = DateTime.Now },
                new Permission { Id = 2, EmployeeForename = "Jane", EmployeeSurname = "Smith", PermissionType = 2, PermissionDate = DateTime.Now }
            };
            var mockLogger = new Mock<ILogger>();
            var mockPermissionRepository = new Mock<IPermissionRepository>();
            mockPermissionRepository.Setup(repo => repo.GetAll()).ReturnsAsync(expectedPermissions);

            var service = new GetPermissionsService(mockPermissionRepository.Object, null, null, mockLogger.Object);

            // Act
            var result = await service.GetPermissions();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedPermissions.Count, result.Count());
            Assert.IsTrue(expectedPermissions.SequenceEqual(result));
        }

        [Test]
        public async Task GetPermissionById_ReturnsPermission()
        {
            // Arrange
            var expectedPermission = new Permission { Id = 1, EmployeeForename = "John", EmployeeSurname = "Doe", PermissionType = 1, PermissionDate = DateTime.Now };

            var mockLogger = new Mock<ILogger>();
            var mockPermissionRepository = new Mock<IPermissionRepository>();
            mockPermissionRepository.Setup(repo => repo.GetById(1)).ReturnsAsync(expectedPermission);

            var service = new GetPermissionsService(mockPermissionRepository.Object, null, null, mockLogger.Object);

            // Act
            var result = await service.GetPermissionById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedPermission.Id, result.Id);
            Assert.AreEqual(expectedPermission.EmployeeForename, result.EmployeeForename);
            Assert.AreEqual(expectedPermission.EmployeeSurname, result.EmployeeSurname);
            Assert.AreEqual(expectedPermission.PermissionType, result.PermissionType);
            Assert.AreEqual(expectedPermission.PermissionDate, result.PermissionDate);
        }

        [Test]
        public async Task GetPermissionById_ThrowsExceptionWhenPermissionNotFound()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var mockPermissionRepository = new Mock<IPermissionRepository>();
            mockPermissionRepository.Setup(repo => repo.GetById(It.IsAny<int>())).ReturnsAsync((Permission)null);
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var service = new GetPermissionsService(mockPermissionRepository.Object, mockUnitOfWork.Object, null, mockLogger.Object);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => service.GetPermissionById(1));
        }

        [Test]
        public async Task GetPermissions_ReturnsOnlyPermissionsOfSpecifiedType()
        {
            // Arrange
            var expectedPermissions = new List<Permission>
            {
                new Permission { Id = 1, PermissionType = 1 },
                new Permission { Id = 2, PermissionType = 1 },
                new Permission { Id = 3, PermissionType = 2 }
            };

            var mockLogger = new Mock<ILogger>();
            var mockPermissionRepository = new Mock<IPermissionRepository>();
            mockPermissionRepository.Setup(repo => repo.GetAll()).ReturnsAsync(expectedPermissions);

            var service = new GetPermissionsService(mockPermissionRepository.Object, null, null, mockLogger.Object);

            // Act
            var result = await service.GetPermissions();
            var permissionsOfType1 = result.Where(p => p.PermissionType == 1).ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, permissionsOfType1.Count()); // Only permissions of type 1 are expected
        }

        [Test]
        public async Task GetPermissions_CallsGetAllMethodOfRepository()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var mockPermissionRepository = new Mock<IPermissionRepository>();
            var service = new GetPermissionsService(mockPermissionRepository.Object, null, null, mockLogger.Object);

            // Act
            await service.GetPermissions();

            // Assert
            mockPermissionRepository.Verify(repo => repo.GetAll(), Times.Once);
        }

    }
}
