using AutoMapper;

namespace Currency.Core.Dto.Mappings;

public interface IMapWith
{
    void Mapping(Profile profile);
}

public interface IMapWith<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType()).ReverseMap();
}