
// https://m.blog.naver.com/PostView.nhn?blogId=sukkow&logNo=220055044546&proxyReferer=https%3A%2F%2Fwww.google.com%2F

// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// C # manual conversion work by Yun Kyu Choi
 
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;
using System.Text;
 
enum SaveFormat { Triangles, Quads }
enum SaveResolution { Full=0, Half, Quarter, Eighth, Sixteenth }
 
class ExportTerrain : EditorWindow
{
   SaveFormat saveFormat = SaveFormat.Triangles;
   SaveResolution saveResolution = SaveResolution.Sixteenth;
 
   static TerrainData terrain;
   static Vector3 terrainPos;
 
   int tCount;
   int counter;
   int totalCount;
   int progressUpdateInterval = 10000;
 
   [MenuItem("Terrain/Export To Obj...")]
   static void Init()
   {
      terrain = null;
      Terrain terrainObject = Selection.activeObject as Terrain;
      if (!terrainObject)
      {
         terrainObject = Terrain.activeTerrain;
      }
      if (terrainObject)
      {
         terrain = terrainObject.terrainData;
         terrainPos = terrainObject.transform.position;
      }
 
      EditorWindow.GetWindow<ExportTerrain>().Show();
   }
 
   void OnGUI()
   {
      if (!terrain)
      {
         GUILayout.Label("No terrain found");
         if (GUILayout.Button("Cancel"))
         {
            EditorWindow.GetWindow<ExportTerrain>().Close();
         }
         return;
      }
      saveFormat = (SaveFormat) EditorGUILayout.EnumPopup("Export Format", saveFormat);
 
      saveResolution = (SaveResolution) EditorGUILayout.EnumPopup("Resolution", saveResolution);
 
      if (GUILayout.Button("Export"))
      {
         Export();
      }

      if (GUILayout.Button("Export And Camera Reset Capture"))
      {
         Export();

         Camera.main.transform.localPosition = Vector3.zero;
         Camera.main.transform.localRotation = Quaternion.identity;
         SJ_Menu_Misc.ScreenCapture_CaptureScreenshot();

      }
   }
 
