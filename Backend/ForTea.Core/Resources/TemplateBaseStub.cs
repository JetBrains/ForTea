    {
    	/// <summary>
    	/// The string builder that generation-time code is using to assemble generated output
    	/// </summary>
    	protected global::System.Text.StringBuilder GenerationEnvironment { get; set; }

        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public global::System.CodeDom.Compiler.CompilerErrorCollection Errors => null;

        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent => null;

    	/// <summary>
    	/// Current transformation session
    	/// </summary>
    	public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
	        get => null;
    		set { }
    	}

        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
        }

        /// <summary>
    	/// Write text directly into the generated output
    	/// </summary>
    	public void WriteLine(string textToAppend)
    	{
    	}

    	/// <summary>
    	/// Write formatted text directly into the generated output
    	/// </summary>
    	public void Write(string format, params object[] args)
    	{
    	}

    	/// <summary>
    	/// Write formatted text directly into the generated output
    	/// </summary>
    	public void WriteLine(string format, params object[] args)
    	{
    	}

    	/// <summary>
    	/// Raise an error
    	/// </summary>
    	public void Error(string message)
    	{
    	}

    	/// <summary>
    	/// Raise a warning
    	/// </summary>
    	public void Warning(string message)
    	{
    	}

    	/// <summary>
    	/// Increase the indent
    	/// </summary>
    	public void PushIndent(string indent)
    	{
    	}

        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent() => null;

    	/// <summary>
    	/// Remove any indentation
    	/// </summary>
    	public void ClearIndent()
    	{
    	}

        [__ReSharperSynthetic]
        public class ToStringInstanceHelper
        {
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            [__ReSharperSynthetic]
            public System.IFormatProvider FormatProvider => null;

            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            [__ReSharperSynthetic]
            public string ToStringWithCulture(object objectToConvert) => null;
        }

        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        [__ReSharperSynthetic]
        public ToStringInstanceHelper ToStringHelper => null;
    }
