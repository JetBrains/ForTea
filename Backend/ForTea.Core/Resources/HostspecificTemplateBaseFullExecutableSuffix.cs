    public static class ReSharperGeneratedTransformationExecutor
    {
        public static int Main(string[] args)
        {
            RegisterAssemblyLocations__Generated();
            var transformation = new $(PARAMETER_0)();
            transformation.Host.transformation = transformation;
            transformation.Host.SetOutputEncoding(global::System.Text.Encoding.GetEncoding($(PARAMETER_1)), true);
            string destination = args[0];
            string text = transformation.TransformText();
            var encoding = transformation.Host.Encoding;
            string extension = transformation.Host.FileExtension;
            if (extension != null) destination = destination.WithExtension__Generated(extension);
            foreach (CompilerError error in transformation.Errors)
            {
                global::System.Console.Error.WriteLine(error);
            }

            if (transformation.Errors.HasErrors) return 1;
            global::System.IO.File.WriteAllText(destination, text, encoding);
            return 0;
        }
    
        public static string WithExtension__Generated(this string source, string newExtension)
        {
            if (newExtension.StartsWith(".")) newExtension = newExtension.Substring(1);
            int dotIndex = source.LastIndexOf('.');
            if (dotIndex < 0) return source + newExtension;
            return source.Substring(0, dotIndex + 1) + newExtension;
        }
