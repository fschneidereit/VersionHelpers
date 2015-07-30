/***************************************************************************************************
 *
 *  Copyright © 2014-2015 Flatcode.net
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

#endregion

namespace System.Windows
{
	/// <summary>
	/// Provides static methods that can be used to determine the current OS version or identify
	/// whether it is a Windows or Windows Server release.
	/// </summary>
	public static class VersionHelpers
	{
		#region Fields

		//
		// The result of each version helper method is cached to a static field in order to increase
		// performance as this effectively reduces the amount of necessary calls into unmanaged code
		// on subsequent invocations.
		//
		static readonly Boolean windowsXPOrGreater;
		static readonly Boolean windowsXPSP1OrGreater;
		static readonly Boolean windowsXPSP2OrGreater;
		static readonly Boolean windowsXPSP3OrGreater;
		static readonly Boolean windowsVistaOrGreater;
		static readonly Boolean windowsVistaSP1OrGreater;
		static readonly Boolean windowsVistaSP2OrGreater;
		static readonly Boolean windows7OrGreater;
		static readonly Boolean windows7SP1OrGreater;
		static readonly Boolean windows8OrGreater;
		static readonly Boolean windows8Point1OrGreater;
        static readonly Boolean windows10OrGreater;
		static readonly Boolean windowsServer;

		#endregion

		#region Constructors

		static VersionHelpers()
		{
            // Static initialization in reverse order from newest to oldest OS version to ensure
            // IsWindowsVersionOrGreater() is only called until a match is found.
            windows10OrGreater = IsWindowsVersionOrGreater(10, 0, 0);

            if (!windows10OrGreater) {
                windows8Point1OrGreater = IsWindowsVersionOrGreater(6, 3, 0);
            } else {
                windows8Point1OrGreater = true;
            }

			if (!windows8Point1OrGreater) {
				windows8OrGreater = IsWindowsVersionOrGreater(6, 2, 0);
			} else {
				windows8OrGreater = true;
			}

			if (!windows8OrGreater) {
				windows7SP1OrGreater = IsWindowsVersionOrGreater(6, 1, 1);
			} else {
				windows7SP1OrGreater = true;
			}

			if (!windows7SP1OrGreater) {
				windows7OrGreater = IsWindowsVersionOrGreater(6, 1, 0);
			} else {
				windows7OrGreater = true;
			}

			if (!windows7OrGreater) {
				windowsVistaSP2OrGreater = IsWindowsVersionOrGreater(6, 0, 2);
			} else {
				windowsVistaSP2OrGreater = true;
			}

			if (!windowsVistaSP2OrGreater) {
				windowsVistaSP1OrGreater = IsWindowsVersionOrGreater(6, 0, 1);
			} else {
				windowsVistaSP1OrGreater = true;
			}

			if (!windowsVistaSP1OrGreater) {
				windowsVistaOrGreater = IsWindowsVersionOrGreater(6, 0, 0);
			} else {
				windowsVistaOrGreater = true;
			}

			if (!windowsVistaOrGreater) {
				windowsXPSP3OrGreater = IsWindowsVersionOrGreater(5, 1, 3);
			} else {
				windowsXPSP3OrGreater = true;
			}

			if (!windowsXPSP3OrGreater) {
				windowsXPSP2OrGreater = IsWindowsVersionOrGreater(5, 1, 2);
			} else {
				windowsXPSP2OrGreater = true;
			}

			if (!windowsXPSP2OrGreater) {
				windowsXPSP1OrGreater = IsWindowsVersionOrGreater(5, 1, 1);
			} else {
				windowsXPSP1OrGreater = true;
			}

			if (!windowsXPSP1OrGreater) {
				windowsXPOrGreater = IsWindowsVersionOrGreater(5, 1, 0);
			} else {
				windowsXPOrGreater = true;
			}

			// Static initialization of product type
			windowsServer = IsWindowsServerInternal();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Indicates if the current OS version matches, or is greater than, the provided version
		/// information. This method is useful in confirming a version of Windows Server that
		/// doesn't share a version number with a client release.
		/// </summary>
		/// <remarks>
		/// You should only use this method if the other provided version helper methods do not fit
		/// your scenario.
		/// </remarks>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="majorVersion"/> is smaller than 5 or <paramref name="minorVersion"/>
		/// and/or <paramref name="servicePackMajor"/> are smaller than 0.
		/// </exception>
		/// <param name="majorVersion">The major OS version number.</param>
		/// <param name="minorVersion">The minor OS version number.</param>
		/// <param name="servicePackMajor">The major Service Pack version number.</param>
		/// <returns>True if the specified version matches, or is greater than, the version of the
		/// current Windows OS; otherwise, false.</returns>
		public static Boolean IsWindowsVersionOrGreater(Int32 majorVersion, Int32 minorVersion,
														Int32 servicePackMajor)
		{
			// Validate arguments
			if (majorVersion < 5) {
				// Error: Major version cannot be smaller than 5
				throw new ArgumentOutOfRangeException("majorVersion");
			}

			if (minorVersion < 0) {
				// Error: Minor version cannot be negative
				throw new ArgumentOutOfRangeException("minorVersion");
			}

			if (servicePackMajor < 0) {
				// Error: Major service pack version cannot be negative
				throw new ArgumentOutOfRangeException("servicePackMajor");
			}

			// Initialize OSVERSIONINFOEX structure
			OSVERSIONINFOEX osvi = new OSVERSIONINFOEX();
			osvi.dwOSVersionInfoSize = (UInt32)Marshal.SizeOf(typeof(OSVERSIONINFOEX));
			osvi.dwMajorVersion = (UInt32)majorVersion;
			osvi.dwMinorVersion = (UInt32)minorVersion;
			osvi.wServicePackMajor = (UInt16)servicePackMajor;

			// Initialize condition mask
			UInt64 conditionMask =
				VerSetConditionMask(
					VerSetConditionMask(
						VerSetConditionMask(0,
											VER_MAJORVERSION,
											VER_GREATER_EQUAL),
										VER_MINORVERSION,
										VER_GREATER_EQUAL),
									VER_SERVICEPACKMAJOR,
									VER_GREATER_EQUAL);

			// Verify version info
			return VerifyVersionInfo(ref osvi,
									 VER_MAJORVERSION | VER_MINORVERSION | VER_SERVICEPACKMAJOR,
									 conditionMask);
		}

		/// <summary>
		/// Indicates if the current OS version matches, or is greater than, the Windows XP version.
		/// </summary>
		/// <returns>True if the current OS version matches, or is greater than, the Windows XP
		/// version; otherwise, false.</returns>
		#if AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static Boolean IsWindowsXPOrGreater()
		{
			return windowsXPOrGreater;
		}

		/// <summary>
		/// Indicates if the current OS version matches, or is greater than, the Windows XP with
		/// Service Pack 1 (SP1) version.
		/// </summary>
		/// <returns>True if the current OS version matches, or is greater than, the Windows XP
		/// with SP1 version; otherwise, false.</returns>
		#if AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static Boolean IsWindowsXPSP1OrGreater()
		{
			return windowsXPSP1OrGreater;
		}

		/// <summary>
		/// Indicates if the current OS version matches, or is greater than, the Windows XP with
		/// Service Pack 2 (SP2) version.
		/// </summary>
		/// <returns>True if the current OS version matches, or is greater than, the Windows XP
		/// with SP2 version; otherwise, false.</returns>
		#if AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static Boolean IsWindowsXPSP2OrGreater()
		{
			return windowsXPSP2OrGreater;
		}

		/// <summary>
		/// Indicates if the current OS version matches, or is greater than, the Windows XP with
		/// Service Pack 3 (SP3) version.
		/// </summary>
		/// <returns>True if the current OS version matches, or is greater than, the Windows XP
		/// with SP3 version; otherwise, false.</returns>
		#if AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static Boolean IsWindowsXPSP3OrGreater()
		{
			return windowsXPSP3OrGreater;
		}

		/// <summary>
		/// Indicates if the current OS version matches, or is greater than, the Windows Vista
		/// version.
		/// </summary>
		/// <returns>True if the current OS version matches, or is greater than, the Windows Vista
		/// version; otherwise, false.</returns>
		#if AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static Boolean IsWindowsVistaOrGreater()
		{
			return windowsVistaOrGreater;
		}

		/// <summary>
		/// Indicates if the current OS version matches, or is greater than, the Windows Vista with
		/// Service Pack 1 (SP1) version.
		/// </summary>
		/// <returns>True if the current OS version matches, or is greater than, the Windows Vista
		/// with SP1 version; otherwise, false.</returns>
		#if AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static Boolean IsWindowsVistaSP1OrGreater()
		{
			return windowsVistaSP1OrGreater;
		}

		/// <summary>
		/// Indicates if the current OS version matches, or is greater than, the Windows Vista with
		/// Service Pack 2 (SP2) version.
		/// </summary>
		/// <returns>True if the current OS version matches, or is greater than, the Windows Vista
		/// with SP2 version; otherwise, false.</returns>
		#if AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static Boolean IsWindowsVistaSP2OrGreater()
		{
			return windowsVistaSP2OrGreater;
		}

		/// <summary>
		/// Indicates if the current OS version matches, or is greater than, the Windows 7 version.
		/// </summary>
		/// <returns>True if the current OS version matches, or is greater than, the Windows 7
		/// version; otherwise, false.</returns>
		#if AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static Boolean IsWindows7OrGreater()
		{
			return windows7OrGreater;
		}

		/// <summary>
		/// Indicates if the current OS version matches, or is greater than, the Windows 7 with
		/// Service Pack 1 (SP1) version.
		/// </summary>
		/// <returns>True if the current OS version matches, or is greater than, the Windows 7 with
		/// SP1 version; otherwise, false.</returns>
		#if AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static Boolean IsWindows7SP1OrGreater()
		{
			return windows7SP1OrGreater;
		}

		/// <summary>
		/// Indicates if the current OS version matches, or is greater than, the Windows 8 version.
		/// </summary>
		/// <returns>True if the current OS version matches, or is greater than, the Windows 8
		/// version; otherwise, false.</returns>
		#if AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static Boolean IsWindows8OrGreater()
		{
			return windows8OrGreater;
		}

		/// <summary>
		/// Indicates if the current OS version matches, or is greater than, the Windows 8.1
		/// version.
		/// </summary>
		///  <returns>True if the current OS version matches, or is greater than, the Windows 8.1
		/// version; otherwise, false.</returns>
		#if AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static Boolean IsWindows8Point1OrGreater()
		{
			return windows8Point1OrGreater;
		}

        /// <summary>
		/// Indicates if the current OS version matches, or is greater than, the Windows 10
		/// version.
		/// </summary>
		///  <returns>True if the current OS version matches, or is greater than, the Windows 10
		/// version; otherwise, false.</returns>
		#if AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static Boolean IsWindows10OrGreater()
		{
			return windows10OrGreater;
		}

		/// <summary>
		/// Indicates if the current OS is a Windows Server release. Applications that need to
		/// distinguish between server and client versions of Windows should call this method.
		/// </summary>
		/// <returns>True if the current OS is a Windows Server version; otherwise, false.</returns>
		#if AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		#endif
		public static Boolean IsWindowsServer()
		{
			return windowsServer;
		}

		#endregion

		#region Methods: Internal

		internal static Boolean IsWindowsServerInternal()
		{
			// Initialize OSVERSIONINFOEX structure
			OSVERSIONINFOEX osvi = new OSVERSIONINFOEX();
			osvi.dwOSVersionInfoSize = (UInt32)Marshal.SizeOf(typeof(OSVERSIONINFOEX));
			osvi.wProductType = VER_NT_WORKSTATION;

			// Initialize condition mask
			UInt64 conditionMask = VerSetConditionMask(0, VER_PRODUCT_TYPE, VER_EQUAL);

			// Verify version info
			return !VerifyVersionInfo(ref osvi, VER_PRODUCT_TYPE, conditionMask);
		}

		#endregion

		#region P/Invoke: Constants

		// Version type masks
		const UInt32 VER_MAJORVERSION = 0x00000020;
		const UInt32 VER_MINORVERSION = 0x00000010;
		const UInt32 VER_SERVICEPACKMAJOR = 0x00000200;
		const UInt32 VER_PRODUCT_TYPE = 0x00000800;

		// Version condition masks
		const Byte VER_EQUAL = 0x01;
		const Byte VER_GREATER_EQUAL = 0x03;

		// Version product types
		const Byte VER_NT_WORKSTATION = 0x10;

		#endregion

		#region P/Invoke: Structures

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		struct OSVERSIONINFOEX
		{
			public UInt32 dwOSVersionInfoSize;
			public UInt32 dwMajorVersion;
			public UInt32 dwMinorVersion;
			public UInt32 dwBuildNumber;
			public UInt32 dwPlatformId;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public String szCSDVersion;

			public UInt16 wServicePackMajor;
			public UInt16 wServicePackMinor;
			public UInt16 wSuiteMask;
			public Byte wProductType;
			public Byte wReserved;
		}

		#endregion

		#region P/Invoke: Methods

		[DllImport("Kernel32.dll",
                   CallingConvention = CallingConvention.Winapi,
				   CharSet = CharSet.Unicode,
                   SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern Boolean VerifyVersionInfo(
				[In] ref OSVERSIONINFOEX lpVersionInfo,
				[In] UInt32 dwTypeMask,
				[In] UInt64 dwlConditionMask);

		[DllImport("Kernel32.dll",
				   CallingConvention = CallingConvention.Winapi)]
		[SuppressUnmanagedCodeSecurity]
		static extern UInt64 VerSetConditionMask(
			[In] UInt64 dwlConditionMask,
			[In] UInt32 dwTypeBitMask,
			[In] Byte ConditionMask);

		#endregion
	}
}
