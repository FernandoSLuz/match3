# Application Layer

## CommandBus
- Simple FIFO queue of `ICommand`.

## TurnManager
- **Coroutine-based** turn execution.
- Validates swaps, processes cascades **one at a time**, waits for animations to complete between steps.
- Command: `SwapTilesCommand { From, To }`.
- Uses `ProcessSwap(command, waitCallback)` to yield between animation steps.

## EventBus
- Publish/subscribe by event type, decouples systems and presentation.
- Key pattern: `AnimationCompleteEvent` signals presentation is ready for next domain step.
