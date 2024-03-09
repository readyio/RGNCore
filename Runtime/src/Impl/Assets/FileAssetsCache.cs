using System;
using System.Collections.Generic;
using System.IO;
using RGN.ImplDependencies.Assets;
using UnityEngine;

namespace RGN.Impl.Firebase.Assets
{
    public class FileAssetsCache : IAssetCache
    {
        private string _basePath = Application.persistentDataPath;
        private Dictionary<AssetCategory, string> _categoryPaths = new Dictionary<AssetCategory, string>();

        public FileAssetsCache()
        {
            PrepareCategoryDirectories();
            PrepareCategoryPaths();
        }

        public bool HasInCache(AssetCategory category, string key)
        {
            CreateCategoryDirectoryIfThereNo(category);
            
            string assetPath = GetAssetPath(category, key);
            return File.Exists(assetPath);
        }

        public byte[] ReadFromCache(AssetCategory category, string key)
        {
            CreateCategoryDirectoryIfThereNo(category);
            
            string assetPath = GetAssetPath(category, key);
            byte[] assetBytes = File.ReadAllBytes(assetPath);
            return assetBytes;
        }

        public bool TryReadFromCache(AssetCategory category, string key, out byte[] data)
        {
            CreateCategoryDirectoryIfThereNo(category);

            if (!HasInCache(category, key))
            {
                data = Array.Empty<byte>();
                return false;
            }

            data = ReadFromCache(category, key);
            return true;
        }

        public void WriteToCache(AssetCategory category, string key, byte[] data)
        {
            CreateCategoryDirectoryIfThereNo(category);

            string assetPath = GetAssetPath(category, key);
            File.WriteAllBytes(assetPath, data);
        }

        public void Clear()
        {
            foreach (AssetCategory category in (AssetCategory[])Enum.GetValues(typeof(AssetCategory)))
            {
                ClearCategoryDirectory(category);
            }
        }

        private void PrepareCategoryDirectories()
        {
            foreach (AssetCategory assetCategory in (AssetCategory[])Enum.GetValues(typeof(AssetCategory)))
            {
                CreateCategoryDirectoryIfThereNo(assetCategory);
            }
        }

        private void PrepareCategoryPaths()
        {
            foreach (AssetCategory assetCategory in (AssetCategory[])Enum.GetValues(typeof(AssetCategory)))
            {
                _categoryPaths[assetCategory] = Path.Combine(_basePath, assetCategory.ToString());
            }
        }

        private void CreateCategoryDirectoryIfThereNo(AssetCategory category)
        {
            string categoryPath = Path.Combine(_basePath, category.ToString());
            
            if (!Directory.Exists(categoryPath))
            {
                Directory.CreateDirectory(categoryPath);
            }
        }
        
        private void ClearCategoryDirectory(AssetCategory category)
        {
            string categoryPath = GetCategoryPath(category);
            
            if (Directory.Exists(categoryPath))
            {
                DirectoryInfo assetsDirectory = new DirectoryInfo(categoryPath);
                foreach (FileInfo assetFile in assetsDirectory.GetFiles())
                {
                    assetFile.Delete();
                }
            }
        }

        private string GetCategoryPath(AssetCategory category)
            => _categoryPaths[category];
        
        private string GetAssetPath(AssetCategory category, string key)
            => Path.Combine(GetCategoryPath(category), key);
    }
}
