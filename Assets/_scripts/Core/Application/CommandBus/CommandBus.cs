using System.Collections.Generic;

namespace MatchTree.Core.Application.CommandBus
{
    public class CommandBus
    {
        private readonly Queue<ICommand> queue = new Queue<ICommand>();

        public void Enqueue(ICommand command)
        {
            queue.Enqueue(command);
        }

        public bool TryDequeue(out ICommand command)
        {
            if (queue.Count > 0)
            {
                command = queue.Dequeue();
                return true;
            }
            command = null;
            return false;
        }
    }
}


