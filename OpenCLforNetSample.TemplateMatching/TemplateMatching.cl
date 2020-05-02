typedef struct cl_scored
{
    int i;
    int j;
    float score;
} scored;

kernel void templateMatching(
               const  int   iWd,

        global const uchar* src    , 
               const  int   srcWid ,

        global const uchar* tmp    ,
               const  int   tmpWid ,
               const  int   tmpHei ,

        global scored* scoredList
        )
{
    int argI = get_global_id(0);
    int argJ = get_global_id(1);

    long dotSum = 0;
    long srcSum = 0;
    long tmpSum = 0;

    int srcIdxSlide = 3*( srcWid - tmpWid );

    int srcIdx = 3*( argI*srcWid + argJ );
    int tmpIdx = 0;

    for(int i=0; i<tmpHei; ++i){

        for(int j=0; j<tmpWid; ++j){
            int srcR = src[srcIdx++];
            int srcG = src[srcIdx++];
            int srcB = src[srcIdx++];

            int tmpR = tmp[tmpIdx++];
            int tmpG = tmp[tmpIdx++];
            int tmpB = tmp[tmpIdx++];

            dotSum += srcR*tmpR + srcG*tmpG + srcB*tmpB;
            srcSum += srcR*srcR + srcG*srcG + srcB*srcB;
            tmpSum += tmpR*tmpR + tmpG*tmpG + tmpB*tmpB;
        }

        srcIdx += srcIdxSlide;
    }

    scoredList[argI*iWd + argJ] = (struct cl_scored) {
        argI,
        argJ,
        dotSum / sqrt( (float)(srcSum*tmpSum) )
    };
}