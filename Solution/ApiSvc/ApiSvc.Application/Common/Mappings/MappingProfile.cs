using System;
using System.Linq;
using System.Reflection;
using AutoMapper;

namespace ApiSvc.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(type => type.GetInterfaces()
                    .Any(i =>
                        i.IsGenericType 
                        && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod(nameof(IMapFrom<object>.Mapping)) 
                                 ?? type.GetInterface($"{nameof(IMapFrom<object>)}`1").GetMethod(nameof(IMapFrom<object>.Mapping));

                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}
