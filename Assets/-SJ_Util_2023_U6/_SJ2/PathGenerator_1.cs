using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq; // for sorting

public class PathGenerator_1 : MonoBehaviour
{
    [Header("Image Settings")]
    [Tooltip("생성할 이미지의 가로 크기 (픽셀)")]
    public int imageWidth = 512;
    [Tooltip("생성할 이미지의 세로 크기 (픽셀)")]
    public int imageHeight = 512;

    [Header("Path Generation")]
    [Tooltip("생성할 주요 길의 개수")]
    public int numberOfMainPaths = 3;
    [Tooltip("하나의 길을 구성하는 경유지의 수 (많을수록 복잡)")]
    [Range(2, 20)]
    public int pointsPerPath = 5;
    [Tooltip("길의 굵기 (픽셀)")]
    [Range(1, 50)]
    public int pathThickness = 10;
    
    [Header("Natural Wiggle (Perlin Noise)")]
    [Tooltip("구불거림의 빈도 (작을수록 완만한 커브)")]
    public float noiseScale = 0.05f;
    [Tooltip("구불거림의 강도 (클수록 심하게 구불거림)")]
    public float noiseStrength = 15f;
    
    [Header("File Output")]
    [Tooltip("저장할 파일 이름 (확장자 제외)")]
    public string outputFileName = "GeneratedPath";

    // 인스펙터에서 이 함수를 바로 실행할 수 있게 해주는 어트리뷰트입니다.
    // 컴포넌트의 케밥 메뉴 (세로 점 3개)를 누르면 "Generate Path Image"가 보입니다.
    [ContextMenu("Generate Path Image")]
    public void GenerateAndSavePathImage()
    {
        // 1. 텍스처 생성 및 초기화 (전체를 검은색으로 채우기)
        Texture2D texture = new Texture2D(imageWidth, imageHeight);
        for (int y = 0; y < imageHeight; y++)
        {
            for (int x = 0; x < imageWidth; x++)
            {
                texture.SetPixel(x, y, Color.black);
            }
        }

        // 2. 설정된 개수만큼 주요 경로 생성
        for (int i = 0; i < numberOfMainPaths; i++)
        {
            GenerateSinglePath(texture);
        }

        // 3. 텍스처 변경사항 적용
        texture.Apply();

        // 4. PNG 파일로 저장
        SaveTextureToFile(texture, outputFileName);

        // 메모리 정리
        if (Application.isEditor)
        {
            DestroyImmediate(texture);
        }
        else
        {
            Destroy(texture);
        }
    }
    
    private void GenerateSinglePath(Texture2D texture)
    {
        // a. 랜덤 경유지 생성
        List<Vector2> waypoints = new List<Vector2>();
        for (int i = 0; i < pointsPerPath; i++)
        {
            waypoints.Add(new Vector2(
                Random.Range(0, imageWidth),
                Random.Range(0, imageHeight)
            ));
        }

        // b. 경로가 자연스럽게 이어지도록 X좌표 기준으로 정렬 (왼쪽 -> 오른쪽)
        waypoints = waypoints.OrderBy(p => p.x).ToList();

        // c. 캣멀롬 스플라인으로 부드러운 경로 계산 및 그리기
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            // 캣멀롬 스플라인은 4개의 점이 필요 (p0, p1, p2, p3)
            // p1과 p2 사이의 곡선을 그립니다.
            Vector2 p0 = (i > 0) ? waypoints[i - 1] : waypoints[i];
            Vector2 p1 = waypoints[i];
            Vector2 p2 = waypoints[i + 1];
            Vector2 p3 = (i < waypoints.Count - 2) ? waypoints[i + 2] : waypoints[i + 1];

            // 현재 구간(p1 -> p2)을 100개의 점으로 나누어 부드럽게 표현
            for (int j = 0; j < 100; j++)
            {
                float t = j / 100f;
                Vector2 splinePoint = GetPointOnCatmullRomSpline(p0, p1, p2, p3, t);

                // d. 펄린 노이즈를 이용해 구불거림 추가
                float noise = (Mathf.PerlinNoise(splinePoint.x * noiseScale, splinePoint.y * noiseScale) - 0.5f) * 2f; // -1 to 1 range
                Vector2 direction = (p2 - p1).normalized;
                Vector2 perpendicular = new Vector2(direction.y, -direction.x); // 경로의 수직 방향
                Vector2 finalPoint = splinePoint + perpendicular * noise * noiseStrength;

                // e. 최종 위치에 굵기를 적용하여 원 그리기
                DrawCircle(texture, (int)finalPoint.x, (int)finalPoint.y, pathThickness, Color.white);
            }
        }
    }

    // 캣멀롬 스플라인 보간 함수
    private Vector2 GetPointOnCatmullRomSpline(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }
    
    // 텍스처에 원을 그리는 함수
    private void DrawCircle(Texture2D tex, int cx, int cy, int radius, Color color)
    {
        int r_squared = radius * radius;
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y <= r_squared)
                {
                    int drawX = cx + x;
                    int drawY = cy + y;
                    if (drawX >= 0 && drawX < tex.width && drawY >= 0 && drawY < tex.height)
                    {
                        tex.SetPixel(drawX, drawY, color);
                    }
                }
            }
        }
    }
    
    // 텍스처를 파일로 저장하는 함수
    private void SaveTextureToFile(Texture2D texture, string fileName)
    {
        byte[] bytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, fileName + ".png");
        File.WriteAllBytes(path, bytes);
        Debug.Log($"<color=green>성공적으로 이미지를 저장했습니다: {path}</color>");
        
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}