// <copyright file="PexAssemblyInfo.cs" company="lotosbin">Copyright ? 2013</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "VisualStudioUnitTest")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("BinbinDotNetOpenAuth.AspNet")]
[assembly: PexInstrumentAssembly("System.Runtime.Serialization")]
[assembly: PexInstrumentAssembly("Newtonsoft.Json")]
[assembly: PexInstrumentAssembly("TopSdk")]
[assembly: PexInstrumentAssembly("System.Web")]
[assembly: PexInstrumentAssembly("DotNetOpenAuth.AspNet")]
[assembly: PexInstrumentAssembly("log4net")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Runtime.Serialization")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Newtonsoft.Json")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "TopSdk")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Web")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "DotNetOpenAuth.AspNet")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "log4net")]

