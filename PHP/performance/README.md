Performance
===========

Iterating and mapping 1k and then 100m items

# Run compiled

## Array
    1000        - 1     ms  | 2     MB 
    10000       - 10    ms  | 6     MB 
    100000      - 127   ms  | 44    MB 
    1000000     - 1320  ms  | 414   MB
    10000000    - 15139 ms  | 4174  MB

## Mutable\List
    1000        - 7      ms | 2     MB 
    10000       - 17     ms | 6     MB
    100000      - 180    ms | 44    MB
    1000000     - 3883   ms | 414   MB
    10000000    - 261735 ms | 4174  MB

## Immutable\List
    1000        - 12     ms | 2     MB 
    10000       - 398    ms | 6     MB
    100000      - 163593 ms | 44    MB
    1000000     - XXX    ms | XXX   MB
    10000000    - XXX    ms | XXX   MB

## Seq
    1000        - 15     ms | 2     MB
    10000       - 21     ms | 2     MB
    100000      - 171    ms | 2     MB
    1000000     - 1765   ms | 2     MB
    10000000    - 17311  ms | 2     MB

## For
    1000        - 1      ms | 2     MB
    10000       - 8      ms | 2     MB
    100000      - 78     ms | 2     MB
    1000000     - 831    ms | 2     MB
    10000000    - 8344   ms | 2     MB

## Doctrine\Collections
    1000        - 1      ms | 2     MB
    10000       - 13     ms | 6     MB
    100000      - 136    ms | 48    MB
    1000000     - 1564   ms | 446   MB
    10000000    - 16036  ms | 4686  MB
