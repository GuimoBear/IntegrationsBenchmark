# IntegrationsBenchmark

> Please, before the first docker-compose execution, create the ssl certificate, **[see here](certs/README.md)** how to create

## Benchmark result on .NET 6.0 Runtime

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.200-preview.22055.15
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  Job-BMOYAW : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


```
|          Channel |                                     Method |     Mean |    StdDev |     Error |  Gen 0 |  Gen 1 | Allocated |
|----------------- |------------------------------------------- |---------:|----------:|----------:|-------:|-------:|----------:|
|             Grpc | &#39;Send request to duplex streaming channel&#39; | 1.391 ms | 0.0549 ms | 0.0923 ms | 1.0000 | 0.5000 |      8 KB |
|             Grpc |                             &#39;Send request&#39; | 1.710 ms | 0.0740 ms | 0.1118 ms | 1.5000 | 0.5000 |     12 KB |
|             Grpc |         &#39;Send request and get data stream&#39; | 1.767 ms | 0.0583 ms | 0.0980 ms | 2.0000 | 0.5000 |     13 KB |
| Rest with HTTP 3 |                             &#39;Send request&#39; | 3.114 ms | 0.1284 ms | 0.1942 ms | 2.5000 | 0.5000 |     15 KB |
| Rest with HTTP 2 |                             &#39;Send request&#39; | 3.129 ms | 0.1017 ms | 0.1537 ms | 2.5000 | 0.5000 |     15 KB |
