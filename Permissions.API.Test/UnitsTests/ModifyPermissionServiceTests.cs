using NUnit.Framework;
using Moq;
using Permissions.BL.Models;
using Permissions.BL.Repositories;
using Permissions.BL.Repositories.UnitOfWork;
using Permissions.BL.Services.Implements;
using System;
using System.Threading.Tasks;
using Serilog;

namespace Permissions.API.Tests.UnitsTests
{
    [TestFixture]
    public class ModifyPermissionServiceTests
    {
        [Test]
        public async Task ModifyPermission_UpdatesExistingPermission()
        {
            // Arrange
            var existingPermission = new Permission { Id = 1, EmployeeForename = "John", EmployeeSurname = "Doe" };
            var modifiedPermission = new Permission { Id = 1, EmployeeForename = "Jane", EmployeeSurname = "Smith" };

            var mockPermissionRepository = new Mock<IPermissionRepository>();
            mockPermissionRepository.Setup(repo => repo.GetById(1)).ReturnsAsync(existingPermission);
            mockPermissionRepository.Setup(repo => repo.Update(It.IsAny<Permission>())).ReturnsAsync(modifiedPermission);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger>();

            var service = new ModifyPermissionService(mockPermissionRepository.Object, mockUnitOfWork.Object, null, mockLogger.Object);

            // Act
            var result = await service.ModifyPermission(1, modifiedPermission);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(modifiedPermission.EmployeeForename, result.EmployeeForename);
            Assert.AreEqual(modifiedPermission.EmployeeSurname, result.EmployeeSurname);
        }

        [Test]
        public async Task ModifyPermission_ThrowsExceptionWhenPermissionNotFound()
        {
            // Arrange
            var mockPermissionRepository = new Mock<IPermissionRepository>();
            mockPermissionRepository.Setup(repo => repo.GetById(1)).ReturnsAsync((Permission)null);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger>();

            var service = new ModifyPermissionService(mockPermissionRepository.Object, mockUnitOfWork.Object, null, mockLogger.Object);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => service.ModifyPermission(1, new Permission()));
        }

        [Test]
        public async Task ModifyPermission_RollbacksUnitOfWorkOnError()
        {
            // Arrange
            var existingPermission = new Permission { Id = 1, EmployeeForename = "John", EmployeeSurname = "Doe" };
            var modifiedPermission = new Permission { Id = 1, EmployeeForename = "Jane", EmployeeSurname = "Smith" };

            var mockPermissionRepository = new Mock<IPermissionRepository>();
            mockPermissionRepository.Setup(repo => repo.GetById(1)).ReturnsAsync(existingPermission);
            mockPermissionRepository.Setup(repo => repo.Update(It.IsAny<Permission>())).ThrowsAsync(new Exception());

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger>();

            var service = new ModifyPermissionService(mockPermissionRepository.Object, mockUnitOfWork.Object, null, mockLogger.Object);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => service.ModifyPermission(1, modifiedPermission));

            // Assert
            mockUnitOfWork.Verify(uow => uow.Rollback(), Times.Once);
        }
    }
}
