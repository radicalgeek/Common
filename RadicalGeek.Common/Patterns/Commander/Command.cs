namespace RadicalGeek.Common.Patterns.Commander
{
    public abstract class Command<T>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "T could be an immutable reference type or a value type. In any case, this instance should be read-only.")]
        private readonly T parameter;
        protected T Parameter { get { return parameter; } }

        protected Command(T parameter)
        {
            this.parameter = parameter;
        }

        public abstract T Execute(T currentValue);
        public abstract T Undo(T currentValue);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        public override string ToString()
        {
            return string.Format("{0}({1})", GetType().Name, Parameter);
        }
    }
}
