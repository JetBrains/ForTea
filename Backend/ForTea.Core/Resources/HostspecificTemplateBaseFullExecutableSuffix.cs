public static class TemplateBaseFullExecutableSuffix____GeneratedClass___LetsHopeNoOneEverNamesAClassLikeThat__IfYouNameClassLikeThatWeAreNotResponsibleForIncorrectBehaviour
{
    public static void Main(string[] args)
    {
        var transformation = new $(PARAMETER_0)();
        transformation.Host.SetOutputEncoding(Encoding.GetEncoding($(PARAMETER_1)), true);
        string destination = args[0];
        string text = transformation.TransformText();
        var encoding = transformation.Host.Encoding;
        string extension = transformation.Host.FileExtension;
        if (extension != null) destination = destination.WithExtension__Generated(extension);
        File.WriteAllText(destination, text, encoding);
    }

    public static string WithExtension__Generated(this string source, string newExtension)
    {
        if (newExtension.StartsWith(".")) newExtension = newExtension.Substring(1);
        int dotIndex = source.LastIndexOf('.');
        if (dotIndex < 0) return source;
        return source.Substring(0, dotIndex + 1) + newExtension;
    }
}
