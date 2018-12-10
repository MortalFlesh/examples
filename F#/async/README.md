Async example
=============

Run 10 messages synchronously
-----------------------------

- Render message - 1 after 795ms
- Render message - 2 after 817ms
- Render message - 3 after 682ms
- Render message - 4 after 355ms
- Render message - 5 after 912ms
- Render message - 6 after 743ms
- Render message - 7 after 206ms
- Render message - 8 after 869ms
- Render message - 9 after 404ms
- Render message - 10 after 220ms

### After computation
- message - 1   795ms
- message - 2   817ms
- message - 3   682ms
- message - 4   355ms
- message - 5   912ms
- message - 6   743ms
- message - 7   206ms
- message - 8   869ms
- message - 9   404ms
- message - 10  220ms

`=== Execution lasts 6057.139700 ms ===`

Run 10 messages synchonously in parallel
----------------------------------------

- Render message - 9 after 331ms
- Render message - 8 after 456ms
- Render message - 3 after 534ms
- Render message - 1 after 573ms
- Render message - 2 after 620ms
- Render message - 5 after 621ms
- Render message - 7 after 646ms
- Render message - 6 after 766ms
- Render message - 10 after 924ms
- Render message - 4 after 966ms

### After computation
- message - 1   573ms
- message - 2   620ms
- message - 3   534ms
- message - 4   966ms
- message - 5   621ms
- message - 6   766ms
- message - 7   646ms
- message - 8   456ms
- message - 9   331ms
- message - 10  924ms

`=== Execution lasts 1000.277200 ms ===`

Run 10 messages making them async synchonously in parallel
----------------------------------------------------------

- Render message - 3 after 620ms
- Render message - 4 after 683ms
- Render message - 2 after 751ms
- Render message - 1 after 978ms
- Render message - 8 after 232ms
- Render message - 5 after 699ms
- Render message - 7 after 650ms
- Render message - 6 after 856ms
- Render message - 10 after 778ms
- Render message - 9 after 974ms

### After computation
- message - 1   978ms
- message - 2   751ms
- message - 3   620ms
- message - 4   683ms
- message - 5   699ms
- message - 6   856ms
- message - 7   650ms
- message - 8   232ms
- message - 9   974ms
- message - 10  778ms

`=== Execution lasts 1962.154000 ms ===`
