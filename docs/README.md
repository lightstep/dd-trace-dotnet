# ls-trace-dotnet

## ⛔️ Deprecation Warning ⛔️
Lightstep will be EOLing ls-trace tracers in the near future.
* All new users are recommended to use [OpenTelemetry](https://github.com/open-telemetry/opentelemetry-dotnet).
* For those currently using these tracers, we will be reaching out in Q3 2021 to ensure you have a smooth transition to OpenTelemetry. If for any reason you find a gap with OpenTelemetry for your use case, please reach out to your Customer Success representative to discuss and set up time with our Data Onboarding team.

[![CircleCI](https://circleci.com/gh/lightstep/ls-trace-dotnet/tree/master.svg?style=svg)](https://circleci.com/gh/lightstep/ls-trace-dotnet/tree/master)

Datadog has generously announced the [donation](https://www.datadoghq.com/blog/opentelemetry-instrumentation) of their tracer libraries to the [OpenTelemetry](https://opentelemetry.io/) project. Auto-instrumentation is a core feature of these libraries, making it possible to create and collect telemetry data without needing to change your code. LightStep wants you to be able to use these libraries now! `ls-trace-dontnet` is LightStep's fork of Datadog’s tracing client for .NET. You can install and use it to take advantage of auto-instrumentation without waiting for OpenTelemetry. Each LightStep agent is [“pinned” to a Datadog release](#versioning) and is fully supported by LightStep’s Customer Success team.

## Installation and Usage

Please [read our documentation](https://docs.lightstep.com/docs/net-auto-instrumentation) for instructions on setting up .NET tracing and details about supported frameworks.

## Downloads

Package|Download
-|-
Windows and Linux Installers|[See releases](https://github.com/LightStep/ls-trace-dotnet/releases)
`LightStep.Trace`|[![LightStep.Trace](https://img.shields.io/nuget/vpre/LightStep.Trace.svg)](https://www.nuget.org/packages/LightStep.Trace)
`LightStep.Trace.OpenTracing`|[![LightStep.Trace.OpenTracing](https://img.shields.io/nuget/vpre/LightStep.Trace.OpenTracing.svg)](https://www.nuget.org/packages/LightStep.Trace.OpenTracing)

## Versioning

ls-trace-dotnet follows dd-trace-dotnet versions, providing pinned versions of each release:

| ls-trace-dotnet version | dd-trace-dotnet version |
|-------------------------|-------------------------|
| v1.15.0                 | v1.15.0                 |

## Get in touch

Contact `support@lightstep.com` for additional questions and resources, or to be added to our community slack channel.

## Licensing

This is a fork of [dd-trace-dotnet](https://github.com/DataDog/dd-trace-dotnet) and retains the original Datadog license and copyright. See the [license](https://github.com/lightstep/ls-trace-dotnet/blob/master/LICENSE) for more details.
