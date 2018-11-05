<?php declare(strict_types=1);

abstract class PaymentMethod
{
}

class Cash extends PaymentMethod
{
}

class Cheque extends PaymentMethod
{
    /** @var int */
    public $number;

    public function __construct(int $number)
    {
        $this->number = $number;
    }
}

class Card extends PaymentMethod
{
    /** @var string */
    public $cardType;
    /** @var int */
    public $cardNumber;

    public function __construct(string $cardType, int $cardNumber)
    {
        $this->cardType = $cardType;
        $this->cardNumber = $cardNumber;
    }
}

class PatternMatching
{
    private $input;
    /** @var array <Condition, Case> */
    private $matching;

    /**
     * type Input = 'T
     */
    public function match($input): self
    {
        $this->input = $input;

        return $this;
    }

    /**
     * type Pattern =
     *      | callable of Input -> bool
     *      | className of string
     *
     * type Condition =
     *      | None
     *      | callable of Input -> bool
     *
     * type Case = callable of Input -> Result
     */
    public function case($pattern, callable $case): self
    {
        return $this->addMatching($pattern, null, $case);
    }

    public function caseAny(callable $case): self
    {
        return $this->addMatching(null, null, $case);
    }

    public function caseWhen($pattern, callable $condition, callable $case): self
    {
        return $this->addMatching($pattern, $condition, $case);
    }

    private function addMatching($pattern, $condition, $case): self
    {
        // todo
        // - assertion - only 1 pattern = null
        // - how to find out some case is missing?
        // - how to find out `caseAny` is needed

        $this->matching[] = [$pattern, $condition, $case];

        return $this;
    }

    /**
     * type Result = 'U
     */
    public function getResult()
    {
        foreach ($this->matching as [$pattern, $when, $case]) {
            if ($this->isMatchingPattern($pattern) && $this->isSatisfyWhen($when)) {
                return $case($this->input);
            }
        }

        throw new \LogicException('Not all cases was covered!');
    }

    private function isMatchingPattern($pattern): bool
    {
        if ($pattern === null) {
            return true;
        }

        if (is_callable($pattern) && $pattern($this->input)) {
            return true;
        }

        if (class_exists($pattern) && $this->input instanceof $pattern) {
            return true;
        }

        return false;
    }

    private function isSatisfyWhen($when): bool
    {
        return $when === null
            ? true
            : $when($this->input);
    }
}

$printValue1 = function (PaymentMethod $payment): string {
    return (new PatternMatching())
        ->match($payment)
        ->case(Cash::class, function (Cash $cash) {
            return 'Paid in cash';
        })
        ->case(Cheque::class, function (Cheque $cheque) {
            return sprintf('Paid by cheque: %d', $cheque->number);
        })
        ->caseWhen(
            Card::class,
            function (Card $card) {
                return $card->cardType === 'mc';
            },
            function (Card $card) {
                return sprintf('Paid with MasterCard %d', $card->cardNumber);
            }
        )
        ->case(Card::class, function (Card $card) {
            return sprintf('Paid with %s %d', $card->cardType, $card->cardNumber);
        })
        ->caseAny(function (PaymentMethod $unknown) {
            return sprintf('Paid with unknown method');
        })
        ->getResult();
};

$printValue2 = function (PaymentMethod $payment) {
    if ($payment instanceof Cash) {
        return 'Paid in cash';
    } elseif ($payment instanceof Cheque) {
        return sprintf('Paid by cheque: %d', $payment->number);
    } elseif ($payment instanceof Card && $payment->cardType === 'mc') {
        return sprintf('Paid with MasterCard %d', $payment->cardNumber);
    } elseif ($payment instanceof Card) {
        return sprintf('Paid with %s %d', $payment->cardType, $payment->cardNumber);
    } else {
        return sprintf('Paid with unknown method');
    }
};

$print = function (string $message) {
    echo sprintf('%s%s', $message, PHP_EOL);
};

$printValue = $printValue2;

$print($printValue(new Card('visa', 1234)));
$print($printValue(new Card('mc', 2345)));
$print($printValue(new Card('mastercard', 3456)));
$print($printValue(new Cash()));
$print($printValue(new Cheque(42)));
$print($printValue(new class extends PaymentMethod{}));
