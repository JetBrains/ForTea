        private static void RegisterAssemblyLocations__Generated() =>
            global::System.AppDomain.CurrentDomain.AssemblyResolve += new ResolveHandler__Generated().Resolve;

        private sealed class ResolveHandler__Generated
        {
            private global::System.Collections.Generic.IDictionary<string, string> AssembliesToLoad { get; } =
                new global::System.Collections.Generic.Dictionary<string, string>
                {
$(PARAMETER_0)
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