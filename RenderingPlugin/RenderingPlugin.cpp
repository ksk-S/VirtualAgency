// Example low level rendering Unity plugin
#include "UnityPluginInterface.h"
#include <math.h>
#include <stdio.h>
#include <windows.h>
// --------------------------------------------------------------------------
// Include headers for the graphics APIs we support

#if SUPPORT_D3D9
	#include <d3d9.h>
#endif
#if SUPPORT_D3D11
	#include <d3d11.h>
#endif
#if SUPPORT_OPENGL
	#if UNITY_WIN
		#include <gl/GL.h>
	#else
		#include <OpenGL/OpenGL.h>
	#endif
#endif

bool isRenderLive   = false;
bool isRenderStream = false;

bool isTextureUpdatedLive = false;
bool isTextureUpdatedStream = false;
bool isTextureUpdatedStreamAlt = false;

bool isLiveTransparent = false;
bool isStreamTransparent = false;
bool isStreamAltTransparent = false;

unsigned char liveAlpha = 0;
unsigned char streamAlpha = 0;
unsigned char streamAltAlpha = 0;

static void* g_TexturePointerLive;
static void* g_TexturePointerStream;
static void* g_TexturePointerStreamAlt;

unsigned char* textureImageLive;
unsigned char* textureImageStream;
unsigned char* textureImageStreamAlt;

int liveTextureWidth = 1;
int liveTextureHeight = 1;

int streamTextureWidth = 1;
int streamTextureHeight = 1;


//unsigned char**	   arpBGRABuffers;
//byte** arpBGRABuffers;
// --------------------------------------------------------------------------
// Helper utilities


// Prints a string
static void DebugLog (const char* str)
{
	#if UNITY_WIN
	OutputDebugStringA (str);
	#else
	printf ("%s", str);
	#endif
}

// COM-like Release macro
#ifndef SAFE_RELEASE
#define SAFE_RELEASE(a) if (a) { a->Release(); a = NULL; }
#endif



// --------------------------------------------------------------------------
// SetTimeFromUnity, an example function we export which is called by one of the scripts.

static float g_Time;

extern "C" void EXPORT_API SetTimeFromUnity (float t) { g_Time = t; }



// --------------------------------------------------------------------------
// SetTextureFromUnity, an example function we export which is called by one of the scripts.



// variable to store the HANDLE to the hook. Don't declare it anywhere else then globally
// or you will get problems since every function uses this variable.
HHOOK _hook;

// This struct contains the data received by the hook callback. As you see in the callback function
// it contains the thing you will need: vkCode = virtual key code.
KBDLLHOOKSTRUCT kbdStruct;

// This is the callback function. Consider it the event that is raised when, in this case, 
// a key is pressed.
LRESULT __stdcall HookCallback(int nCode, WPARAM wParam, LPARAM lParam)
{
	DebugLog("Hook callback");
    if (nCode >= 0)
    {
        // the action is valid: HC_ACTION.
        if (wParam == WM_KEYDOWN)
        {
            // lParam is the pointer to the struct containing the data needed, so cast and assign it to kdbStruct.
            kbdStruct = *((KBDLLHOOKSTRUCT*)lParam);
			  // a key (non-system) is pressed.
            if (kbdStruct.vkCode == VK_F1)
            {
                // F1 is pressed!
               // MessageBox(NULL, TEXT("F1 is pressed!"), TEXT("key pressed"), MB_ICONINFORMATION);
				DebugLog("Hook F1 pressed");
            }
        }
    }

    // call the next hook in the hook chain. This is nessecary or your hook chain will break and the hook stops
    return CallNextHookEx(_hook, nCode, wParam, lParam);
}

extern "C" void SetHook()
{
	DebugLog("Set Hook");

    // Set the hook and set it to use the callback function above
    // WH_KEYBOARD_LL means it will set a low level keyboard hook. More information about it at MSDN.
    // The last 2 parameters are NULL, 0 because the callback function is in the same thread and window as the
    // function that sets and releases the hook. If you create a hack you will not need the callback function 
    // in another place then your own code file anyway. Read more about it at MSDN.
    if (!(_hook = SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, NULL, 0)))
    {
		DebugLog("Failed to install hook!");
        //MessageBox(NULL, TEXT("Failed to install hook!"), TEXT("Error"), MB_ICONERROR);
    }
}

extern "C" void ReleaseHook()
{
    UnhookWindowsHookEx(_hook);
}


