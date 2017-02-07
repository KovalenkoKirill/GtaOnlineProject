#include <windows.h>
#include <iostream>
#include <fstream>
#include <string>
#include <Shlobj.h>
#include <dsound.h>
#include <atlstr.h>
#include <wchar.h>

HINSTANCE hExecutableInstance, hDLLInstance;
TCHAR DllPath[MAX_PATH], *DllName, szSystemPath[MAX_PATH];

struct dsound_dll
{
	HMODULE dll;
	FARPROC DirectSoundCaptureCreate;
	FARPROC DirectSoundCaptureCreate8;
	FARPROC DirectSoundCaptureEnumerateA;
	FARPROC DirectSoundCaptureEnumerateW;
	FARPROC DirectSoundCreate;
	FARPROC DirectSoundCreate8;
	FARPROC DirectSoundEnumerateA;
	FARPROC DirectSoundEnumerateW;
	FARPROC DirectSoundFullDuplexCreate;
	FARPROC DllCanUnloadNow_dsound;
	FARPROC DllGetClassObject_dsound;
	FARPROC GetDeviceID;
} dsound;

struct dinput8_dll
{
	HMODULE dll;
	FARPROC DirectInput8Create;
	FARPROC DllCanUnloadNow;
	FARPROC DllGetClassObject;
	FARPROC DllRegisterServer;
	FARPROC DllUnregisterServer;
} dinput8;

typedef HRESULT(*fn_DirectSoundCaptureCreate)(LPGUID lpGUID, LPDIRECTSOUNDCAPTURE *lplpDSC, LPUNKNOWN pUnkOuter);
void _DirectSoundCaptureCreate() { (fn_DirectSoundCaptureCreate)dsound.DirectSoundCaptureCreate(); }

typedef HRESULT(*fn_DirectSoundCaptureCreate8)(LPCGUID lpcGUID, LPDIRECTSOUNDCAPTURE8 * lplpDSC, LPUNKNOWN pUnkOuter);
void _DirectSoundCaptureCreate8() { (fn_DirectSoundCaptureCreate8)dsound.DirectSoundCaptureCreate8(); }

typedef HRESULT(*fn_DirectSoundCaptureEnumerateA)(LPDSENUMCALLBACKA lpDSEnumCallback, LPVOID lpContext);
void _DirectSoundCaptureEnumerateA() { (fn_DirectSoundCaptureEnumerateA)dsound.DirectSoundCaptureEnumerateA(); }

typedef HRESULT(*fn_DirectSoundCaptureEnumerateW)(LPDSENUMCALLBACKW lpDSEnumCallback, LPVOID lpContext);
void _DirectSoundCaptureEnumerateW() { (fn_DirectSoundCaptureEnumerateW)dsound.DirectSoundCaptureEnumerateW(); }

typedef HRESULT(*fn_DirectSoundCreate)(LPCGUID lpcGUID, LPDIRECTSOUND* ppDS, IUnknown* pUnkOuter);
void _DirectSoundCreate() { (fn_DirectSoundCreate)dsound.DirectSoundCreate(); }

typedef HRESULT(*fn_DirectSoundCreate8)(LPCGUID lpcGUID, LPDIRECTSOUND8* ppDS, IUnknown* pUnkOuter);
void _DirectSoundCreate8() { (fn_DirectSoundCreate8)dsound.DirectSoundCreate8(); }

typedef HRESULT(*fn_DirectSoundEnumerateA)(LPDSENUMCALLBACKA lpDSEnumCallback, LPVOID lpContext);
void _DirectSoundEnumerateA() { (fn_DirectSoundEnumerateA)dsound.DirectSoundEnumerateA(); }

typedef HRESULT(*fn_DirectSoundEnumerateW)(LPDSENUMCALLBACKW lpDSEnumCallback, LPVOID lpContext);
void _DirectSoundEnumerateW() { (fn_DirectSoundEnumerateW)dsound.DirectSoundEnumerateW(); }

typedef HRESULT(*fn_DirectSoundFullDuplexCreate)(const GUID* capture_dev, const GUID* render_dev, const DSCBUFFERDESC* cbufdesc, const DSBUFFERDESC* bufdesc, HWND  hwnd, DWORD level, IDirectSoundFullDuplex**  dsfd, IDirectSoundCaptureBuffer8** dscb8, IDirectSoundBuffer8** dsb8, IUnknown* outer_unk);
void _DirectSoundFullDuplexCreate() { (fn_DirectSoundFullDuplexCreate)dsound.DirectSoundFullDuplexCreate(); }

typedef HRESULT(*fn_DllCanUnloadNow_dsound)();
void _DllCanUnloadNow_dsound() { (fn_DllCanUnloadNow_dsound)dsound.DllCanUnloadNow_dsound(); }

typedef HRESULT(*fn_DllGetClassObject_dsound)(REFCLSID rclsid, REFIID   riid, LPVOID   *ppv);
void _DllGetClassObject_dsound() { (fn_DllGetClassObject_dsound)dsound.DllGetClassObject_dsound(); }

typedef HRESULT(*fn_GetDeviceID)(LPCGUID pGuidSrc, LPGUID pGuidDest);
void _GetDeviceID() { (fn_GetDeviceID)dsound.GetDeviceID(); }


