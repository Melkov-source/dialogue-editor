using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sandbox.Dialogue
{
    public class DialogParameterMetaFinder
    {
        public static IEnumerable<DialogParameterMetaAttribute> FindAllDialogParameterMeta(DialogParameterType parameterType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var typeAttribute = typeof(DialogParameterMetaAttribute);

            foreach (var assembly in assemblies)
            {
                Type[] types;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray();
                }
                catch
                {
                    continue;
                }

                foreach (var type in types)
                {
                    if (type.IsClass == false || type.IsAbstract || type.IsInterface)
                    {
                        continue;
                    }

                    if (type.GetCustomAttributes(typeAttribute, false).FirstOrDefault() is not DialogParameterMetaAttribute meta)
                    {
                        continue;
                    }
                    
                    if (meta.Type == parameterType)
                    {
                        yield return meta;
                    }
                }
            }
        }
    }
}