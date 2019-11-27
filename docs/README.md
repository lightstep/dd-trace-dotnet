# ls-trace-dotnet

[![CircleCI](https://circleci.com/gh/lightstep/ls-trace-dotnet/tree/master.svg?style=svg)](https://circleci.com/gh/lightstep/ls-trace-dotnet/tree/master)

Datadog has generously announced the [donation](https://www.datadoghq.com/blog/opentelemetry-instrumentation) of their tracer libraries to the [OpenTelemety](https://opentelemetry.io/), project. Auto-instrumentation is a core feature of these libraries, making it possible to create and collect telemetry data without needing to change your code. LightStep wants you to be able to use these libraries now! `ls-trace-dontnet` is LightStep's fork of Datadog’s tracing client for .NET. You can install and use it to take advantage of auto-instrumentation without waiting for OpenTelemetry. Each LightStep agent is [“pinned” to a Datadog release](#versioning) and is fully supported by LightStep’s Customer Success team.

## Installation and Usage

Please read to our [documentation](https://docs.lightstep.com/docs/net-auto-instrumentation) for instructions on setting up .NET tracing and details about supported frameworks.

## Downloads

Package|Download
-|-
Windows and Linux Installers|[See releases](https://github.com/LightStep/ls-trace-dotnet/releases)
`LightStep.Trace`|[![Datadog.Trace](https://img.shields.io/nuget/vpre/LightStep.Trace.svg)](https://www.nuget.org/packages/LightStep.Trace)
`LightStep.Trace.ClrProfiler.Managed`|[![LightStep.Trace.ClrProfiler.Managed](https://img.shields.io/nuget/vpre/LightStep.Trace.ClrProfiler.Managed.svg)](https://www.nuget.org/packages/LightStep.Trace.ClrProfiler.Managed)

## Versioning

ls-trace-dotnet follows dd-trace-dotnet versions, providing pinned versions of each release:

| ls-trace-dotnet version | dd-trace-dotnet version |
|-------------------------|-------------------------|
| v1.19.0                 | v1.19.0                 |

## Get in touch

Contact `support@lightstep.com` for additional questions and resources, or to be added to our community slack channel.

## Licensing

This is a fork of [dd-trace-dotnet](https://github.com/DataDog/dd-trace-dotnet) and retains the original Datadog license and copyright. See the [license](https://github.com/lightstep/ls-trace-dotnet/blob/master/LICENSE) for more details.
