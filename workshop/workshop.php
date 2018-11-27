<?php declare(strict_types=1);

namespace Workshop;

//
// ========= Imutabilita =========
//

// example 1

function example() {
    $data = [1, 2, 3];

    doSomething($data);

    return $data;
}

// example 2

class Person
{
    /** @var string */
    private $name;

    public function __construct(
        string $name
    ) {
        $this->name = $name;
    }

    public function getName(): string
    {
        return $this->name;
    }
}

function example2() {
    $data = new Person('Jon Snow');

    doSomething($data);

    return $data;
}


function doSomething($data) {
    // its ok .. (or.. is it?)
    var_dump($data);
}

function doSomething2(&$data) {
    // broke data!
    $data = null;
}

//
// ========= Rekurze =========
//

function quickSort(array $array): array
{
    if (count($array) <= 1) {
        return $array;
    } else {
        $first = $array[0];
        $smaller = $larger = [];

        for ($i = 1; $i < count($array); $i++) {
            if ($array[$i] >= $first) {
                $larger[] = $array[$i];
            } else {
                $smaller[] = $array[$i];
            }
        }

        return array_merge(quickSort($smaller), [$first], quickSort($larger));
    }
}

//
// ========= Kompozice =========
//

// example

function fillNameTemplate(string $template, string $firstName, string $surname): string
{
    $result = str_replace('{{firstName}}', $firstName, $template);
    $result = str_replace('{{surname}}', $surname, $result);
    $upper = mb_strtoupper($result);

    return $upper;
}


function fillNameTemplate2(string $template, string $firstName, string $surname): string
{
    return mb_strtoupper(str_replace('{{surname}}', $surname, str_replace('{{firstName}}', $firstName, $template)));
}

function fillNameTemplate3(string $template, string $firstName, string $surname): string
{
    return mb_strtoupper(
        str_replace(
            '{{surname}}',
            $surname,
            str_replace('{{firstName}}', $firstName, $template)
        )
    );
}

// run example

$template = "Ahoj {{firstName}} {{surname}}, jak se máš?";

echo fillNameTemplate($template, "Jon", "Snow");
echo fillNameTemplate2($template, "Jon", "Snow");
echo fillNameTemplate3($template, "Jon", "Snow");
