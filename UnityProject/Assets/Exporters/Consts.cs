public class Consts
{
    public static string BSIPAPluginStarter =
    @"namespace WeatherCustomScript { [Plugin(RuntimeOptions.SingleStartInit)] class Plugin
{
}
}
";

    public static string UsingIPA =
    @"using IPA;
";

    public static string IPADLLPath = "/DLLS/IPA/IPA.Loader.dll";
    public static string OutputPath = "/DLLS/CustomScriptsOutput";
}