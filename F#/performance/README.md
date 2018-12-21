Performance
===========

Iterating mapping, filtering and reducing from 1k to 10m items

# Run compiled

## List
    1000        - 101   ms
    10000       - 189   ms
    100000      - 273   ms
    1000000     - 1978  ms
    10000000    - 21389 ms

## Seq
    1000        - 117   ms
    10000       - 114   ms
    100000      - 167   ms
    1000000     - 728   ms
    10000000    - 7388  ms

## For
    1000        - 109   ms
    10000       - 108   ms
    100000      - 157   ms
    1000000     - 651   ms
    10000000    - 5613  ms

# Run interactively

## List
    1000        - Real: 00:00:00.025, CPU: 00:00:00.025, GC gen0: 0,    gen1: 0
    10000       - Real: 00:00:00.048, CPU: 00:00:00.048, GC gen0: 2,    gen1: 0
    100000      - Real: 00:00:00.251, CPU: 00:00:00.270, GC gen0: 22,   gen1: 1
    1000000     - Real: 00:00:01.904, CPU: 00:00:02.207, GC gen0: 236,  gen1: 6
    10000000    - Real: 00:00:22.294, CPU: 00:00:25.067, GC gen0: 2425, gen1: 13

## Seq
    1000        - Real: 00:00:00.025, CPU: 00:00:00.025, GC gen0: 0,    gen1: 0
    10000       - Real: 00:00:00.049, CPU: 00:00:00.047, GC gen0: 2,    gen1: 0
    100000      - Real: 00:00:00.190, CPU: 00:00:00.188, GC gen0: 19,   gen1: 0
    1000000     - Real: 00:00:01.456, CPU: 00:00:01.440, GC gen0: 204,  gen1: 0
    10000000    - Real: 00:00:15.187, CPU: 00:00:14.943, GC gen0: 2066, gen1: 0

## For
    1000        - Real: 00:00:00.021, CPU: 00:00:00.022, GC gen0: 0,    gen1: 0
    10000       - Real: 00:00:00.038, CPU: 00:00:00.038, GC gen0: 2,    gen1: 0
    100000      - Real: 00:00:00.171, CPU: 00:00:00.165, GC gen0: 19,   gen1: 0
    1000000     - Real: 00:00:01.333, CPU: 00:00:01.320, GC gen0: 203,  gen1: 0
    10000000    - Real: 00:00:14.089, CPU: 00:00:13.672, GC gen0: 2065, gen1: 0
