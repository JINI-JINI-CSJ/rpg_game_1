using UnityEngine;

using System.IO;
using UnityEngine.UI;
public class FogOfWarLargeScale : MonoBehaviour
{
    public int width = 1000;
    public int height = 1000;
    private Texture2D fogTexture;
    private byte[] fogData; // bool 대신 byte 사용 (메모리 효율)

    public RawImage rawImage;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        if( fogTexture != null ) return;
        // Alpha8 포맷: 픽셀당 1바이트만 사용
        fogTexture = new Texture2D(width, height, TextureFormat.Alpha8, false);
        fogData = new byte[width * height];

        // 초기화: 모두 안개 상태 (Alpha 255)
        for (int i = 0; i < fogData.Length; i++) fogData[i] = 255;

        fogTexture.LoadRawTextureData(fogData);
        fogTexture.Apply();

        rawImage.texture = fogTexture;

//        Debug.Log( "FogOfWarLargeScale~~~~" );
    }

    public void RevealFog(int x, int z)
    {
        if( fogTexture == null ) return;

        x = Mathf.Clamp(x, 0, width- 1);
        z = Mathf.Clamp(z, 0, height - 1);

        int index = z * width + x;
        if (fogData[index] == 0) return; // 이미 해제됨

        fogData[index] = 0; // 안개 해제 (Alpha 0)
        
        // 팁: 매 프레임 모든 픽셀을 SetPixel 하지 말고, 
        // 바뀐 부분만 업데이트하는 것이 대형 맵의 핵심입니다.
        fogTexture.SetPixel(x, z, new Color(0, 0, 0, 0));
        
        // 주의: Apply()는 매번 호출하지 말고, Update 마지막에 
        // "수정 사항이 있을 때만 한 번" 호출하세요.
        shouldApply = true; 
    }

    // -1 ~ 1

    public void RevealFog_CenterRatio( Vector2 v , int expend = 0 )
    {
        if( fogTexture == null ) return;
        v.x = (v.x + 1) * 0.5f;
        v.y = (v.y + 1) * 0.5f;
        int x = (int)(v.x * (float)width);
        int y = (int)(v.y * (float)height);
        RevealFog(x,y);

        if( expend > 0 )
        {
            for( int ex = x - expend ; ex <= x + expend ; ex++ )
            {
                for( int ey = y - expend ; ey <= y + expend ; ey++ )
                {
                    RevealFog(ex,ey);
                }
            }
        }
    }

    bool shouldApply = false;
    void LateUpdate()
    {
        if (shouldApply)
        {
            fogTexture.Apply();
            shouldApply = false;
        }
    }
}



public class FogDataSerializer
{
    // 저장: BinaryWriter를 인자로 받아 RLE 압축 적용
    public void SaveFogData(BinaryWriter writer, byte[] data)
    {
        if (data == null || data.Length == 0) return;

        int n = data.Length;
        int i = 0;

        while (i < n)
        {
            byte currentValue = data[i];
            int runLength = 1;

            // 동일한 값이 연속되는지 확인 (최대 255회까지 한 단위로 묶음)
            while (i + runLength < n && data[i + runLength] == currentValue && runLength < 255)
            {
                runLength++;
            }

            // [값(1byte), 연속 횟수(1byte)] 형태로 기록
            writer.Write(currentValue);
            writer.Write((byte)runLength);

            i += runLength;
        }
    }

    // 불러오기: BinaryReader를 인자로 받아 데이터 복원
    public byte[] LoadFogData(BinaryReader reader, int totalSize)
    {
        byte[] decompressedData = new byte[totalSize];
        int currentIndex = 0;

        // 파일의 끝에 도달하거나 배열이 꽉 찰 때까지 읽음
        while (currentIndex < totalSize && reader.BaseStream.Position < reader.BaseStream.Length)
        {
            byte value = reader.ReadByte();
            byte count = reader.ReadByte();

            for (int i = 0; i < count; i++)
            {
                if (currentIndex < totalSize)
                {
                    decompressedData[currentIndex] = value;
                    currentIndex++;
                }
            }
        }
        return decompressedData;
    }
}