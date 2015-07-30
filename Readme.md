# Managed Version Helpers Library #
Copyright &copy; 2014-2015 Flatcode.net  
Licensed and distributed under the terms of the MIT license.

[Official Website](http://github.com/flatcode/VersionHelpers "Official Website")  
[Official NuGet Package](http://nuget.org/packages/VersionHelpers "Official NuGet Package")

## Introduction ##
The Managed Version Helpers Library is a free and open source .NET assembly that provides a managed implementation of the native [Version Helper functions](http://msdn.microsoft.com/library/windows/desktop/dn424972.aspx "Version Helper functions") found in the Microsoft Windows&reg; SDK.

## Compilation ##
### Prerequisites ###

- Microsoft&reg; .NET Framework 4.0, or higher

### Building ###
To build the assembly, you can either use Microsoft&reg; Visual Studio&reg; or a compatible IDE that supports the Visual Studio Solution File format, or the batch files in the root of the repository (*BuildDebug.cmd*, *BuildPublish.cmd*, and *BuildRelease.cmd*).

### Remarks ###
#### Debug and Release ####
Both the *Debug* and *Release* project configurations are delay-signed by default, using the public key of the official strong-name keyfile, which is part of this repository.

#### Publish ####
Besides the traditional *Debug* and *Release* project configurations, there is a third one called *Publish* that can be used to build a version of the assembly that is configured for redistribution.

Please note that the *Publish* project configuration requires a valid strong-name keyfile named `VersionHelpers.snk` to be present in the `snk` directory; otherwise, the build process will fail. **The official strong-name keyfile is not part of this repository!** You have to provide your own public/private keypair for your (inofficial) redistributable assembly.

#### Target Frameworks ###
Every configuration supports v4.0 and v4.5 as target framework version. Support for v2.0 has been dropped with the Windows 10 release.

## Usage ##
Example C# code:

    using System.Windows;

    class Program
    {
        static void Main(String[] args)
        {
            if (VersionHelpers.IsWindows7OrGreater()) {
                Console.WriteLine("Hello on Windows 7 or greater!");
            } else {
                Console.WriteLine("You need Windows 7 or greater to be greeted!");
            }
        }
    }

Make sure to add a reference to the `VersionHelpers.dll` in order to be able to compile the example above.

In addition to the documentation found here, you can also refer to the official [MSDN documentation](http://msdn.microsoft.com/library/windows/desktop/dn424972.aspx "Version Helper functions") for further information about the original Version Helper functions.

## Implementation Details ##
### Single Source ###
Apart from using the Visual Studio solution that is used for building the assembly, you are free to use the raw C# source file of the `VersionHelpers` object (located in the file *src\VersionHelpers\Sources\VersionHelpers.cs*) in your own projects and modify it according to your needs. The entire implementation for the managed version helpers is kept in this single source file.

### Namespace ###
By default, the static `VersionHelpers` object resides in the `System.Windows` namespace because that's the place where it should naturally belong to. You are free to change that according to your needs, of course.

### Performance Considerations ###
In order to increase performance, the result of all version helper methods is cached in a static read-only field. This significantly reduces the amount of calls into unmanaged code as the specific Windows version has to be determined only once at runtime.

## License ##
The Managed Version Helpers Library is licensed and distributed under the terms of the MIT license. Please see [License.md](License.md) for the entire license text.

----------
<small>File: Readme.md</small>
