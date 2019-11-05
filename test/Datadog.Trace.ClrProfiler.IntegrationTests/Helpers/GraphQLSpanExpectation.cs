namespace Datadog.Trace.ClrProfiler.IntegrationTests
{
    public class GraphQLSpanExpectation : WebServerSpanExpectation
    {
        public GraphQLSpanExpectation(string serviceName, string operationName)
            : base(serviceName, operationName, SpanTypes.GraphQL)
        {
            RegisterCustomExpectation(nameof(IsGraphQLError), actual: s => string.IsNullOrEmpty(GetTag(s, Tags.ErrorMsg)).ToString(), expected => (!IsGraphQLError).ToString());
            RegisterTagExpectation(nameof(Tags.GraphQLSource), expected: GraphQLSource);
            RegisterTagExpectation(nameof(Tags.GraphQLOperationType), expected: GraphQLOperationType);
        }

        public string GraphQLRequestBody { get; set; }

        public string GraphQLOperationType { get; set; }

        public string GraphQLOperationName { get; set; }

        public string GraphQLSource { get; set; }

        public bool IsGraphQLError { get; set; }
    }
}