//****************************************
// Specify texture pointer
//****************************************
extern "C" void EXPORT_API SetTexturePtrFromUnity (void* texturePtr, int width, int height, int type)
{
	// A script calls this at initialization time; just remember the texture pointer here.
	// Will update texture pixels each frame from the plugin rendering event (texture update
	// needs to happen on the rendering thread).
	if(type == 0){
		g_TexturePointerLive = texturePtr;
		liveTextureWidth = width;
		liveTextureHeight = height;
	}else if(type == 1){
		g_TexturePointerStream = texturePtr;
		streamTextureWidth = width;
		streamTextureHeight = height;
	}else{
		g_TexturePointerStreamAlt = texturePtr;
		streamTextureWidth = width;
		streamTextureHeight = height;

	}
}

//****************************************
//update texture contents
//****************************************
extern "C" void EXPORT_API UpdateTextureLive (unsigned char processImage[])
{
	//DebugLog ("Update Texture from Unity\n");
	textureImageLive = processImage;
	isTextureUpdatedLive = true;
}

extern "C" void EXPORT_API UpdateTextureStream (unsigned char* processImage, bool alt)
{
	//DebugLog ("Update Texture from Unity\n");
	if(!alt){
		textureImageStream = processImage;
		isTextureUpdatedStream = true;
	}else{
		textureImageStreamAlt = processImage;
		isTextureUpdatedStreamAlt = true;
	}
}


//****************************************
//change rendering status;
//****************************************
extern "C" void EXPORT_API SetLiveRenderStatus (bool status)
{
	isRenderLive = status;
}

extern "C" void EXPORT_API SetStreamRenderStatus (bool status)
{
	isRenderStream = status;
}


extern "C" void EXPORT_API SetLiveTransparent (bool trans_state, unsigned int alpha)
{
	isLiveTransparent = trans_state;
	liveAlpha = alpha;
}

extern "C" void EXPORT_API SetStreamTransparent (bool trans_state, unsigned int alpha)
{
	isStreamTransparent = trans_state;
	streamAlpha = alpha;
}

extern "C" void EXPORT_API SetStreamAltTransparent (bool trans_state, unsigned int alpha)
{
	isStreamAltTransparent = trans_state;
	streamAltAlpha = alpha;
}




// --------------------------------------------------------------------------
// UnitySetGraphicsDevice

static int g_DeviceType = -1;


// Actual setup/teardown functions defined below
#if SUPPORT_D3D9
static void SetGraphicsDeviceD3D9 (IDirect3DDevice9* device, GfxDeviceEventType eventType);
#endif
#if SUPPORT_D3D11
static void SetGraphicsDeviceD3D11 (ID3D11Device* device, GfxDeviceEventType eventType);
#endif


extern "C" void EXPORT_API UnitySetGraphicsDevice (void* device, int deviceType, int eventType)
{
	// Set device type to -1, i.e. "not recognized by our plugin"
	g_DeviceType = -1;
	
	#if SUPPORT_D3D9
	// D3D9 device, remember device pointer and device type.
	// The pointer we get is IDirect3DDevice9.
	if (deviceType == kGfxRendererD3D9)
	{
		DebugLog ("Set D3D9 graphics device\n");
		g_DeviceType = deviceType;
		SetGraphicsDeviceD3D9 ((IDirect3DDevice9*)device, (GfxDeviceEventType)eventType);
	}
	#endif

	#if SUPPORT_D3D11
	// D3D11 device, remember device pointer and device type.
	// The pointer we get is ID3D11Device.
	if (deviceType == kGfxRendererD3D11)
	{
		DebugLog ("Set D3D11 graphics device\n");
		g_DeviceType = deviceType;
		SetGraphicsDeviceD3D11 ((ID3D11Device*)device, (GfxDeviceEventType)eventType);
	}
	#endif

	#if SUPPORT_OPENGL
	// If we've got an OpenGL device, remember device type. There's no OpenGL
	// "device pointer" to remember since OpenGL always operates on a currently set
	// global context.
	if (deviceType == kGfxRendererOpenGL)
	{
		DebugLog ("Set OpenGL graphics device\n");
		g_DeviceType = deviceType;
	}
	#endif
}



// --------------------------------------------------------------------------
// UnityRenderEvent
// This will be called for GL.IssuePluginEvent script calls; eventID will
// be the integer passed to IssuePluginEvent. In this example, we just ignore
// that value.


struct MyVertex {
	float x, y, z;
	unsigned int color;
};
static void SetDefaultGraphicsState ();
static void DoRendering (const float* worldMatrix, const float* identityMatrix, float* projectionMatrix, const MyVertex* verts);


