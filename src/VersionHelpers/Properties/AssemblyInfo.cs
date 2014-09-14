/***************************************************************************************************
 *
 *  Managed Version Helpers Library
 *  Copyright © 2014 Florian Schneidereit. All rights reserved.
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 *  and associated documentation files (the "Software"), to deal in the Software without
 *  restriction, including without limitation the rights to use, copy, modify, merge, publish,
 *  distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 *  Software is furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in all copies or
 *  substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 *  BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 *  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 *  DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *
 **************************************************************************************************/

#region Using Directives

using System;
using System.Reflection;
using System.Runtime.InteropServices;

#endregion

#region Attributes

[assembly: AssemblyTitle("VersionHelpers")]
[assembly: AssemblyDescription("VersionHelpers-DLL")]
[assembly: AssemblyCompany("Flatcode.net")]
[assembly: AssemblyProduct("Managed Version Helpers Library")]
[assembly: AssemblyCopyright("Copyright © 2014 Flatcode.net")]

#endregion

#region Attributes: Configuration

[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: Guid("65cb0882-fa44-42d5-81e9-e2aa53d08fd3")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

#endregion

#region Attributes: Versioning

[assembly: AssemblyVersion("6.3.0.0")]
[assembly: AssemblyFileVersion("6.3")]

#endregion
