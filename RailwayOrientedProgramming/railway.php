<?php declare(strict_types=1);

class Result
{
    /** @var mixed T */
    private $success;
    /** @var string */
    private $failure;
    /** @var bool */
    private $isFailure;

    private function __construct()
    {
    }

    public static function success($success)
    {
        $result = new self();
        $result->success = $success;
        $result->isFailure = false;

        return $result;
    }

    public static function failure(string $failure)
    {
        $result = new self();
        $result->failure = $failure;
        $result->isFailure = true;

        return $result;
    }

    public function getSuccess()
    {
        return $this->success;
    }

    public function getFailure(): string
    {
        return $this->failure;
    }

    public function isFailure(): bool
    {
        return $this->isFailure;
    }
}

class Person
{
    /** @var string */
    private $email;

    public function __construct(string $email)
    {
        $this->email = $email;
    }

    public function getEmail(): string
    {
        return $this->email;
    }
}

class Parser
{
    public static function parse(array $args): Result
    {
        array_shift($args);

        return count($args) === 1
            ? Result::success(reset($args))
            : Result::failure(sprintf('Not valid input - %s given', var_export($args, true)));
    }
}

class Validator
{
    public static function notBlank(string $input): Result
    {
        return !empty($input)
            ? Result::success($input)
            : Result::failure(sprintf('Input "%s" is blank.', $input));
    }

    public static function email(string $value): Result
    {
        return strpos($value, '@') !== false
            ? Result::success($value)
            : Result::failure(sprintf('Value "%s" is not a valid e-mail.', $value));
    }
}

class Repository
{
    public static function savePerson(string $email): Result
    {
        // Result::failure(); // - db error, ...
        return Result::success(new Person($email));
    }
}

class Railway
{
    /** @var Result */
    private $current;

    public static function start($input): self
    {
        return new self(Result::success($input));
    }

    private function __construct(Result $result)
    {
        $this->current = $result;
    }

    /**
     * Bind (T -> Result<T>) to (Result<T> -> Result<U>)
     * input ___ success -> success ___ success
     *        \_ failure    failure _\_ failure
     *
     * @param callable $switchFunction (T) -> Result<T>
     */
    public function bind(callable $switchFunction): self
    {
        $this->current = $this->current->isFailure()
            ? $this->current
            : $switchFunction($this->current->getSuccess());

        return $this;
    }

    /**
     * Map (T -> U) to (Result<T> -> Result<U>)
     * input ___ success -> success ___ success
     *                      failure ___ failure
     *
     * @param callable $singleTrackFunction (T) -> U
     */
    public function map(callable $singleTrackFunction): self
    {
        return $this->bind(function () use ($singleTrackFunction) {
            return Result::success($singleTrackFunction($this->current->getSuccess()));
        });
    }

    public function getResult(): Result
    {
        return $this->current;
    }
}

class Facade
{
    public static function registerPerson(array $args): string
    {
        $savedPerson = Railway::start($args)
            ->bind([Parser::class, 'parse'])
            ->bind([Validator::class, 'notBlank'])
            ->bind([Validator::class, 'email'])
            ->map('strtolower')
            ->bind([Repository::class, 'savePerson'])
            ->getResult();

        if ($savedPerson->isFailure()) {
            return sprintf('Error: %s', $savedPerson->getFailure());
        }

        /** @var Person $person */
        $person = $savedPerson->getSuccess();

        return sprintf('New person registered: %s', $person->getEmail());
    }
}

echo Facade::registerPerson($argv) . PHP_EOL;

// php railway.php -> error: not valid input
// php railway.php foo -> error: is not a valid email
// php railway.php foo@bar -> New person registered: foo@bar
// php railway.php Foo@Bar -> New person registered: foo@bar