extern "C" void EXPORT_API UnityRenderEvent (int eventID)
{
	// Unknown graphics device type? Do nothing.
	if (g_DeviceType == -1)
		return;


	// A colored triangle. Note that colors will come out differently
	// in D3D9/11 and OpenGL, for example, since they expect color bytes
	// in different ordering.
	MyVertex verts[3] = {
		{ -0.5f, -0.25f,  0, 0xFFff0000 },
		{  0.5f, -0.25f,  0, 0xFF00ff00 },
		{  0,     0.5f ,  0, 0xFF0000ff },
	};


	// Some transformation matrices: rotate around Z axis for world
	// matrix, identity view matrix, and identity projection matrix.

	float phi = g_Time;
	float cosPhi = cosf(phi);
	float sinPhi = sinf(phi);

	float worldMatrix[16] = {
		cosPhi,-sinPhi,0,0,
		sinPhi,cosPhi,0,0,
		0,0,1,0,
		0,0,0.7f,1,
	};
	float identityMatrix[16] = {
		1,0,0,0,
		0,1,0,0,
		0,0,1,0,
		0,0,0,1,
	};
	float projectionMatrix[16] = {
		1,0,0,0,
		0,1,0,0,
		0,0,1,0,
		0,0,0,1,
	};

	// Actual functions defined below
	SetDefaultGraphicsState ();
	DoRendering (worldMatrix, identityMatrix, projectionMatrix, verts);
}


// -------------------------------------------------------------------
//  Direct3D 9 setup/teardown code


#if SUPPORT_D3D9

static IDirect3DDevice9* g_D3D9Device;

// A dynamic vertex buffer just to demonstrate how to handle D3D9 device resets.
static IDirect3DVertexBuffer9* g_D3D9DynamicVB;

static void SetGraphicsDeviceD3D9 (IDirect3DDevice9* device, GfxDeviceEventType eventType)
{
	g_D3D9Device = device;

	// Create or release a small dynamic vertex buffer depending on the event type.
	switch (eventType) {
	case kGfxDeviceEventInitialize:
	case kGfxDeviceEventAfterReset:
		// After device is initialized or was just reset, create the VB.
		if (!g_D3D9DynamicVB)
			g_D3D9Device->CreateVertexBuffer (1024, D3DUSAGE_WRITEONLY | D3DUSAGE_DYNAMIC, 0, D3DPOOL_DEFAULT, &g_D3D9DynamicVB, NULL);
		break;
	case kGfxDeviceEventBeforeReset:
	case kGfxDeviceEventShutdown:
		// Before device is reset or being shut down, release the VB.
		SAFE_RELEASE(g_D3D9DynamicVB);
		break;
	}
}

#endif // #if SUPPORT_D3D9



// -------------------------------------------------------------------
//  Direct3D 11 setup/teardown code


#if SUPPORT_D3D11

static ID3D11Device* g_D3D11Device;
static ID3D11Buffer* g_D3D11VB; // vertex buffer
static ID3D11Buffer* g_D3D11CB; // constant buffer
static ID3D11VertexShader* g_D3D11VertexShader;
static ID3D11PixelShader* g_D3D11PixelShader;
static ID3D11InputLayout* g_D3D11InputLayout;
static ID3D11RasterizerState* g_D3D11RasterState;
static ID3D11BlendState* g_D3D11BlendState;
static ID3D11DepthStencilState* g_D3D11DepthState;

typedef HRESULT (WINAPI *D3DCompileFunc)(
	const void* pSrcData,
	unsigned long SrcDataSize,
	const char* pFileName,
	const D3D10_SHADER_MACRO* pDefines,
	ID3D10Include* pInclude,
	const char* pEntrypoint,
	const char* pTarget,
	unsigned int Flags1,
	unsigned int Flags2,
	ID3D10Blob** ppCode,
	ID3D10Blob** ppErrorMsgs);

static const char* kD3D11ShaderText =
"cbuffer MyCB : register(b0) {\n"
"	float4x4 worldMatrix;\n"
"}\n"
"void VS (float3 pos : POSITION, float4 color : COLOR, out float4 ocolor : COLOR, out float4 opos : SV_Position) {\n"
"	opos = mul (worldMatrix, float4(pos,1));\n"
"	ocolor = color;\n"
"}\n"
"float4 PS (float4 color : COLOR) : SV_TARGET {\n"
"	return color;\n"
"}\n";


