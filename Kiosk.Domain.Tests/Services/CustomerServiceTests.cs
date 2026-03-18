using AutoMapper;
using Kiosk.Data;
using Kiosk.Domain.DTOs;
using Kiosk.Domain.Mapping;
using Kiosk.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kiosk.Domain.Tests.Services;

public class CustomerServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
        return config.CreateMapper();
    }

    private static KioskDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<KioskDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new KioskDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistCustomer()
    {
        using var context = CreateContext(nameof(CreateAsync_ShouldPersistCustomer));
        var service = new CustomerService(context, CreateMapper(), NullLogger<CustomerService>.Instance);

        var dto = new CreateCustomerDto { Name = "Test customer", CIF = "A12345678" };
        var created = await service.CreateAsync(dto);

        Assert.NotNull(created);
        Assert.Equal(dto.Name, created.Name);
        Assert.Equal(dto.CIF, created.CIF);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingCustomer()
    {
        using var context = CreateContext(nameof(UpdateAsync_ShouldUpdateExistingCustomer));
        var service = new CustomerService(context, CreateMapper(), NullLogger<CustomerService>.Instance);

        var createDto = new CreateCustomerDto { Name = "Initial", CIF = "A12345678" };
        var created = await service.CreateAsync(createDto);

        var updateDto = new UpdateCustomerDto { Name = "Updated", CIF = "B87654321" };
        var updated = await service.UpdateAsync(created.Id, updateDto);

        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated!.Id);
        Assert.Equal(updateDto.Name, updated.Name);
        Assert.Equal(updateDto.CIF, updated.CIF);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCustomer()
    {
        using var context = CreateContext(nameof(DeleteAsync_ShouldRemoveCustomer));
        var service = new CustomerService(context, CreateMapper(), NullLogger<CustomerService>.Instance);

        var created = await service.CreateAsync(new CreateCustomerDto { Name = "To Delete", CIF = "A12345678" });
        var deleted = await service.DeleteAsync(created.Id);

        Assert.True(deleted);

        var loaded = await service.GetByIdAsync(created.Id);
        Assert.Null(loaded);
    }

    [Fact]
    public async Task AssignAndUnassignKioskAsync_ShouldUpdateKioskCustomerId()
    {
        using var context = CreateContext(nameof(AssignAndUnassignKioskAsync_ShouldUpdateKioskCustomerId));
        var service = new CustomerService(context, CreateMapper(), NullLogger<CustomerService>.Instance);

        var customer = await service.CreateAsync(new CreateCustomerDto { Name = "Cliente", CIF = "A12345678" });

        // Create kiosk directly in context (it is not part of the service)
        var kiosk = new Kiosk.Entities.Kiosk
        {
            Name = "Kiosk 1",
            SerialNumber = "SN-001",
            AdminURL = "https://localhost",
            AdminPort = 5000
        };

        context.Kiosks.Add(kiosk);
        await context.SaveChangesAsync();

        var assigned = await service.AssignKioskAsync(customer.Id, kiosk.Id);
        Assert.True(assigned);

        var kioskFromDb = await context.Kiosks.FindAsync(kiosk.Id);
        Assert.NotNull(kioskFromDb);
        Assert.Equal(customer.Id, kioskFromDb!.CustomerId);

        var unassigned = await service.UnassignKioskAsync(customer.Id, kiosk.Id);
        Assert.True(unassigned);

        kioskFromDb = await context.Kiosks.FindAsync(kiosk.Id);
        Assert.NotNull(kioskFromDb);
        Assert.Null(kioskFromDb!.CustomerId);
    }
}
