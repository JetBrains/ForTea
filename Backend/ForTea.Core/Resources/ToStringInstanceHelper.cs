    /// <summary>
    /// Utility class to produce culture-oriented representation of an object as a string.
    /// </summary>
    public static class ToStringInstanceHelper
    {
        private static global::System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
        /// <summary>
        /// Gets or sets format provider to be used by ToStringWithCulture method.
        /// </summary>
        public static global::System.IFormatProvider FormatProvider
        {
            get
            {
                return formatProviderField;
            }
            set
            {
                if (value == null) return;
                formatProviderField  = value;
            }
        }
        /// <summary>
        /// This is called from the compile/run appdomain to convert objects within an expression block to a string
        /// </summary>
        public static string ToStringWithCulture(object objectToConvert)
        {
            if ((objectToConvert == null))
            {
                throw new global::System.ArgumentNullException("objectToConvert");
            }

            global::System.Type t = objectToConvert.GetType();
            global::System.Reflection.MethodInfo method = t.GetMethod("ToString", new global::System.Type[] {
                        typeof(global::System.IFormatProvider)});
            if ((method == null))
            {
                return objectToConvert.ToString();
            }
            else
            {
                return ((string)(method.Invoke(objectToConvert, new object[] { formatProviderField })));
            }
        }
    }
