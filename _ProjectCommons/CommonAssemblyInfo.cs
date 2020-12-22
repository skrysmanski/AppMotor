using System;

// Mark this assembly as "CLS compliant" or "not CLS compliant".
//
// You should always mark an assembly as "CLS compliant", if possible (this is also the default in "Project.Code.props").
//
// The only reason to mark an assembly as "not CLS compliant" is if it's using "not CLS compliant"
// dependencies that have "public" or "protected" visibility. ASP.NET Core is an example of such a dependency.
//
// To mark this assembly as "not CLS compliant", add the property "<ClsCompliant>false</ClsCompliant>" to
// your project file.
//
// For details, see: https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/CA1014
//
// NOTE: This can't be done in a .csproj file at the moment: https://github.com/dotnet/msbuild/issues/2281
#if CLS_COMPLIANT
[assembly: CLSCompliant(true)]
#else
[assembly: CLSCompliant(false)]
#endif
