using System;
using System.Collections.Generic;
using System.Text;

using FrugalCafe;

namespace ICSharpCode.ILSpy
{
    internal static class CSharpLanguage
    {
        static string ToCSharpString(MetadataReader metadata, TypeDefinitionHandle handle, bool fullName, bool omitGenerics)
        {
            StringBuilder builder = new StringBuilder();
            var currentTypeDefHandle = handle;
            var typeDef = metadata.GetTypeDefinition(currentTypeDefHandle);

            while (currentTypeDefHandle != null)
            {
                if (builder.Length > 0)
                    builder.Insert(0, '.');
                typeDef = metadata.GetTypeDefinition(currentTypeDefHandle);
                var part = ReflectionHelper.SplitTypeParameterCountFromReflectionName(metadata.GetString(typeDef.Name), out int typeParamCount);
                var genericParams = typeDef.GetGenericParameters();
                if (!omitGenerics && genericParams.Count > 0)
                {
                    builder.Insert(0, '>');
                    int firstIndex = genericParams.Count - typeParamCount;
                    for (int i = genericParams.Count - 1; i >= genericParams.Count - typeParamCount; i--)
                    {
                        builder.Insert(0, metadata.GetString(metadata.GetGenericParameter(genericParams[i]).Name));
                        builder.Insert(0, i == firstIndex ? '<' : ',');
                    }
                }
                builder.Insert(0, part);
                currentTypeDefHandle = typeDef.GetDeclaringType();
                if (!fullName)
                    break;
            }

            if (fullName && (typeDef.Namespace != null))
            {
                builder.Insert(0, '.');
                builder.Insert(0, metadata.GetString(typeDef.Namespace));
            }

            return builder.ToString();
        }

        static string ToCSharpStringFrugal(MetadataReader metadata, TypeDefinitionHandle handle, bool fullName, bool omitGenerics)
        {
            StringList builder = StringList.Rent();

            try
            {
                var currentTypeDefHandle = handle;
                var typeDef = metadata.GetTypeDefinition(currentTypeDefHandle);

                while (currentTypeDefHandle != null)
                {
                    if (builder.Count > 0)
                        builder.Add(".");

                    typeDef = metadata.GetTypeDefinition(currentTypeDefHandle);
                    var part = ReflectionHelper.SplitTypeParameterCountFromReflectionName(metadata.GetString(typeDef.Name), out int typeParamCount);
                    var genericParams = typeDef.GetGenericParameters();
                    if (!omitGenerics && genericParams.Count > 0)
                    {
                        builder.Add(">");
                        int firstIndex = genericParams.Count - typeParamCount;
                        for (int i = genericParams.Count - 1; i >= genericParams.Count - typeParamCount; i--)
                        {
                            builder.Add(metadata.GetString(metadata.GetGenericParameter(genericParams[i]).Name));
                            builder.Add(i == firstIndex ? "<" : ",");
                        }
                    }
                    builder.Add(part);
                    currentTypeDefHandle = typeDef.GetDeclaringType();
                    if (!fullName)
                        break;
                }

                if (fullName && (typeDef.Namespace != null))
                {
                    builder.Add(".");
                    builder.Add(metadata.GetString(typeDef.Namespace));
                }

                return builder.ReverseConcat();
            }
            finally
            {
                StringList.Return(builder);
            }
        }
    }

    internal static class ReflectionHelper
    {
        public static string SplitTypeParameterCountFromReflectionName(string name, out int count)
        {
            count = 0;

            return name;
        }
    }

    internal class MetadataReader
    {
        public TypeDefinitionHandle GetTypeDefinition(TypeDefinitionHandle handle)
        {
            return null;
        }

        public string GetString(string name)
        {
            return name;
        }

        public Parameter GetGenericParameter(Parameter name)
        {
            return null;
        }
    }

    internal class Parameter
    {
        public string Name = string.Empty;
    }

    internal class  TypeDefinitionHandle
    {
        public string Name = string.Empty;
        public string Namespace = string.Empty;

        public TypeDefinitionHandle GetDeclaringType()
        {
            return null;
        }

        public IList<Parameter> GetGenericParameters()
        {
            return null;
        }

    }
}
