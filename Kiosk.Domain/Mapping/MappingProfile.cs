using AutoMapper;
using Kiosk.Domain.DTOs;
using KioskEntities = Kiosk.Entities;

namespace Kiosk.Domain.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity to DTO
        CreateMap<KioskEntities.Customer, CustomerDto>()
            .ForMember(dest => dest.CIF, opt => opt.MapFrom(src => src.CIF ?? string.Empty));
            
        CreateMap<KioskEntities.Kiosk, KioskDto>()
            .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(src => src.SerialNumber ?? string.Empty));

        CreateMap<KioskEntities.Device, DeviceDto>()
            .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(src => src.SerialNumber))
            .ForMember(dest => dest.DeviceType, opt => opt.MapFrom(src => src.DeviceType ?? string.Empty))
            .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand ?? string.Empty))
            .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model ?? string.Empty));

        // DTO to Entity
        // Customer
        CreateMap<CustomerDto, KioskEntities.Customer>()
            .ForMember(dest => dest.Kiosks, opt => opt.Ignore());

        CreateMap<CreateCustomerDto, KioskEntities.Customer>();

        CreateMap<UpdateCustomerDto, KioskEntities.Customer>();

        // Kiosk
        CreateMap<KioskDto, KioskEntities.Kiosk>()
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Devices, opt => opt.Ignore());

        CreateMap<CreateKioskDto, KioskEntities.Kiosk>();

        CreateMap<UpdateKioskDto, KioskEntities.Kiosk>();

        // Device
        CreateMap<DeviceDto, KioskEntities.Device>()
            .ForMember(dest => dest.Kiosk, opt => opt.Ignore());

        CreateMap<CreateDeviceDto, KioskEntities.Device>();

        CreateMap<UpdateDeviceDto, KioskEntities.Device>();
    }
}