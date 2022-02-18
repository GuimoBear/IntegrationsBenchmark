# IntegrationsBenchmark

> Please, before the first docker-compose execution, create the ssl certificate, **[see here](certs/README.md)** how to create

## Benchmark result on .NET 6.0 Runtime

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.200-preview.22055.15
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  Job-ZUVCIS : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


```
|                          Channel |                                     Method |     Mean |    StdDev |     Error |  Gen 0 |  Gen 1 | Allocated |
|--------------------------------- |------------------------------------------- |---------:|----------:|----------:|-------:|-------:|----------:|
|                 Grpc without SSL | &#39;Send request to duplex streaming channel&#39; | 1.295 ms | 0.0570 ms | 0.0862 ms | 1.0000 |      - |      8 KB |
|                    Grpc with SSL | &#39;Send request to duplex streaming channel&#39; | 1.324 ms | 0.0357 ms | 0.0540 ms | 1.0000 | 0.5000 |      8 KB |
|         Grpc with SSL and HTTP 3 | &#39;Send request to duplex streaming channel&#39; | 1.357 ms | 0.0340 ms | 0.0571 ms | 1.0000 | 0.5000 |      8 KB |
|                    Grpc with SSL |                             &#39;Send request&#39; | 1.652 ms | 0.0945 ms | 0.1428 ms | 1.5000 | 0.5000 |     12 KB |
|         Grpc with SSL and HTTP 3 |                             &#39;Send request&#39; | 1.676 ms | 0.0695 ms | 0.1051 ms | 1.5000 | 0.5000 |     12 KB |
|                 Grpc without SSL |                             &#39;Send request&#39; | 1.692 ms | 0.1036 ms | 0.1566 ms | 1.5000 | 0.5000 |     11 KB |
|                 Grpc without SSL |         &#39;Send request and get data stream&#39; | 1.701 ms | 0.0973 ms | 0.1471 ms | 1.5000 | 0.5000 |     12 KB |
|                    Grpc with SSL |         &#39;Send request and get data stream&#39; | 1.763 ms | 0.0937 ms | 0.1416 ms | 2.0000 | 0.5000 |     13 KB |
|         Grpc with SSL and HTTP 3 |         &#39;Send request and get data stream&#39; | 1.764 ms | 0.0847 ms | 0.1281 ms | 2.0000 | 0.5000 |     13 KB |
|         Rest with HTTP 2 and SSL |                             &#39;Send request&#39; | 3.062 ms | 0.1051 ms | 0.1589 ms | 2.5000 | 0.5000 |     15 KB |
| Rest with HTTP 2 and without SSL |                             &#39;Send request&#39; | 3.065 ms | 0.1075 ms | 0.1626 ms | 2.0000 | 0.5000 |     14 KB |
|                 Rest with HTTP 3 |                             &#39;Send request&#39; | 3.074 ms | 0.0978 ms | 0.1479 ms | 2.5000 | 0.5000 |     15 KB |
