
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4042.0), X64 RyuJIT  [AttachedDebugger]
  Job-YRNBIY : .NET Framework 4.8 (4.8.4042.0), X64 RyuJIT

IterationCount=100  LaunchCount=1  WarmupCount=0

             Method |     Mean |     Error |    StdDev |      Min |      Max | Gen 0 | Gen 1 | Gen 2 | Allocated |
------------------- |---------:|----------:|----------:|---------:|---------:|------:|------:|------:|----------:|
 EvalSetLargeString | 2.671 ms | 0.0720 ms | 0.2018 ms | 2.226 ms | 3.233 ms |     - |     - |     - | 105.81 KB |
