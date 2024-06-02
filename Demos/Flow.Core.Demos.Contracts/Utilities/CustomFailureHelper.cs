using Flow.Core.Areas.Returns;
using Flow.Core.Demos.Contracts.Common.CustomFailures;
using ProtoBuf.Meta;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;


namespace Flow.Core.Demos.Contracts.Utilities
{
    public static class CustomFailureHelper
    {
        public static void AddCustomFailuresToGrpcRuntimeModel()

            => RuntimeTypeModel.Default.Add(typeof(Failure), true).AddSubType(200, typeof(NotApprovedFailure));

        public static CustomFailureTypeResolver GetJsonCustomFailureTypeResolver()

            => new CustomFailureTypeResolver();

        public static void AddCustomFailuresToJsonResolver(JsonTypeInfo jsonTypeInfo)
        {
            /*
                 * Flow's FailureValue property is the base type Failure which is the polymorphic base
             */ 
            if (jsonTypeInfo.Type == typeof(Failure))
            {
                if (jsonTypeInfo.PolymorphismOptions is not null)
                {
                    var derivedTypes = jsonTypeInfo.PolymorphismOptions.DerivedTypes;
                    var maxEntryID = derivedTypes.Max(d => int.Parse(d.TypeDiscriminator!.ToString()!));

                    if (maxEntryID < 200)//Then our custom failures have not been added
                    {
                        /*
                            * Add all your custom failures here starting with numbers 200 or greater 
                        */
                        derivedTypes.Add(new JsonDerivedType(typeof(NotApprovedFailure), 200));
                    }
                }
            }
        }
    }

    public class CustomFailureTypeResolver : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);
            /*
                 * Flow's FailureValue property is the base type Failure which is the polymorphic base
             */
            if (jsonTypeInfo.Type == typeof(Failure))
            {
                if (jsonTypeInfo.PolymorphismOptions is not null)
                {
                    var derivedTypes = jsonTypeInfo.PolymorphismOptions.DerivedTypes;
                    var maxEntryID   = derivedTypes.Max(d => int.Parse(d.TypeDiscriminator!.ToString()!));

                    if (maxEntryID < 200)//Then our custom failures have not been added
                    {
                        /*
                            * Add all your custom failures here starting with numbers 200 or greater 
                        */ 
                        derivedTypes.Add(new JsonDerivedType(typeof(NotApprovedFailure), 200));
                    }
                }
            }
            return jsonTypeInfo;
        }
    }
}
