using System.Diagnostics;
using System.Reflection;

namespace NHSv2.Gateway;

public class ActivitySourceHelper
{
    public static readonly AssemblyName AssemblyName = typeof(ActivitySourceHelper).Assembly.GetName();
    
    public static readonly string ActivitySourceName = AssemblyName.Name;

    public static readonly Version Version = AssemblyName.Version;
    
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName, Version.ToString());
}