typedef HRESULT(*fn_DirectInput8Create)(HINSTANCE hinst, DWORD dwVersion, REFIID riidltf, LPVOID * ppvOut, LPUNKNOWN punkOuter);
void _DirectInput8Create() { (fn_DirectInput8Create)dinput8.DirectInput8Create(); }

typedef HRESULT(*fn_DllCanUnloadNow)();
void _DllCanUnloadNow() { (fn_DllCanUnloadNow)dinput8.DllCanUnloadNow(); }

typedef HRESULT(*fn_DllGetClassObject)(REFCLSID rclsid, REFIID riid, LPVOID *ppv);
void _DllGetClassObject() { (fn_DllGetClassObject)dinput8.DllGetClassObject(); }

typedef HRESULT(*fn_DllRegisterServer)();
void _DllRegisterServer() { (fn_DllRegisterServer)dinput8.DllRegisterServer(); }

typedef HRESULT(*fn_DllUnregisterServer)();
void _DllUnregisterServer() { (fn_DllUnregisterServer)dinput8.DllUnregisterServer(); }

void LoadOriginalLibrary()
{
	if (_stricmp((char*)DllName + 1, "dsound.dll") == NULL) {
		dsound.dll = LoadLibrary(szSystemPath);
		dsound.DirectSoundCaptureCreate = GetProcAddress(dsound.dll, "DirectSoundCaptureCreate");
		dsound.DirectSoundCaptureCreate8 = GetProcAddress(dsound.dll, "DirectSoundCaptureCreate8");
		dsound.DirectSoundCaptureEnumerateA = GetProcAddress(dsound.dll, "DirectSoundCaptureEnumerateA");
		dsound.DirectSoundCaptureEnumerateW = GetProcAddress(dsound.dll, "DirectSoundCaptureEnumerateW");
		dsound.DirectSoundCreate = GetProcAddress(dsound.dll, "DirectSoundCreate");
		dsound.DirectSoundCreate8 = GetProcAddress(dsound.dll, "DirectSoundCreate8");
		dsound.DirectSoundEnumerateA = GetProcAddress(dsound.dll, "DirectSoundEnumerateA");
		dsound.DirectSoundEnumerateW = GetProcAddress(dsound.dll, "DirectSoundEnumerateW");
		dsound.DirectSoundFullDuplexCreate = GetProcAddress(dsound.dll, "DirectSoundFullDuplexCreate");
		dsound.DllCanUnloadNow_dsound = GetProcAddress(dsound.dll, "DllCanUnloadNow");
		dsound.DllGetClassObject_dsound = GetProcAddress(dsound.dll, "DllGetClassObject");
		dsound.GetDeviceID = GetProcAddress(dsound.dll, "GetDeviceID");
	}
	else
	{
		if (_stricmp((char*)DllName + 1, "dinput8.dll") == NULL)
		{
			dinput8.dll = LoadLibrary(szSystemPath);
			dinput8.DirectInput8Create = GetProcAddress(dinput8.dll, "DirectInput8Create");
			dinput8.DllCanUnloadNow = GetProcAddress(dinput8.dll, "DllCanUnloadNow");
			dinput8.DllGetClassObject = GetProcAddress(dinput8.dll, "DllGetClassObject");
			dinput8.DllRegisterServer = GetProcAddress(dinput8.dll, "DllRegisterServer");
			dinput8.DllUnregisterServer = GetProcAddress(dinput8.dll, "DllUnregisterServer");
		}
		else
		{
			ExitProcess(0);
		}
	}
}

const char * WinGetEnv(const char * name)
{
	const DWORD buffSize = 65535;
	static char buffer[buffSize];
	if (GetEnvironmentVariableA(name, buffer, buffSize))
	{
		return buffer;
	}
	else
	{
		return 0;
	}
}

void LoadGtaLibrary()
{
	const char * LauncherPath = WinGetEnv("LauncherPath");
	if (LauncherPath != NULL)
	{
		std::string ScriptHook = std::string(LauncherPath) + "ScriptHookV.dll";
		std::string ScriptHookDotNet = std::string(LauncherPath) + "ScriptHookVDotNet.dll";
		HINSTANCE  handle1 = LoadLibrary(ScriptHook.c_str());
		HINSTANCE  handle2 = LoadLibrary(ScriptHookDotNet.c_str());

	}
}
void LoadPlugins()
{
	LoadOriginalLibrary();
	LoadGtaLibrary();
}


BOOL WINAPI DllMain(HINSTANCE hInst, DWORD reason, LPVOID)
{
	if (reason == DLL_PROCESS_ATTACH)
	{
		hExecutableInstance = GetModuleHandle(NULL); // passing NULL should be safe even with the loader lock being held (according to ReactOS ldr.c)
		GetModuleFileName(hInst, DllPath, MAX_PATH);
		DllName = (TCHAR*)strrchr((char*)DllPath, '\\');
		SHGetFolderPath(NULL, CSIDL_SYSTEM, NULL, 0, szSystemPath);
		strcat((char*)szSystemPath, (char*)DllName);

		LoadPlugins();
	}

	if (reason == DLL_PROCESS_DETACH)
	{
		FreeLibrary(dsound.dll);
		FreeLibrary(dinput8.dll);
	}
	return TRUE;
}
int main() {
	LoadGtaLibrary();
	return 0;
}