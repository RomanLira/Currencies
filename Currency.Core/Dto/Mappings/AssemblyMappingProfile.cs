using System.Reflection;
using AutoMapper;

namespace Currency.Core.Dto.Mappings;

public class AssemblyMappingProfile : Profile
{
    public AssemblyMappingProfile()
    {
        ApplyMappingsFromAssembly(AppDomain.CurrentDomain.GetAssemblies());
    }

    private void ApplyMappingsFromAssembly(Assembly[] assemblies)
    {
        var types = assemblies.Where(x => !x.IsDynamic).SelectMany(assembly => assembly.GetExportedTypes().Where(t =>
            t.GetInterfaces().Any(i => 
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapWith<>) || i == typeof(IMapWith)) 
            && t.IsClass && !t.IsAbstract).ToList());
        
        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            var methodInfo = type.GetMethod("Mapping")
                             ?? type.GetInterface("IMapFrom`1")?.GetMethod("Mapping");

            methodInfo?.Invoke(instance, new object[] {this});
        }
    }
}