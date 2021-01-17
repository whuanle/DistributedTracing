``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-3740QM CPU 2.70GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.200-preview.20601.7
  [Host]        : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
  .NET Core 3.1 : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT

Job=.NET Core 3.1  Runtime=.NET Core 3.1  

```
| Method |      Mean |    Error |   StdDev |
|------- |----------:|---------:|---------:|
|   Get1 |  16.01 ns | 0.321 ns | 0.284 ns |
|   Get2 | 204.63 ns | 3.009 ns | 2.349 ns |
|   Get3 | 182.53 ns | 2.215 ns | 2.072 ns |
