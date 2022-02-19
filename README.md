# IntegrationsBenchmark

> Please, before the first docker-compose execution, create the ssl certificate, **[see here](certs/README.md)** how to create

## Benchmark result on .NET 6.0 Runtime

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.200-preview.22055.15
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  Job-DSGVMT : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


```
|                          Channel |  Runtime |                                     Method |     Mean |    StdDev |     Error |  Gen 0 |  Gen 1 | Allocated |
|--------------------------------- |--------- |------------------------------------------- |---------:|----------:|----------:|-------:|-------:|----------:|
|             Grpc with Flatbuffer | .NET 6.0 | &#39;Send request to duplex streaming channel&#39; | 1.064 ms | 0.0279 ms | 0.0422 ms | 2.5000 | 0.5000 |     18 KB |
|                 Grpc without SSL | .NET 6.0 | &#39;Send request to duplex streaming channel&#39; | 1.301 ms | 0.0321 ms | 0.0539 ms | 1.0000 | 0.5000 |      8 KB |
|                    Grpc with SSL | .NET 6.0 | &#39;Send request to duplex streaming channel&#39; | 1.555 ms | 0.0972 ms | 0.1469 ms | 1.0000 | 0.5000 |      8 KB |
|         Grpc with SSL and HTTP 3 | .NET 6.0 | &#39;Send request to duplex streaming channel&#39; | 1.599 ms | 0.0528 ms | 0.0798 ms | 1.0000 | 0.5000 |      8 KB |
|                    Grpc with SSL | .NET 6.0 |                             &#39;Send request&#39; | 1.755 ms | 0.0752 ms | 0.1137 ms | 1.5000 | 0.5000 |     12 KB |
|                 Grpc without SSL | .NET 6.0 |                             &#39;Send request&#39; | 1.757 ms | 0.0713 ms | 0.1077 ms | 1.5000 | 0.5000 |     11 KB |
|         Grpc with SSL and HTTP 3 | .NET 6.0 |                             &#39;Send request&#39; | 1.759 ms | 0.0802 ms | 0.1213 ms | 1.5000 | 0.5000 |     12 KB |
|             Grpc with Flatbuffer | .NET 6.0 |                             &#39;Send request&#39; | 1.824 ms | 0.1191 ms | 0.1801 ms | 1.0000 | 0.5000 |      6 KB |
|                 Grpc without SSL | .NET 6.0 |         &#39;Send request and get data stream&#39; | 1.824 ms | 0.1637 ms | 0.2474 ms | 1.5000 | 0.5000 |     12 KB |
|         Grpc with SSL and HTTP 3 | .NET 6.0 |         &#39;Send request and get data stream&#39; | 1.829 ms | 0.0825 ms | 0.1387 ms | 2.0000 | 0.5000 |     13 KB |
|                    Grpc with SSL | .NET 6.0 |         &#39;Send request and get data stream&#39; | 1.831 ms | 0.0672 ms | 0.1016 ms | 2.0000 | 0.5000 |     13 KB |
|             Grpc with Flatbuffer | .NET 6.0 |         &#39;Send request and get data stream&#39; | 1.936 ms | 0.0535 ms | 0.0809 ms | 2.5000 | 0.5000 |     17 KB |
| Rest with HTTP 2 and without SSL | .NET 6.0 |                             &#39;Send request&#39; | 3.162 ms | 0.1189 ms | 0.1798 ms | 2.0000 | 0.5000 |     14 KB |
|                 Rest with HTTP 3 | .NET 6.0 |                             &#39;Send request&#39; | 3.197 ms | 0.1195 ms | 0.1806 ms | 2.5000 | 0.5000 |     15 KB |
|         Rest with HTTP 2 and SSL | .NET 6.0 |                             &#39;Send request&#39; | 3.220 ms | 0.1619 ms | 0.2720 ms | 2.5000 | 0.5000 |     15 KB |
