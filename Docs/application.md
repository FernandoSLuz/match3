# Application Layer

## CommandBus
- Simple FIFO queue of `ICommand`.

## TurnManager
- Consumes commands, validates swaps, performs resolve, emits events per cascade.
- Command: `SwapTilesCommand { From, To }`.

## EventBus
- Publish/subscribe by event type, decouples systems and presentation.
