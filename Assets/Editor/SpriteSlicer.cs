// 작성자 : 김도건
// 마지막 수정 : 2025.10.10. - Done
// 스프라이트의 전처리를 간단히 하기 위한 코드.


using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;

namespace Editor
{
    public class SpriteSlicer : AssetPostprocessor
    {
        // 자동 격자단위 Slice가 필요한 디렉토리명과 Slice의 단위크기를 아래 리스트에 삽입해주세요.
        private static readonly List<(string, int)> Targets = new() {("/units/", 32)}; 
        private void OnPreprocessTexture()
        {
            if (!assetPath.ToLower().Contains("/sprites/")) return;

            var size = -1; // Default Value for Fallback
            
            foreach (var (dir, s) in Targets)
            {
                if (assetPath.ToLower().Contains(dir)) size = s;
            }

            if (size != -1) return;
            
            var assetName = Path.GetFileNameWithoutExtension(assetPath);
            
            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.filterMode = FilterMode.Point;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            
            // SpriteGenerate에서 Physics Shape를 사용하지 않도록 함.\
            // (물리를 사용할 경우, 스프라이트에 상관없이 고유한 충돌크기를 가져야 함)
            var importerSettings = new TextureImporterSettings();
            importer.ReadTextureSettings(importerSettings);
            importerSettings.spriteGenerateFallbackPhysicsShape = false;
            importer.SetTextureSettings(importerSettings);

            importer.spritePixelsPerUnit = size;
            
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
            dataProvider.InitSpriteEditorDataProvider();
            
            var textureProvider = dataProvider.GetDataProvider<ITextureDataProvider>();

            textureProvider.GetTextureActualWidthAndHeight(out var width, out var height);
            var textureRect = new Rect(0, 0, width, height);

            var slice = new List<Rect>();
            int cx = 0, cy = 0;
            while (textureRect.Contains(new Vector2(cx, cy)))
            {
                slice.Add(new Rect(cx, cy, size, size));
                cx += size;
                if (cx >= width)
                {
                    cx = 0;
                    cy += size;
                }
            }

            var spriteRects = slice.Select((rect,
                    i) => new SpriteRect
                {
                    rect = rect,
                    pivot = new Vector2(0.5f,
                        0.5f),
                    alignment = SpriteAlignment.Custom,
                    name = $"{assetName}_{i}"
                })
                .ToList();
            
            dataProvider.SetSpriteRects(spriteRects.ToArray());
            dataProvider.Apply();

            importer.SaveAndReimport();
        }
    }
}
