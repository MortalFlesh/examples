<?php declare(strict_types=1);

class Person
{
    /** @var int */
    private $id;
    /** @var string */
    private $name;

    public function __construct(int $id, string $name)
    {
        $this->id = $id;
        $this->name = $name;
    }

    public function getId(): int
    {
        return $this->id;
    }

    public function getName(): string
    {
        return $this->name;
    }
}

interface Functor   /* <F> */
{
    /**
     * @param callable $f (<F>) -> mixed <T>
     * @return mixed <T>
     */
    public function map(callable $f);
}

interface Monad     /* <M> */
{
    /**
     * @param callable $f (M) -> M
     * @return Monad
     */
    public function bind(callable $f);
}

class PersonWrapper implements Functor, Monad
{
    /** @var Person|null */
    private $person;

    public static function loadFromDb(int $id): self
    {
        return new self(
            $id === 42
                ? new Person(42, 'Jon Snow')
                : null
        );
    }

    private function __construct(?Person $person)
    {
        $this->person = $person;
    }

    /**
     * @param callable $f (Person) -> mixed <T>
     * @return mixed <T>
     */
    public function map(callable $f)
    {
        return $this->person === null
            ? null
            : $f($this->person);
    }

    /**
     * @param callable $f (Person) -> Person
     */
    public function bind(callable $f): PersonWrapper
    {
        return $this->person === null
            ? $this
            : new self($f($this->person));
    }
}

$jonSnow = PersonWrapper::loadFromDb(42);
$unknown = PersonWrapper::loadFromDb(0);

$getName = function (Person $person): string {
    return $person->getName();
};

$renameTo = function (string $newName) {
    return function (Person $person) use ($newName) {
        return new Person($person->getId(), $newName);
    };
};

var_dump($jonSnow->map($getName));
var_dump($unknown->map($getName));

echo "\nRename ...\n";
var_dump($jonSnow->bind($renameTo('Peter Parker'))->map($getName));
var_dump($unknown->bind($renameTo('Peter Parker'))->map($getName));
