using System.ComponentModel.DataAnnotations;
using Kiosk.Domain.DTOs;

namespace Kiosk.Domain.Tests;

public class DtoValidationTests
{
    [Fact]
    public void CreateCustomerDto_ShouldFailWhenRequiredFieldsMissing()
    {
        var dto = new CreateCustomerDto();
        var results = TestHelpers.Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateCustomerDto.Name)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateCustomerDto.CIF)));
    }

    [Fact]
    public void CreateKioskDto_ShouldValidateUrlAndPort()
    {
        var dto = new CreateKioskDto
        {
            Name = "",
            SerialNumber = "",
            AdminURL = "not-a-url",
            AdminPort = 99999
        };

        var results = TestHelpers.Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateKioskDto.Name)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateKioskDto.AdminURL)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateKioskDto.AdminPort)));
    }

    [Fact]
    public void CreateDeviceDto_ShouldRequireMandatoryFields()
    {
        var dto = new CreateDeviceDto();
        var results = TestHelpers.Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateDeviceDto.Name)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateDeviceDto.DeviceType)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateDeviceDto.Brand)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateDeviceDto.Model)));
    }
}
