// NDIDLL.cpp : 定义 DLL 应用程序的导出函数。
//


#include "stdafx.h"
#include "NDIDLL.h"

#include "sleep.c"
#include "ot_aux.c"
#include "certus_aux.c"



NDIDLL_API int MyAdd(int a, int b){
	return a+b;
}


NDIDLL_API int initSystem(void){
		//step1
		fprintf( stdout, "...TransputerLoadSystem\n" );
		if( TransputerLoadSystem( "system" ) != OPTO_NO_ERROR_CODE )
		{
			errorex();
		} 
		sleep( 1 );
		
		//step2
		fprintf( stdout, "...TransputerInitializeSystem\n" );
		if( TransputerInitializeSystem( OPTO_LOG_ERRORS_FLAG | OPTO_LOG_MESSAGES_FLAG ) )
		{
			errorex();
		} 
	
		return 0;
}

NDIDLL_API int initCamera(void){
		//step3
		fprintf( stdout, "...OptotrakSetProcessingFlags\n" );
		if( OptotrakSetProcessingFlags( OPTO_LIB_POLL_REAL_DATA |
										OPTO_CONVERT_ON_HOST |
										OPTO_RIGID_ON_HOST ) )
		{
			errorex();
		}
		
		//step4
		fprintf( stdout, "...OptotrakLoadCameraParameters\n" );
		if( OptotrakLoadCameraParameters( "standard" ) )
		{
			errorex();
		}
		
		//step5
		fprintf( stdout, "...OptotrakSetStroberPortTable\n" );
		if( OptotrakSetStroberPortTable( MARKERS_PORT1, MARKERS_PORT2, MARKERS_PORT3, MARKERS_PORT4 ) )
		{
			errorex();
		} 

		//step6
		fprintf( stdout, "...OptotrakSetupCollection\n" );
		if( OptotrakSetupCollection(
				NUM_MARKERS,    /* Number of markers in the collection. */
				(float)250.0,   /* Frequency to collect data frames at. */
				(float)2500.0,  /* Marker frequency for marker maximum on-time. */
				30,             /* Dynamic or Static Threshold value to use. */
				160,            /* Minimum gain code amplification to use. */
				0,              /* Stream mode for the data buffers. */
				(float)0.35,    /* Marker Duty Cycle to use. */
				(float)7.0,     /* Voltage to use when turning on markers. */
				(float)1.0,     /* Number of seconds of data to collect. */
				(float)0.0,     /* Number of seconds to pre-trigger data by. */
				OPTOTRAK_NO_FIRE_MARKERS_FLAG | OPTOTRAK_BUFFER_RAW_FLAG | OPTOTRAK_GET_NEXT_FRAME_FLAG ) )
		{
			errorex();
		}
		
		/*
		 * Wait one second to let the camera adjust.
		*/
		sleep( 1 );

		/*
		 * Activate the markers.
		 */
		fprintf( stdout, "...OptotrakActivateMarkers\n" );
		if( OptotrakActivateMarkers() )
		{
			errorex();
		} /* if */
		sleep( 1 );
		return 0;
}


NDIDLL_API int shutdownSystem(){
		/*
		 * De-activate the markers.
		 */
		fprintf( stdout, "...OptotrakDeActivateMarkers\n" );
		if( OptotrakDeActivateMarkers() )
		{
			errorex();
		} /* if */

		/*
		 * Shutdown the processors message passing system.
		 */
		fprintf( stdout, "...TransputerShutdownSystem\n" );
		if( TransputerShutdownSystem() )
		{
			errorex();
		} /* if */

		/*
		 * Exit the program.
		 */
		fprintf( stdout, "\nProgram execution complete.\n" );
		return 0;
}

/*
NDIDLL_API int get3D(Position3d *dtPosition3d){

		if( DataGetLatest3D( &uFrameNumber, &uElements, &uFlags, p3dData) )
		{
			errorex();
		}
			
		*dtPosition3d=p3dData[0];

		return 0;
}
*/

NDIDLL_API int get3D(Position3d *dtPosition3d0, Position3d *dtPosition3d1, Position3d *dtPosition3d2){

		if( DataGetLatest3D( &uFrameNumber, &uElements, &uFlags, p3dData) )
		{
			errorex();
		}
			
		*dtPosition3d0=p3dData[0];
		*dtPosition3d1=p3dData[1];
		*dtPosition3d2=p3dData[2];
		return 0;
}


void errorex(){
	fprintf( stdout, "\nAn error has occurred during execution of the program.\n" );
    if( OptotrakGetErrorString( szNDErrorString,
                                MAX_ERROR_STRING_LENGTH + 1 ) == 0 )
    {
        fprintf( stdout, szNDErrorString );
    } 

	fprintf( stdout, "\n\n...TransputerShutdownSystem\n" );
    TransputerShutdownSystem();
	exit(1);
}

