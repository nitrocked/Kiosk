using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Kiosk.Data;
using Kiosk.Domain.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Kiosk.Domain.Tests;

public static class TestHelpers
{
    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
        return config.CreateMapper();
    }

    public static KioskDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<KioskDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new KioskDbContext(options);
    }

    public static IReadOnlyCollection<ValidationResult> Validate(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
