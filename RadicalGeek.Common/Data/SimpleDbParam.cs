using System.Data;

namespace RadicalGeek.Common.Data
{
    public sealed class SimpleDbParam
    {
        public string ParameterName;
        public object Value;
        private ParameterDirection direction;

        public ParameterDirection Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                if (direction == ParameterDirection.ReturnValue || direction == ParameterDirection.Output)
                {
                    if (!DbType.HasValue)
                        DbType = System.Data.DbType.String;
                    if (!Size.HasValue)
                        Size = -1;
                }
            }
        }

        public int? Size;

        public DbType? DbType;

        public SimpleDbParam(string parameterName, object value)
        {
            ParameterName = parameterName;
            Value = value;
        }

        public SimpleDbParam(string parameterName)
        {
            ParameterName = parameterName;
        }

        public T GetValue<T>()
        {
            return Value.CoerceValue<T>();
        }
    }
}