   void Export()
   {
		DateTime t = DateTime.Now;

		string int_fmt = "D2";

		string str_name = t.Year.ToString() + "_" + t.Month.ToString(int_fmt) + "_" + t.Day.ToString(int_fmt) + "__" + t.Hour.ToString(int_fmt) + t.Minute.ToString(int_fmt) + t.Second.ToString(int_fmt);

		str_name += "_" + saveResolution.ToString();

      string fileName = EditorUtility.SaveFilePanel("Export .obj file", "", "Terrain_" + str_name , "obj");
      int w = terrain.heightmapResolution;
      int h = terrain.heightmapResolution;
      Vector3 meshScale = terrain.size;
      int tRes = (int)Mathf.Pow(2, (int)saveResolution );
      meshScale = new Vector3(meshScale.x / (w - 1) * tRes, meshScale.y, meshScale.z / (h - 1) * tRes);
      Vector2 uvScale = new Vector2(1.0f / (w - 1), 1.0f / (h - 1));
      float[,] tData = terrain.GetHeights(0, 0, w, h);
 
      w = (w - 1) / tRes + 1;
      h = (h - 1) / tRes + 1;
      Vector3[] tVertices = new Vector3[w * h];
      Vector2[] tUV = new Vector2[w * h];
 
      int[] tPolys;
 
      if (saveFormat == SaveFormat.Triangles)
      {
         tPolys = new int[(w - 1) * (h - 1) * 6];
      }
      else
      {
         tPolys = new int[(w - 1) * (h - 1) * 4];
      }
 
		// csj
		Quaternion rot_uv = Quaternion.AngleAxis(180 , new Vector3( 1 , 1, 0 ) );

      // Build vertices and UVs
      for (int y = 0; y < h; y++)
      {
         for (int x = 0; x < w; x++)
         {
            tVertices[y * w + x] = Vector3.Scale(meshScale, new Vector3(-y, tData[x * tRes, y * tRes], x)) + terrainPos;
            tUV[y * w + x] = Vector2.Scale( new Vector2(x * tRes, y * tRes), uvScale);

			//csj
			tUV[y * w + x] = rot_uv * tUV[y * w + x];
         }
      }
 
      int  index = 0;
      if (saveFormat == SaveFormat.Triangles)
      {
         // Build triangle indices: 3 indices into vertex array for each triangle
         for (int y = 0; y < h - 1; y++)
         {
            for (int x = 0; x < w - 1; x++)
            {
               // For each grid cell output two triangles
               tPolys[index++] = (y * w) + x;
               tPolys[index++] = ((y + 1) * w) + x;
               tPolys[index++] = (y * w) + x + 1;
 
               tPolys[index++] = ((y + 1) * w) + x;
               tPolys[index++] = ((y + 1) * w) + x + 1;
               tPolys[index++] = (y * w) + x + 1;
            }
         }
      }
      else
      {
         // Build quad indices: 4 indices into vertex array for each quad
         for (int y = 0; y < h - 1; y++)
         {
            for (int x = 0; x < w - 1; x++)
            {
               // For each grid cell output one quad
               tPolys[index++] = (y * w) + x;
               tPolys[index++] = ((y + 1) * w) + x;
               tPolys[index++] = ((y + 1) * w) + x + 1;
               tPolys[index++] = (y * w) + x + 1;
            }
         }
      }
 
      // Export to .obj
      StreamWriter sw = new StreamWriter(fileName);
      try
      {
 
         sw.WriteLine("# Unity terrain OBJ File");
 
         // Write vertices
         System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
         counter = tCount = 0;
         totalCount = (tVertices.Length * 2 + (saveFormat == SaveFormat.Triangles ? tPolys.Length / 3 : tPolys.Length / 4)) / progressUpdateInterval;
         for (int i = 0; i < tVertices.Length; i++)
         {
            UpdateProgress();
            StringBuilder sb = new StringBuilder("v ", 20);
            // StringBuilder stuff is done this way because it's faster than using the "{0} {1} {2}"etc. format
            // Which is important when you're exporting huge terrains.
            sb.Append(tVertices[i].x.ToString()).Append(" ").
               Append(tVertices[i].y.ToString()).Append(" ").
               Append(tVertices[i].z.ToString());
            sw.WriteLine(sb);
         }
         // Write UVs
         for (int i = 0; i < tUV.Length; i++)
         {
            UpdateProgress();
            StringBuilder sb = new StringBuilder("vt ", 22);
            sb.Append(tUV[i].x.ToString()).Append(" ").
               Append(tUV[i].y.ToString());
            sw.WriteLine(sb);
         }
         if (saveFormat == SaveFormat.Triangles)
         {
            // Write triangles
            for (int i = 0; i < tPolys.Length; i += 3)
            {
               UpdateProgress();
               StringBuilder sb = new StringBuilder("f ", 43);
               sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").
                  Append(tPolys[i + 1] + 1).Append("/").Append(tPolys[i + 1] + 1).Append(" ").
                  Append(tPolys[i + 2] + 1).Append("/").Append(tPolys[i + 2] + 1);
               sw.WriteLine(sb);
            }
         }
         else
         {
            // Write quads
            for (int i = 0; i < tPolys.Length; i += 4)
            {
               UpdateProgress();
               StringBuilder sb = new StringBuilder("f ", 57);
               sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").
                  Append(tPolys[i + 1] + 1).Append("/").Append(tPolys[i + 1] + 1).Append(" ").
                  Append(tPolys[i + 2] + 1).Append("/").Append(tPolys[i + 2] + 1).Append(" ").
                  Append(tPolys[i + 3] + 1).Append("/").Append(tPolys[i + 3] + 1);
               sw.WriteLine(sb);
            }
         }
      }
      catch(Exception err)
      {
         Debug.Log("Error saving file: " + err.Message);
      }
      sw.Close();
 
      terrain = null;
      EditorUtility.ClearProgressBar();
      EditorWindow.GetWindow<ExportTerrain>().Close();
   }
 
   void UpdateProgress()
   {
      if (counter++ == progressUpdateInterval)
      {
         counter = 0;
         EditorUtility.DisplayProgressBar("Saving...", "", Mathf.InverseLerp(0, totalCount, ++tCount));
      }
   }
}