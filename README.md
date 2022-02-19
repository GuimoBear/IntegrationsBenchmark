# IntegrationsBenchmark

> Please, before the first docker-compose execution, create the ssl certificate, **[see here](certs/README.md)** how to create

## Benchmark result on .NET 6.0 Runtime

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
Intel Core i7-9750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.200-preview.22055.15
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  Job-LXVAGO : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


```
|                                             Channel |                                     Method |     Mean |    StdDev |     Error |  Gen 0 |  Gen 1 | Allocated |
|---------------------------------------------------- |------------------------------------------- |---------:|----------:|----------:|-------:|-------:|----------:|
|           Grpc with Flatbuffer lazy deserialization | &#39;Send request to duplex streaming channel&#39; | 1.112 ms | 0.0045 ms | 0.0087 ms | 2.5000 | 0.5000 |     17 KB |
|    Grpc with Flatbuffer progressive deserialization | &#39;Send request to duplex streaming channel&#39; | 1.127 ms | 0.0079 ms | 0.0152 ms | 2.5000 | 0.5000 |     18 KB |
| Grpc with Flatbuffer greedy mutable deserialization | &#39;Send request to duplex streaming channel&#39; | 1.130 ms | 0.0026 ms | 0.0039 ms | 2.5000 | 0.5000 |     17 KB |
|         Grpc with Flatbuffer greedy deserialization | &#39;Send request to duplex streaming channel&#39; | 1.139 ms | 0.0095 ms | 0.0143 ms | 2.5000 | 0.5000 |     17 KB |
|                                    Grpc without SSL | &#39;Send request to duplex streaming channel&#39; | 1.452 ms | 0.0841 ms | 0.1413 ms | 1.0000 | 0.5000 |      8 KB |
|                            Grpc with SSL and HTTP 3 | &#39;Send request to duplex streaming channel&#39; | 1.527 ms | 0.0691 ms | 0.1045 ms | 1.0000 | 0.5000 |      8 KB |
|                                       Grpc with SSL | &#39;Send request to duplex streaming channel&#39; | 1.566 ms | 0.1246 ms | 0.1884 ms | 1.0000 | 0.5000 |      8 KB |
|                                    Grpc without SSL |         &#39;Send request and get data stream&#39; | 1.720 ms | 0.0321 ms | 0.0613 ms | 1.5000 | 0.5000 |     12 KB |
|                                    Grpc without SSL |                             &#39;Send request&#39; | 1.737 ms | 0.1180 ms | 0.1784 ms | 1.5000 | 0.5000 |     11 KB |
|                            Grpc with SSL and HTTP 3 |                             &#39;Send request&#39; | 1.748 ms | 0.1125 ms | 0.1702 ms | 1.5000 | 0.5000 |     12 KB |
|           Grpc with Flatbuffer lazy deserialization |                             &#39;Send request&#39; | 1.805 ms | 0.0996 ms | 0.1506 ms | 0.5000 |      - |      5 KB |
|    Grpc with Flatbuffer progressive deserialization |                             &#39;Send request&#39; | 1.823 ms | 0.0719 ms | 0.1087 ms | 1.0000 | 0.5000 |      6 KB |
|         Grpc with Flatbuffer greedy deserialization |                             &#39;Send request&#39; | 1.826 ms | 0.1087 ms | 0.1644 ms | 1.0000 | 0.5000 |      7 KB |
|                            Grpc with SSL and HTTP 3 |         &#39;Send request and get data stream&#39; | 1.860 ms | 0.1014 ms | 0.1532 ms | 2.0000 | 0.5000 |     13 KB |
| Grpc with Flatbuffer greedy mutable deserialization |                             &#39;Send request&#39; | 1.866 ms | 0.0722 ms | 0.1091 ms | 1.0000 | 0.5000 |      7 KB |
|                                       Grpc with SSL |                             &#39;Send request&#39; | 1.897 ms | 0.1082 ms | 0.1636 ms | 1.5000 | 0.5000 |     12 KB |
|         Grpc with Flatbuffer greedy deserialization |         &#39;Send request and get data stream&#39; | 1.927 ms | 0.0681 ms | 0.1029 ms | 2.5000 | 0.5000 |     17 KB |
|    Grpc with Flatbuffer progressive deserialization |         &#39;Send request and get data stream&#39; | 1.929 ms | 0.0792 ms | 0.1197 ms | 2.5000 | 0.5000 |     17 KB |
| Grpc with Flatbuffer greedy mutable deserialization |         &#39;Send request and get data stream&#39; | 1.929 ms | 0.0493 ms | 0.0828 ms | 2.5000 | 0.5000 |     17 KB |
|           Grpc with Flatbuffer lazy deserialization |         &#39;Send request and get data stream&#39; | 1.957 ms | 0.0357 ms | 0.0540 ms | 2.5000 | 0.5000 |     16 KB |
|                                       Grpc with SSL |         &#39;Send request and get data stream&#39; | 2.058 ms | 0.1147 ms | 0.1734 ms | 2.0000 | 0.5000 |     13 KB |
|                    Rest with HTTP 2 and without SSL |                             &#39;Send request&#39; | 3.177 ms | 0.1301 ms | 0.1967 ms | 2.0000 | 0.5000 |     14 KB |
|                            Rest with HTTP 2 and SSL |                             &#39;Send request&#39; | 3.189 ms | 0.0923 ms | 0.1396 ms | 2.5000 | 0.5000 |     15 KB |
|                                    Rest with HTTP 3 |                             &#39;Send request&#39; | 3.216 ms | 0.1430 ms | 0.2163 ms | 2.5000 | 0.5000 |     15 KB |