static void CreateD3D11Resources()
{
	D3D11_BUFFER_DESC desc;
	memset (&desc, 0, sizeof(desc));

	// vertex buffer
	desc.Usage = D3D11_USAGE_DEFAULT;
	desc.ByteWidth = 1024;
	desc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
	g_D3D11Device->CreateBuffer (&desc, NULL, &g_D3D11VB);

	// constant buffer
	desc.Usage = D3D11_USAGE_DEFAULT;
	desc.ByteWidth = 64; // hold 1 matrix
	desc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
	desc.CPUAccessFlags = 0;
	g_D3D11Device->CreateBuffer (&desc, NULL, &g_D3D11CB);

	// shaders
	HMODULE compiler = LoadLibraryA("D3DCompiler_43.dll");

	if (compiler == NULL)
	{
		// Try compiler from Windows 8 SDK
		compiler = LoadLibraryA("D3DCompiler_46.dll");
	}
	if (compiler)
	{
		ID3D10Blob* vsBlob = NULL;
		ID3D10Blob* psBlob = NULL;

		D3DCompileFunc compileFunc = (D3DCompileFunc)GetProcAddress (compiler, "D3DCompile");
		if (compileFunc)
		{
			HRESULT hr;
			hr = compileFunc(kD3D11ShaderText, strlen(kD3D11ShaderText), NULL, NULL, NULL, "VS", "vs_4_0", 0, 0, &vsBlob, NULL);
			if (SUCCEEDED(hr))
			{
				g_D3D11Device->CreateVertexShader (vsBlob->GetBufferPointer(), vsBlob->GetBufferSize(), NULL, &g_D3D11VertexShader);
			}

			hr = compileFunc(kD3D11ShaderText, strlen(kD3D11ShaderText), NULL, NULL, NULL, "PS", "ps_4_0", 0, 0, &psBlob, NULL);
			if (SUCCEEDED(hr))
			{
				g_D3D11Device->CreatePixelShader (psBlob->GetBufferPointer(), psBlob->GetBufferSize(), NULL, &g_D3D11PixelShader);
			}
		}

		// input layout
		if (g_D3D11VertexShader && vsBlob)
		{
			D3D11_INPUT_ELEMENT_DESC layout[] = {
				{ "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
				{ "COLOR", 0, DXGI_FORMAT_R8G8B8A8_UNORM, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
//				{ "COLOR", 0, DXGI_FORMAT_B8G8R8A8_UNORM, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },

			};

			g_D3D11Device->CreateInputLayout (layout, 2, vsBlob->GetBufferPointer(), vsBlob->GetBufferSize(), &g_D3D11InputLayout);
		}

		SAFE_RELEASE(vsBlob);
		SAFE_RELEASE(psBlob);

		FreeLibrary (compiler);
	}
	else
	{
		DebugLog ("D3D11: HLSL shader compiler not found, will not render anything\n");
	}

	// render states
	D3D11_RASTERIZER_DESC rsdesc;
	memset (&rsdesc, 0, sizeof(rsdesc));
	rsdesc.FillMode = D3D11_FILL_SOLID;
	rsdesc.CullMode = D3D11_CULL_NONE;
	rsdesc.DepthClipEnable = TRUE;
	g_D3D11Device->CreateRasterizerState (&rsdesc, &g_D3D11RasterState);

	D3D11_DEPTH_STENCIL_DESC dsdesc;
	memset (&dsdesc, 0, sizeof(dsdesc));
	dsdesc.DepthEnable = TRUE;
	dsdesc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ZERO;
	dsdesc.DepthFunc = D3D11_COMPARISON_LESS_EQUAL;
	g_D3D11Device->CreateDepthStencilState (&dsdesc, &g_D3D11DepthState);

	D3D11_BLEND_DESC bdesc;
	memset (&bdesc, 0, sizeof(bdesc));
	bdesc.RenderTarget[0].BlendEnable = FALSE;
	bdesc.RenderTarget[0].RenderTargetWriteMask = 0xF;
	g_D3D11Device->CreateBlendState (&bdesc, &g_D3D11BlendState);
}

static void ReleaseD3D11Resources()
{
	SAFE_RELEASE(g_D3D11VB);
	SAFE_RELEASE(g_D3D11CB);
	SAFE_RELEASE(g_D3D11VertexShader);
	SAFE_RELEASE(g_D3D11PixelShader);
	SAFE_RELEASE(g_D3D11InputLayout);
	SAFE_RELEASE(g_D3D11RasterState);
	SAFE_RELEASE(g_D3D11BlendState);
	SAFE_RELEASE(g_D3D11DepthState);
}

static void SetGraphicsDeviceD3D11 (ID3D11Device* device, GfxDeviceEventType eventType)
{
	g_D3D11Device = device;

	if (eventType == kGfxDeviceEventInitialize)
		CreateD3D11Resources();
	if (eventType == kGfxDeviceEventShutdown)
		ReleaseD3D11Resources();
}

#endif // #if SUPPORT_D3D11



// --------------------------------------------------------------------------
// SetDefaultGraphicsState
//
// Helper function to setup some "sane" graphics state. Rendering state
// upon call into our plugin can be almost completely arbitrary depending
// on what was rendered in Unity before.
// Before calling into the plugin, Unity will set shaders to null,
// and will unbind most of "current" objects (e.g. VBOs in OpenGL case).
//
// Here, we set culling off, lighting off, alpha blend & test off, Z
// comparison to less equal, and Z writes off.

static void SetDefaultGraphicsState ()
{
	#if SUPPORT_D3D9
	// D3D9 case
	if (g_DeviceType == kGfxRendererD3D9)
	{
		g_D3D9Device->SetRenderState (D3DRS_CULLMODE, D3DCULL_NONE);
		g_D3D9Device->SetRenderState (D3DRS_LIGHTING, FALSE);
		g_D3D9Device->SetRenderState (D3DRS_ALPHABLENDENABLE, FALSE);
		g_D3D9Device->SetRenderState (D3DRS_ALPHATESTENABLE, FALSE);
		g_D3D9Device->SetRenderState (D3DRS_ZFUNC, D3DCMP_LESSEQUAL);
		g_D3D9Device->SetRenderState (D3DRS_ZWRITEENABLE, FALSE);
	}
	#endif


	#if SUPPORT_D3D11
	// D3D11 case
	if (g_DeviceType == kGfxRendererD3D11)
	{
		ID3D11DeviceContext* ctx = NULL;
		g_D3D11Device->GetImmediateContext (&ctx);
		ctx->OMSetDepthStencilState (g_D3D11DepthState, 0);
		ctx->RSSetState (g_D3D11RasterState);
		ctx->OMSetBlendState (g_D3D11BlendState, NULL, 0xFFFFFFFF);
		ctx->Release();
	}
	#endif


	#if SUPPORT_OPENGL
	// OpenGL case
	if (g_DeviceType == kGfxRendererOpenGL)
	{
		glDisable (GL_CULL_FACE);
		glDisable (GL_LIGHTING);
		glDisable (GL_BLEND);
		glDisable (GL_ALPHA_TEST);
		glDepthFunc (GL_LEQUAL);
		glEnable (GL_DEPTH_TEST);
		glDepthMask (GL_FALSE);
	}
	#endif
}

static void ConvertRGB2RGBA(int width, int height, unsigned char* dst, unsigned char* image)
{
	unsigned char* p_dst = dst;
	unsigned char* p_src = image;
	for (int i=0; i<width*height; i++, p_dst+=4, p_src+=3)
	{
		*p_dst =  *(p_src);
		*(p_dst+1) =  *(p_src+1);
		*(p_dst+2) =  *(p_src+2);
		*(p_dst+3) =  0x0;

	}
}


static void ConvertRGBTransparent(int width, int height, unsigned char* dst, unsigned char* image, unsigned char alpha )
{
	unsigned char* p_dst = dst;
	unsigned char* p_src = image;
	for (int i=0; i<width*height; i++, p_dst+=4, p_src+=3)	//RGB for source
	{
		*p_dst =  *(p_src);
		*(p_dst+1) =  *(p_src+1);
		*(p_dst+2) =  *(p_src+2);
		*(p_dst+3) =  alpha;		
						
	}
}

static void ConvertRGBATransparent(int width, int height, unsigned char* dst, unsigned char* image)
{
	unsigned char* p_dst = dst;
	unsigned char* p_src = image;

	for (int i=0; i<width*height; i++, p_dst+=4, p_src+=4)	//RGBA for source
	{
		*p_dst =  *(p_src);
		*(p_dst+1) =  *(p_src+1);
		*(p_dst+2) =  *(p_src+2);
		*(p_dst+3) =  liveAlpha;//0x7F;		
	}
}

static void ConvertRGBtoBGR(int width, int height, unsigned char* dst, unsigned char* image)
{
	unsigned char* p_dst = dst;
	unsigned char* p_src = image;

	for (int i=0; i<width*height; i++, p_dst+=4, p_src+=4)	//RGBA for source
	{
		*p_dst = *(p_src+2);
		*(p_dst+1) = *(p_src+1);
		*(p_dst+2) = *(p_src);
	}
}


static void ConvertRGBtoBGRA(int width, int height, unsigned char* dst, unsigned char* image)
{
	unsigned char* p_dst = dst;
	unsigned char* p_src = image;

	for (int i=0; i<width*height; i++, p_dst+=4, p_src+=3)	//RGBA for source
	{
		*p_dst = *(p_src+2);
		*(p_dst+1) = *(p_src+1);
		*(p_dst+2) = *(p_src);
		*(p_dst+3) = 0x0;
	}
}

static void FillTextureFromCode (int width, int height, int stride, unsigned char* dst)
{
	const float t = g_Time * 4.0f;

	for (int y = 0; y < height; ++y)
	{
		unsigned char* ptr = dst;
		for (int x = 0; x < width; ++x)
		{
			// Simple oldskool "plasma effect", a bunch of combined sine waves
			int vv = int(
				(127.0f + (127.0f * sinf(x/7.0f+t))) +
				(127.0f + (127.0f * sinf(y/5.0f-t))) +
				(127.0f + (127.0f * sinf((x+y)/6.0f-t))) +
				(127.0f + (127.0f * sinf(sqrtf(float(x*x + y*y))/4.0f-t)))
				) / 4;

			// Write the texture pixel
			ptr[0] = vv;
			ptr[1] = vv;
			ptr[2] = vv;
			ptr[3] = vv;

			// To next pixel (our pixels are 4 bpp)
			ptr += 4;
		}

		// To next image row
		dst += stride;
	}
}


static void DoRendering (const float* worldMatrix, const float* identityMatrix, float* projectionMatrix, const MyVertex* verts)
{
	//DebugLog ("DoRendering\n");
	// Does actual rendering of a simple triangle

	#if SUPPORT_D3D9
	// D3D9 case
	if (g_DeviceType == kGfxRendererD3D9)
	{
		/*
		// Transformation matrices
		g_D3D9Device->SetTransform (D3DTS_WORLD, (const D3DMATRIX*)worldMatrix);
		g_D3D9Device->SetTransform (D3DTS_VIEW, (const D3DMATRIX*)identityMatrix);
		g_D3D9Device->SetTransform (D3DTS_PROJECTION, (const D3DMATRIX*)projectionMatrix);

		// Vertex layout
		g_D3D9Device->SetFVF (D3DFVF_XYZ|D3DFVF_DIFFUSE);

		// Texture stage states to output vertex color
		g_D3D9Device->SetTextureStageState (0, D3DTSS_COLOROP, D3DTOP_SELECTARG1);
		g_D3D9Device->SetTextureStageState (0, D3DTSS_COLORARG1, D3DTA_CURRENT);
		g_D3D9Device->SetTextureStageState (0, D3DTSS_ALPHAOP, D3DTOP_SELECTARG1);
		g_D3D9Device->SetTextureStageState (0, D3DTSS_ALPHAARG1, D3DTA_CURRENT);
		g_D3D9Device->SetTextureStageState (1, D3DTSS_COLOROP, D3DTOP_DISABLE);
		g_D3D9Device->SetTextureStageState (1, D3DTSS_ALPHAOP, D3DTOP_DISABLE);

		// Copy vertex data into our small dynamic vertex buffer. We could have used
		// DrawPrimitiveUP just fine as well.
		void* vbPtr;
		g_D3D9DynamicVB->Lock (0, 0, &vbPtr, D3DLOCK_DISCARD);
		memcpy (vbPtr, verts, sizeof(verts[0])*3);
		g_D3D9DynamicVB->Unlock ();
		g_D3D9Device->SetStreamSource (0, g_D3D9DynamicVB, 0, sizeof(MyVertex));

		// Draw!
		g_D3D9Device->DrawPrimitive (D3DPT_TRIANGLELIST, 0, 1);
		*/


		if(isRenderLive && isTextureUpdatedLive){
		// Update native texture from code
			if (g_TexturePointerLive)
			{
				IDirect3DTexture9* d3dtex = (IDirect3DTexture9*)g_TexturePointerLive;
				D3DSURFACE_DESC desc;
				d3dtex->GetLevelDesc (0, &desc);
				D3DLOCKED_RECT lr;
				d3dtex->LockRect (0, &lr, NULL, 0);
				
				/*
				char buf[100] ;
				sprintf(buf, "d3d9 %d", lr.Pitch);
				DebugLog(buf);
				*/

				//ConvertRGBtoGBR(liveTextureWidth, liveTextureHeight,  (unsigned char*)lr.pBits, (unsigned char*)textureImageLive);
				
				if(	isLiveTransparent){
					ConvertRGBATransparent(liveTextureWidth, liveTextureHeight, (unsigned char*)lr.pBits, (unsigned char*)textureImageLive);
				}else{
					//live image is RGBA, thus no need for convert
					memcpy(lr.pBits, (const unsigned char*)textureImageLive, liveTextureWidth*liveTextureHeight*4);
				}
				
				d3dtex->UnlockRect (0);
				isTextureUpdatedLive = false;
			}
		}

		
		if(isRenderStream && isTextureUpdatedStream){
		// Update native texture from code
			if (g_TexturePointerStream)
			{
				IDirect3DTexture9* d3dtex = (IDirect3DTexture9*)g_TexturePointerStream;
				D3DSURFACE_DESC desc;
				d3dtex->GetLevelDesc (0, &desc);
				D3DLOCKED_RECT lr;
				d3dtex->LockRect (0, &lr, NULL, 0);
			
				//char buf[100] ;
				//sprintf(buf, "%d", lr.Pitch);
				//DebugLog(buf);
				if(isStreamTransparent){
					ConvertRGBTransparent(streamTextureWidth, streamTextureHeight, (unsigned char*)lr.pBits, (unsigned char*)textureImageStream, streamAlpha);
				}else{
				//stream image is RGB, thus need to convert
					ConvertRGB2RGBA(streamTextureWidth, streamTextureHeight, (unsigned char*)lr.pBits, (unsigned char*)textureImageStream);
				}
				d3dtex->UnlockRect (0);
				isTextureUpdatedStream = false;
			}
		}

	
		/*
		if( (isLive && texture_updated_live) || (!isLive && texture_updated_stream)){
		// Update native texture from code
			if (g_TexturePointer)
			{
				IDirect3DTexture9* d3dtex = (IDirect3DTexture9*)g_TexturePointer;
				D3DSURFACE_DESC desc;
				d3dtex->GetLevelDesc (0, &desc);
				D3DLOCKED_RECT lr;
				d3dtex->LockRect (0, &lr, NULL, 0);

				
				if(isLive){
					//live image is RGBA, thus no need for convert
					memcpy(lr.pBits, (const unsigned char*)textureImageLive, textureWidth*textureHeight*4);
				}else{
					//stream image is RGB, thus need to convert
					ConvertRGB2RGBA(textureWidth, textureHeight, (unsigned char*)lr.pBits, (unsigned char*)textureImageStream);
				}
				//FillTextureFromCode (desc.Width, desc.Height, lr.Pitch, (unsigned char*)lr.pBits);
				d3dtex->UnlockRect (0);
				if(isLive){
					texture_updated_live = false;
				}else{
					texture_updated_stream = false;
				}
			}
		}
		*/
	}
	#endif


	#if SUPPORT_D3D11
	// D3D11 case
	if (g_DeviceType == kGfxRendererD3D11 && g_D3D11VertexShader)
	{
		ID3D11DeviceContext* ctx = NULL;
		g_D3D11Device->GetImmediateContext (&ctx);

		// update constant buffer - just the world matrix in our case
		ctx->UpdateSubresource (g_D3D11CB, 0, NULL, worldMatrix, 64, 0);

		// set shaders
		ctx->VSSetConstantBuffers (0, 1, &g_D3D11CB);
		ctx->VSSetShader (g_D3D11VertexShader, NULL, 0);
		ctx->PSSetShader (g_D3D11PixelShader, NULL, 0);

		/*
		// update vertex buffer
		ctx->UpdateSubresource (g_D3D11VB, 0, NULL, verts, sizeof(verts[0])*3, 0);

		// set input assembler data and draw
		ctx->IASetInputLayout (g_D3D11InputLayout);
		ctx->IASetPrimitiveTopology (D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
		UINT stride = sizeof(MyVertex);
		UINT offset = 0;
		ctx->IASetVertexBuffers (0, 1, &g_D3D11VB, &stride, &offset);
		ctx->Draw (3, 0);
		*/

		// update native texture from code
		if(isRenderLive && isTextureUpdatedLive){
			if (g_TexturePointerLive)
			{
				ID3D11Texture2D* d3dtex = (ID3D11Texture2D*)g_TexturePointerLive;
				D3D11_TEXTURE2D_DESC desc;
				d3dtex->GetDesc (&desc);
			
				unsigned char* data = new unsigned char[liveTextureWidth*liveTextureHeight*4];

			
				//memcpy(data, (const unsigned char*)textureImageLive, liveTextureWidth*liveTextureHeight*4);
				ConvertRGBtoBGR(liveTextureWidth, liveTextureHeight, data, (unsigned char*)textureImageLive);
				/*
				if(isLiveTransparent){
					ConvertRGBATransparent( liveTextureWidth, liveTextureHeight, data, (unsigned char*)textureImageLive);
				}else{
					memcpy(data, (const unsigned char*)textureImageLive, liveTextureWidth*liveTextureHeight*4);
				}
				*/

				ctx->UpdateSubresource (d3dtex, 0, NULL, data, desc.Width*4, 0);
				delete[] data;
			}
		}
		if(isRenderStream && isTextureUpdatedStream){
			if (g_TexturePointerStream)
			{
				ID3D11Texture2D* d3dtex = (ID3D11Texture2D*)g_TexturePointerStream;
				D3D11_TEXTURE2D_DESC desc;
				d3dtex->GetDesc (&desc);
			
				unsigned char* data = new unsigned char[streamTextureWidth*streamTextureHeight*4];
				

				ConvertRGBtoBGRA(streamTextureWidth, streamTextureHeight, data, (unsigned char*)textureImageStream);

				/*
				if(isStreamTransparent){
					ConvertRGBTransparent(streamTextureWidth, streamTextureHeight, data, (unsigned char*)textureImageStream, streamAlpha);
				}else{
				//stream image is RGB, thus need to convert
					ConvertRGB2RGBA(streamTextureWidth, streamTextureHeight, data, (unsigned char*)textureImageStream);
				}
				*/

				ctx->UpdateSubresource (d3dtex, 0, NULL, data, desc.Width*4, 0);
				delete[] data;
			}
		}
		

		ctx->Release();
	}
	#endif


	#if SUPPORT_OPENGL
	// OpenGL case
	if (g_DeviceType == kGfxRendererOpenGL)
	{
  
		// Transformation matrices
		glMatrixMode (GL_MODELVIEW);
		glLoadMatrixf (worldMatrix);
		glMatrixMode (GL_PROJECTION);
		// Tweak the projection matrix a bit to make it match what identity
		// projection would do in D3D case.
		projectionMatrix[10] = 2.0f;
		projectionMatrix[14] = -1.0f;
		glLoadMatrixf (projectionMatrix);

		// Vertex layout
		glVertexPointer (3, GL_FLOAT, sizeof(verts[0]), &verts[0].x);
		glEnableClientState (GL_VERTEX_ARRAY);
		glColorPointer (4, GL_UNSIGNED_BYTE, sizeof(verts[0]), &verts[0].color);
		glEnableClientState (GL_COLOR_ARRAY);

		// Draw!
		glDrawArrays (GL_TRIANGLES, 0, 3);

		// update native texture from code
		if (g_TexturePointer)
		{
			GLuint gltex = (GLuint)(size_t)(g_TexturePointer);
			glBindTexture (GL_TEXTURE_2D, gltex);
			int texWidth, texHeight;
			glGetTexLevelParameteriv (GL_TEXTURE_2D, 0, GL_TEXTURE_WIDTH, &texWidth);
			glGetTexLevelParameteriv (GL_TEXTURE_2D, 0, GL_TEXTURE_HEIGHT, &texHeight);

			unsigned char* data = new unsigned char[texWidth*texHeight*4];
			FillTextureFromCode (texWidth, texHeight, texHeight*4, data);
			glTexSubImage2D (GL_TEXTURE_2D, 0, 0, 0, texWidth, texHeight, GL_RGBA, GL_UNSIGNED_BYTE, data);
			delete[] data;
		}
		

	}
	#endif
}
/*
extern "C" void EXPORT_API NativeConvertAndUpdateTexture( LadybugContext context, unsigned int uiCameras, LadybugImage* pImage, LadybugImageInfo* pImageInfo)
{
	ladybugConvertToMultipleBGRU32(context, pImage, arpBGRABuffers, pImageInfo);
	ladybugUpdateTextures(context, uiCameras, (const unsigned char**) arpBGRABuffers);
}


extern "C" void EXPORT_API NativeConvertToMultipleBGRU32 ( LadybugContext context, LadybugImage* pImage, unsigned char** buffers, LadybugImageInfo* pImageInfo)
{
	ladybugConvertToMultipleBGRU32(context, pImage, buffers, pImageInfo);
	
	//ladybugConvertToMultipleBGRU32(context, pImage, arpBGRABuffers, pImageInfo);
}

extern "C" void EXPORT_API NativeUpdateTextures (LadybugContext context, unsigned int uiCameras, unsigned char** buffers)
{
	ladybugUpdateTextures(context, uiCameras, (const unsigned char**) buffers);

	//ladybugUpdateTextures(context, uiCameras, (const unsigned char**) arpBGRABuffers);
}
*/