//=============================================================================
// Copyright ｩ 2008 Point Grey Research, Inc. All Rights Reserved.
// 
// This software is the confidential and proprietary information of Point
// Grey Research, Inc. ("Confidential Information").  You shall not
// disclose such Confidential Information and shall use it only in
// accordance with the terms of the license agreement you entered into
// with Point Grey Research, Inc. (PGR).
// 
// PGR MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE SUITABILITY OF THE
// SOFTWARE, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE, OR NON-INFRINGEMENT. PGR SHALL NOT BE LIABLE FOR ANY DAMAGES
// SUFFERED BY LICENSEE AS A RESULT OF USING, MODIFYING OR DISTRIBUTING
// THIS SOFTWARE OR ITS DERIVATIVES.
//=============================================================================
//=============================================================================
// $Id: LadybugAPI.cs,v 1.11.2.1 2011/02/10 17:16:24 hirokim Exp $
//=============================================================================
//
// This file defines the interface that C# programs need to use Ladybug SDK.
// This file must be added to your C# project.
//
//=============================================================================
using System;
using System.Runtime.InteropServices;
          
namespace RenderingAPI
{
	
	unsafe public class PluginRender
	{
		//	private const string RENDER_DLL = "D:\\SkyDrive\\Unity\\Projects\\SR\\RenderingPlugin\\VisualStudio2008\\build\\Release\\RenderingPlugin.dll";
			private const string RENDER_DLL = "RenderingPlugin"; // must be in Assets\\Plugins 
		
		// Native plugin rendering events are only called if a plugin is used
		// by some script. This means we have to DllImport at least
		// one function in some active script.
		// For this example, we'll call into plugin's SetTimeFromUnity
		// function and pass the current time so the plugin can animate.
		[DllImport (RENDER_DLL, EntryPoint = "SetTimeFromUnity")]
		public static extern void SetTimeFromUnity (float t);
		// We'll also pass native pointer to a texture in Unity.
		// The plugin will fill texture data from native code.
		[DllImport (RENDER_DLL, EntryPoint = "SetTexturePtrFromUnity")]
		public static extern void SetTexturePtrFromUnity (System.IntPtr texture, int width, int height, int mode);
		
		[DllImport (RENDER_DLL, EntryPoint = "UpdateTextureStream")]
		public static extern void UpdateTextureStream (byte* processImage, bool alt);
		
		[DllImport (RENDER_DLL, EntryPoint = "UpdateTextureLive")]
		public static extern void UpdateTextureLive (byte[] processImage);


		[DllImport (RENDER_DLL, EntryPoint = "SetLiveRenderStatus")]
		public static extern void SetLiveRenderStatus (bool status);
		
		[DllImport (RENDER_DLL, EntryPoint = "SetStreamRenderStatus")]
		public static extern void SetStreamRenderStatus (bool status);
	
		[DllImport (RENDER_DLL, EntryPoint = "SetStreamAltRenderStatus")]
		public static extern void SetStreamAltRenderStatus (bool status);


		[DllImport (RENDER_DLL, EntryPoint = "SetLiveTransparent")]
		public static extern void SetLiveTransparent (bool status, byte alpha);		
		
		[DllImport (RENDER_DLL, EntryPoint = "SetStreamTransparent")]
		public static extern void SetStreamTransparent (bool status, byte alpha);

		[DllImport (RENDER_DLL, EntryPoint = "SetStreamAltTransparent")]
		public static extern void SetStreamAltTransparent (bool status, byte alpha);	


		[DllImport (RENDER_DLL, EntryPoint = "SetHook")]
		public static extern void SetHook ();		


		[DllImport (RENDER_DLL, EntryPoint = "ReleaseHook")]
		public static extern void ReleaseHook ();		
	}
	
}
