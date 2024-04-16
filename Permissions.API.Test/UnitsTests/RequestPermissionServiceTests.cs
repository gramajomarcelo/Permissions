using NUnit.Framework;
using Moq;
using Permissions.BL.Models;
using Permissions.BL.Repositories.UnitOfWork;
using Permissions.BL.Repositories;
using Permissions.BL.Services.Implements;
using System;
using System.Threading.Tasks;
using Serilog;

namespace Permissions.API.Tests.UnitsTests
{
    [TestFixture]
    public class RequestPermissionServiceTests
    {
        [Test]
        public async Task RequestPermission_CreatesNewPermission()
        {
            // Arrange
            var permissionToCreate = new Permission { EmployeeForename = "John", EmployeeSurname = "Doe" };

            var mockPermissionRepository = new Mock<IPermissionRepository>();
            mockPermissionRepository.Setup(repo => repo.Insert(It.IsAny<Permission>())).ReturnsAsync(permissionToCreate);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger>();

            var service = new RequestPermissionService(mockPermissionRepository.Object, mockUnitOfWork.Object, null, mockLogger.Object);

            // Act
            var result = await service.RequestPermission(permissionToCreate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(permissionToCreate.EmployeeForename, result.EmployeeForename);
            Assert.AreEqual(permissionToCreate.EmployeeSurname, result.EmployeeSurname);
        }

        [Test]
        public async Task RequestPermission_RollbacksUnitOfWorkOnError()
        {
            // Arrange
            var permissionToCreate = new Permission { EmployeeForename = "John", EmployeeSurname = "Doe" };

            var mockPermissionRepository = new Mock<IPermissionRepository>();
            mockPermissionRepository.Setup(repo => repo.Insert(It.IsAny<Permission>())).ThrowsAsync(new Exception());

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger>();

            var service = new RequestPermissionService(mockPermissionRepository.Object, mockUnitOfWork.Object, null, mockLogger.Object);

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(() => service.RequestPermission(permissionToCreate));

            // Assert
            mockUnitOfWork.Verify(uow => uow.Rollback(), Times.Once);
        }
    }
}
