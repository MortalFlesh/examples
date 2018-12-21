<?php declare(strict_types=1);

require_once __DIR__ . '/vendor/autoload.php';

$sw = new \Symfony\Component\Stopwatch\Stopwatch(true);
$sw->start('test');

[, $total, $performanceType] = $argv;

function printfn($format, ...$args)
{
    printf($format . "\n", ...$args);
}

printfn('For %s - %d', $performanceType, $total);

class Item
{
    public $label;
    public $number;

    public function __construct($label, $number)
    {
        $this->label = $label;
        $this->number = (int) $number;
    }
}

function transform($n)
{
    return $n % 7 === 0
        ? sprintf('Item [%d]', $n)
        : sprintf('Item [%d] | %d', $n, $n);
}

function parse($i)
{
    $parsed = explode('|', $i);

    return count($parsed) === 2
        ? new Item(...$parsed)
        : null;
}

function fromLength($n)
{
    return function ($string) use ($n) {
        return mb_strlen($string) > $n;
    };
}

// array
if ($performanceType === 'array') {
    $result = array_reduce(
        array_filter(
            array_map(
                'parse',
                array_filter(
                    array_map(
                        'transform',
                        range(0, $total)
                    ),
                    fromLength(11)
                )
            ),
            function ($input) {
                return $input !== null;
            }
        ),
        function ($t, Item $c) use ($total) {
            return ($t + $c->number) / $total;
        },
        0
    );

    printfn('Result is %d', $result);
}

// list
if ($performanceType === 'list') {
    $result = \MF\Collection\Mutable\ListCollection::create(range(1, $total), 'transform')
        ->filter(fromLength(11))
        ->map('parse')
        ->filter(function ($i) {
            return $i !== null;
        })
        ->reduce(function ($t, Item $c) use ($total) {
            return ($t + $c->number) / $total;
        }, 0);

    printfn('Result is %d', $result);
}

// list - immutable
if ($performanceType === 'list-immutable') {
    $result = \MF\Collection\Immutable\ListCollection::create(range(1, $total), 'transform')
        ->filter(fromLength(11))
        ->map('parse')
        ->filter(function ($i) {
            return $i !== null;
        })
        ->reduce(function ($t, Item $c) use ($total) {
            return ($t + $c->number) / $total;
        }, 0);

    printfn('Result is %d', $result);
}

// seq
if ($performanceType === 'seq') {
    $result = \MF\Collection\Immutable\Seq::range('0..' . $total)
        ->map('transform')
        ->filter(fromLength(11))
        ->map('parse')
        ->filter(function ($i) {
            return $i !== null;
        })
        ->reduce(function ($t, Item $c) use ($total) {
            return ($t + $c->number) / $total;
        }, 0);

    printfn('Result is %d', $result);
}

// for
if ($performanceType === 'for') {
    $result = 0;
    for ($i = 0; $i < $total; $i++) {
        $t = transform($i);
        if (fromLength(11)($t)) {
            $p = parse($t);
            if ($p !== null) {
                /** @var Item $p */
                $n = $p->number;
                $result = ($result + $n) / $total;
            }
        }
    }

    printfn('Result is %d', $result);
}

// doctrine
if ($performanceType === 'doctrine') {
    $array = (new \Doctrine\Common\Collections\ArrayCollection(range(0, $total)))
        ->map(function ($i) {
            return transform($i);
        })
        ->filter(fromLength(11))
        ->map(function ($i) {
            return parse($i);
        })
        ->filter(function ($i) {
            return $i !== null;
        })
        ->toArray();
    $result = array_reduce(
        $array,
        function ($t, Item $c) use ($total) {
            return ($t + $c->number) / $total;
        },
        0
    );

    printfn('Result is %d', $result);
}

$result = $sw->stop('test');
printfn('Lasts: %d ms | Took: %d MB', $result->getDuration(), $result->getMemory() / 1024 / 1024);
