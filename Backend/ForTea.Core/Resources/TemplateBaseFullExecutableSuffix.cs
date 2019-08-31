    public static class ReSharperGeneratedTransformationExecutor
    {
        public static int Main(string[] args)
        {
            RegisterAssemblyLocations__Generated();
            var transformation = new $(PARAMETER_0)();
            string result = transformation.TransformText();
            var encoding = global::System.Text.Encoding.GetEncoding($(PARAMETER_1));
            foreach (global::System.CodeDom.Compiler.CompilerError error in transformation.Errors)
            {
                global::System.Console.Error.WriteLine(error);
            }

            if (transformation.Errors.HasErrors) return 1;
            global::System.IO.File.WriteAllText(args[0], result, encoding);
            return 0;
        }

