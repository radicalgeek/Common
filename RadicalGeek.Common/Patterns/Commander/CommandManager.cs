using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RadicalGeek.Common.Patterns.Commander
{
    public class CommandManager<T>
    {
        private readonly T initialValue;
        private readonly List<Command<T>> commands = new List<Command<T>>();

        public CommandManager(T initialValue)
        {
            this.initialValue = initialValue;
            CurrentValue = initialValue;
        }

        public T CurrentValue { get; private set; }

        public T InitialValue
        {
            get
            {
                return initialValue;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public ReadOnlyCollection<Command<T>> Commands
        {
            get
            {
                return new ReadOnlyCollection<Command<T>>(commands);
            }
        }

        public void Add(Command<T> command)
        {
            if (command == null) throw new ArgumentNullException("command");
            commands.Add(command);
            CurrentValue = command.Execute(CurrentValue);
        }

        public void Undo()
        {
            if (commands.Any())
            {
                Command<T> lastCommand = commands.Last();
                CurrentValue = lastCommand.Undo(CurrentValue);
                commands.Remove(lastCommand);
            }
        }

        public void Replay()
        {
            CurrentValue = initialValue;
            foreach (var command in commands)
                CurrentValue = command.Execute(CurrentValue);
        }

        public T ReplayTo(int commandIndex)
        {
            T result = initialValue;
            for (int i = 0; i < commandIndex; i++)
                result = commands[i].Execute(result);
            return result;
        }
    }
}
