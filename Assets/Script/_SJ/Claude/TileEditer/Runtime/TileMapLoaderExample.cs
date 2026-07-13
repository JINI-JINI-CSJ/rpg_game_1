using System.IO;
using UnityEngine;
using TileEditor.Core;

namespace TileEditor.Runtime
{
    /// <summary>
    /// StreamingAssets에 저장된 .tmap 파일을 런타임에 불러와 TileMapRenderer에 바인딩하는 예시.
    /// 실제 프로젝트에서는 세이브 슬롯 경로, Addressables 등으로 대체해서 사용하면 된다.
    /// </summary>
    public class TileMapLoaderExample : MonoBehaviour
    {
        public TileMapRenderer Renderer;
        public string FileNameInStreamingAssets = "NewTileMap.tmap";

        private void Start()
        {
            string path = Path.Combine(Application.streamingAssetsPath, FileNameInStreamingAssets);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"타일맵 파일이 없습니다: {path}");
                return;
            }

            TileMapData map = TileMapBinaryIO.Load(path);
            Renderer.Bind(map);
        }
    }
}
