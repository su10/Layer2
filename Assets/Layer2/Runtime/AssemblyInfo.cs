#if UNITY_EDITOR
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Layer2.Editor")]
[assembly: InternalsVisibleTo("Layer2.Tests.Editor")]
[assembly: InternalsVisibleTo("Layer2.Tests.Runtime")]
#endif
