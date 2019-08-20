    public static class ReSharperGeneratedTransformationExecutor
    {
        public static int Main(string[] args)
        {
            RegisterAssemblyLocations__Generated();
            var transformation = new $(PARAMETER_0)();
            transformation.Host.transformation = transformation;
            transformation.Host.SetOutputEncoding(Encoding.GetEncoding($(PARAMETER_1)), true);
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
            File.WriteAllText(destination, text, encoding);
            return 0;
        }
    
        public static string WithExtension__Generated(this string source, string newExtension)
        {
            if (newExtension.StartsWith(".")) newExtension = newExtension.Substring(1);
            int dotIndex = source.LastIndexOf('.');
            if (dotIndex < 0) return source + newExtension;
            return source.Substring(0, dotIndex + 1) + newExtension;
        }

        private static void RegisterAssemblyLocations__Generated() =>
            global::System.AppDomain.CurrentDomain.AssemblyResolve += new ResolveHandler__Generated().Resolve;

        private sealed class ResolveHandler__Generated
        {
            private global::System.Collections.Generic.IDictionary<string, string> AssembliesToLoad { get; } =
                new global::System.Collections.Generic.Dictionary<string, string>
                {
$(PARAMETER_2)
                };

            public global::System.Reflection.Assembly Resolve(object sender, global::System.ResolveEventArgs args)
            {
                using (new UnsubscribeCookie(this))
                {
                    if (!this.AssembliesToLoad.ContainsKey(args.Name)) return null;
                    return global::System.Reflection.Assembly.LoadFrom(this.AssembliesToLoad[args.Name]);
                }
            }
            
            private struct UnsubscribeCookie : global::System.IDisposable
            {
                private ResolveHandler__Generated Subscription { get; }

                public UnsubscribeCookie(ResolveHandler__Generated subscription)
                {
                    Subscription = subscription;
                    global::System.AppDomain.CurrentDomain.AssemblyResolve -= Subscription.Resolve;
                }

                public void Dispose() => global::System.AppDomain.CurrentDomain.AssemblyResolve += Subscription.Resolve;
            }
        }
    }
