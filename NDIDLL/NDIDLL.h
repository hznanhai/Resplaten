// 下列 ifdef 块是创建使从 DLL 导出更简单的
// 宏的标准方法。此 DLL 中的所有文件都是用命令行上定义的 NDIDLL_EXPORTS
// 符号编译的。在使用此 DLL 的
// 任何其他项目上不应定义此符号。这样，源文件中包含此文件的任何其他项目都会将
// NDIDLL_API 函数视为是从 DLL 导入的，而此 DLL 则将用此宏定义的
// 符号视为是被导出的。
#ifdef NDIDLL_EXPORTS
#define NDIDLL_API extern "C" __declspec(dllexport)
#else
#define NDIDLL_API extern "C" __declspec(dllimport)
#endif


#define NUM_FRAMES		20
#define MARKERS_PORT1	4
#define MARKERS_PORT2	0
#define MARKERS_PORT3	0
#define MARKERS_PORT4	0
#define NUM_MARKERS		MARKERS_PORT1 + MARKERS_PORT2 + MARKERS_PORT3 + MARKERS_PORT4

 unsigned int
					uFlags,
					uElements,
					uFrameCnt,
					uMarkerCnt,
					uFrameNumber;
static Position3d p3dData[NUM_MARKERS];
char   szNDErrorString[MAX_ERROR_STRING_LENGTH + 1];


NDIDLL_API int MyAdd(int a, int b);

NDIDLL_API int initSystem(void);
NDIDLL_API int initCamera(void);
NDIDLL_API int shutdownSystem(void);
NDIDLL_API int get3D(Position3d *dtPosition3d0,Position3d *dtPosition3d1,Position3d *dtPosition3d2);

void errorex();
