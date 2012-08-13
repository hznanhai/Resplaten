// ���� ifdef ���Ǵ���ʹ�� DLL �������򵥵�
// ��ı�׼�������� DLL �е������ļ��������������϶���� NDIDLL_EXPORTS
// ���ű���ġ���ʹ�ô� DLL ��
// �κ�������Ŀ�ϲ�Ӧ����˷��š�������Դ�ļ��а������ļ����κ�������Ŀ���Ὣ
// NDIDLL_API ������Ϊ�Ǵ� DLL ����ģ����� DLL ���ô˺궨���
// ������Ϊ�Ǳ������ġ�
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
