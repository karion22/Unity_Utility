// package 파일에 여러 옵션을 설정할 수 있도록 도와주는 스크립트

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class PackageJsonCreator : EditorWindow
{
    #region Wrapper
    [System.Serializable]
    public struct sDataWrapper
    {
        public string name;
        public string version;
        public string description;
        public string displayName;
        public string unity;
        public sAuthorItemWrapper author;
        public string changelogUrl;
        public List<sDependencyItemWrapper> dependencies;
        public string documentationUrl;
        public bool hideInEditor;
        public string[] keywords;
        public string license;
        public string licensesUrl;
        public List<sSampleItemWrapper> samples;
        public string type;
        public string unityRelease;
    }

    [System.Serializable]
    public struct sAuthorItemWrapper
    {
        public string name;
        public string email;
        public string url;
    }

    [System.Serializable]
    public struct sDependencyItemWrapper
    {
        public string key;
        public string value;
    }

    [System.Serializable]
    public struct sSampleItemWrapper
    {
        public string displayName;
        public string description;
        public string path;
    }
    #endregion

    #region Sub Items
    [System.Serializable]
    public struct DependencyItem
    {
        public string Name;
        public string Version;

        public void Convert(sDependencyItemWrapper inWrapper)
        {
            Name = inWrapper.key;
            Version = inWrapper.value;
        }
    }

    [System.Serializable]
    public struct AuthorItem
    {
        public string Name;
        public string Email;
        public string URL;

        public void Convert(sAuthorItemWrapper inWrapper)
        {
            Name = inWrapper.name;
            Email = inWrapper.email;
            URL = inWrapper.url;
        }
    }

    [System.Serializable]
    public struct SampleItem
    {
        public string DisplayName;
        public string Desciption;
        public string Path;

        public void Convert(sSampleItemWrapper inWrapper)
        {
            DisplayName = inWrapper.displayName;
            Desciption = inWrapper.description;
            Path = inWrapper.path;
        }
    }
    #endregion

    [MenuItem("Expand/Package Json Editor")]
    public static void ShowWIndow()
    {
        var wnd = EditorWindow.GetWindow<PackageJsonCreator>();
        wnd.minSize = new Vector2(640f, 640f);
        wnd.Show();
    }

    #region File Elements
    private ObjectField m_FileSourceField = null;
    private UnityEngine.Object m_FileSource = null;
    private TextField m_FolderField = null;
    private string m_FolderName = string.Empty;
    private TextField m_FileField = null;
    private string m_FileName = string.Empty;
    private Toggle m_PrettyField = null;
    private bool m_PrettyPrint = true;
    #endregion

    #region Require Elements
    private Toggle m_PackageNameToggleField = null;
    private bool m_PackageNameToggle = true;
    private TextField m_PackageNameField = null;
    private string m_PackageNameValue = string.Empty;

    private Toggle m_PackageVersionToggleField = null;
    private bool m_PackageVersionToggle = true;
    private TextField m_PackageVersionField = null;
    private string m_PackageVersionValue = string.Empty;
    #endregion

    #region Recommand Elements
    private Toggle m_DescriptionToggleField = null;
    private bool m_DescriptionToggle = true;
    private TextField m_DescriptionField = null;
    private string m_Description = string.Empty;

    private Toggle m_DisplayToggleField = null;
    private bool m_DisplayToggle = true;
    private TextField m_DisplayField = null;
    private string m_DisplayName = string.Empty;

    private Toggle m_UnityVersionToggleField = null;
    private bool m_UnityVersionToggle = true;
    private TextField m_UnityVersionField = null;
    private string m_UnityVersion = string.Empty;
    #endregion

    #region Optional Elements
    private TextField m_AuthorNameField = null;
    private Toggle m_AuthorNameToggleField = null;
    private bool m_AuthorNameToggle = false;
    private TextField m_AuthorEmailField = null;
    private Toggle m_AuthorEmailToggleField = null;
    private bool m_AuthorEmailToggle = false;
    private TextField m_AuthorURLField = null;
    private Toggle m_AuthorURLToggleField = null;
    private bool m_AuthorURLToggle = false;
    private AuthorItem m_Author = default;

    private Toggle m_LogURLToggleField = null;
    private bool m_LogURLToggle = false;
    private TextField m_LogURLField = null;
    private string m_LogURL = string.Empty;

    private Toggle m_DependenciesToggleField = null;
    private bool m_DependenciesToggle = false;
    private ListView m_DependenciesField = null;
    private List<DependencyItem> m_Dependencies = new List<DependencyItem>();

    private Toggle m_DocumentURLToggleField = null;
    private bool m_DocumentURLToggle = false;
    private TextField m_DocumentURLField = null;
    private string m_DocumentURL = string.Empty;

    /*
    private Toggle m_HideInEditorToggleField = null;
    private bool m_HideInEditorToggle = false;
    private Toggle m_HideInEditorField = null;
    private bool m_HideInEditor = true;
    */

    private Toggle m_KeywordToggleField = null;
    private bool m_KeywordToggle = false;
    private TextField m_KeywordField = null;
    private string m_Keyword = string.Empty;

    private Toggle m_LicenseToggleField = null;
    private bool m_LicenseToggle = false;
    private TextField m_LicenseField = null;
    private string m_License = string.Empty;

    private Toggle m_LicenseURLToggleField = null;
    private bool m_LicenseURLToggle = false;
    private TextField m_LicenseURLField = null;
    private string m_LicenseURL = string.Empty;

    private Toggle m_SamplesToggleField = null;
    private bool m_SamplesToggle = false;
    private ListView m_SampleField = null;
    private List<SampleItem> m_Samples = new List<SampleItem>();

    private Toggle m_TypeToggleField = null;
    private bool m_TypeToggle = false;
    private TextField m_TypeField = null;
    private string m_Type = string.Empty;

    private Toggle m_UnityReleaseToggleField = null;
    private bool m_UnityReleaseToggle = false;
    private TextField m_UnityReleaseField = null;
    private string m_UnityRelease = string.Empty;
    #endregion

    private void CreateGUI()
    {
        var elementAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIToolkit/UXML/UXML_PackageJsonBuilder.uxml");
        var element = elementAsset.CloneTree();

        #region File Element - 파일 관련 처리
        m_FileSourceField = element.Q<ObjectField>("FileObjectField");
        if (m_FileSourceField != null)
        {
            m_FileSource = m_FileSourceField.value;
            m_FileSourceField.RegisterValueChangedCallback((value) => {
                m_FileSource = value.newValue;
                Load();
            });
        }

        m_FolderField = element.Q<TextField>("FolderName");
        if(m_FolderField != null )
        {
            m_FolderName = m_FolderField.text;
            m_FolderField.RegisterValueChangedCallback((value) => {
                m_FolderName = value.newValue;
            });
        }

        m_FileField = element.Q<TextField>("FileName");
        if(m_FileField != null)
        {
            m_FileName = m_FileField.text;
            m_FileField.RegisterValueChangedCallback((value) => {
                m_FileName = value.newValue;
            });
        }

        m_PrettyField = element.Q<Toggle>("PrettyPrint");
        if(m_PrettyField != null)
        {
            m_PrettyPrint = m_PrettyField.value;
            m_PrettyField.RegisterValueChangedCallback((value) => {
                m_PrettyPrint = value.newValue;
            });
        }

        var folderBtn = element.Q<Button>("FolderBtn");
        if (folderBtn != null)
        {
            folderBtn.clickable = null;
            folderBtn.clicked += () => {
                var path = EditorUtility.OpenFolderPanel("Directory", Application.dataPath, string.Empty);
                if(string.IsNullOrEmpty(path) == false)
                {
                    m_FolderField.value = path;
                }
            };
        }
        #endregion

        #region Require Elements - 절대 바뀔 수 없다.
        // Package Name
        m_PackageNameToggleField = element.Q<Toggle>("PackageNameToggle");
        if (m_PackageNameToggleField != null)
        {
            m_PackageNameToggleField.value = true;
            m_PackageNameToggleField.SetEnabled(false);

            m_PackageNameToggleField.RegisterValueChangedCallback((value) => { m_PackageNameToggle = value.newValue; });
        }

        m_PackageNameField = element.Q<TextField>("PackageName");
        if(m_PackageNameField != null)
        {
            m_PackageNameValue = m_PackageNameField.value;
            m_PackageNameField.RegisterValueChangedCallback((value) => { m_PackageNameValue = value.newValue; });
        }

        // Package Version
        m_PackageVersionToggleField = element.Q<Toggle>("PackageVersionToggle");
        if (m_PackageVersionToggleField != null)
        {
            m_PackageVersionToggleField.value = true;
            m_PackageVersionToggleField.SetEnabled(false);
        }

        m_PackageVersionField = element.Q<TextField>("PackageVersion");
        if(m_PackageVersionField != null)
        {
            m_PackageVersionValue = m_PackageVersionField.value;
            m_PackageVersionField.RegisterValueChangedCallback((value) => { m_PackageVersionValue = value.newValue; });
        }
        #endregion

        #region Recommand Elements - 왠만하면 작성 하자.
        // Description
        m_DescriptionToggleField = element.Q<Toggle>("DescriptionToggle");
        if(m_DescriptionToggleField != null)
        {
            m_DependenciesToggle = m_DescriptionToggleField.value;
            m_DescriptionToggleField.RegisterValueChangedCallback((value) => { m_DescriptionToggle = value.newValue; });
        }

        m_DescriptionField = element.Q<TextField>("Description");
        if(m_DescriptionField != null)
        {
            m_Description = m_DescriptionField.value;
            m_DescriptionField.RegisterValueChangedCallback((value) => { m_Description = value.newValue; });
        }

        // DisplayName
        m_DisplayToggleField = element.Q<Toggle>("DisplayNameToggle");
        if(m_DisplayToggleField != null)
        {
            m_DisplayToggle = m_DisplayToggleField.value;
            m_DisplayToggleField.RegisterValueChangedCallback((value) => { m_DisplayToggle = value.newValue; });
        }

        m_DisplayField = element.Q<TextField>("DisplayName");
        if(m_DisplayField != null)
        {
            m_DisplayName = m_DisplayField.value;
            m_DisplayField.RegisterValueChangedCallback((value) => { m_DisplayName = value.newValue; });
        }

        // UnityVersion
        m_UnityVersionToggleField = element.Q<Toggle>("UnityVersionToggle");
        if(m_UnityVersionToggleField != null)
        {
            m_PackageVersionToggle = m_UnityVersionToggleField.value;
            m_UnityVersionToggleField.RegisterValueChangedCallback((value) => { m_PackageVersionToggle = value.newValue; });
        }

        m_UnityVersionField = element.Q<TextField>("UnityVersion");
        if(m_UnityVersionField != null)
        {
            m_UnityVersion = m_UnityVersionField.value;
            m_UnityVersionField.RegisterValueChangedCallback((value) => { m_UnityVersion = value.newValue; });
        }

        var recommandBtn = element.Q<Button>("RecommandBtn");
        if (recommandBtn != null)
        {
            recommandBtn.clickable = null;
            recommandBtn.clicked += () => {
                m_DescriptionToggleField.value = !m_DescriptionToggleField.value;
                m_DisplayToggleField.value = !m_DisplayToggleField.value;
                m_UnityVersionToggleField.value = !m_UnityVersionToggleField.value;
            };
        }
        #endregion

        #region Optional Elements - 필요할 때만 넣자
        m_AuthorNameToggleField = element.Q<Toggle>("AuthorNameToggle");
        if(m_AuthorNameToggleField != null)
            m_AuthorNameToggleField.RegisterValueChangedCallback((value) => { m_AuthorNameToggle = value.newValue; });

        m_AuthorNameField = element.Q<TextField>("AuthorName");
        if(m_AuthorNameField != null)
        {
            m_Author.Name = m_AuthorNameField.value;
            m_AuthorNameField.RegisterValueChangedCallback((value) => { m_Author.Name = value.newValue; });
        }

        m_AuthorEmailToggleField = element.Q<Toggle>("AuthorEmailToggle");
        if(m_AuthorEmailToggleField != null)
            m_AuthorEmailToggleField.RegisterValueChangedCallback((value) => { m_AuthorEmailToggle = value.newValue; });

        m_AuthorEmailField = element.Q<TextField>("AuthorEmail");
        if(m_AuthorEmailField != null)
        {
            m_Author.Email = m_AuthorEmailField.value;
            m_AuthorEmailField.RegisterValueChangedCallback((value) => { m_Author.Email = value.newValue; });
        }

        m_AuthorURLToggleField = element.Q<Toggle>("AuthorURLToggle");
        if(m_AuthorURLToggleField != null)
            m_AuthorURLToggleField.RegisterValueChangedCallback((value) => { m_AuthorURLToggle = value.newValue; });

        m_AuthorURLField = element.Q<TextField>("AuthorURL");
        if (m_AuthorURLField != null)
        {
            m_Author.URL = m_AuthorURLField.value;
            m_AuthorURLField.RegisterValueChangedCallback((value) => { m_Author.URL = value.newValue; });
        }

        m_LogURLToggleField = element.Q<Toggle>("ChangeLogURLToggle");
        if(m_LogURLToggleField != null)
            m_LogURLToggleField.RegisterValueChangedCallback((value) => { m_LogURLToggle = value.newValue; });

        m_LogURLField = element.Q<TextField>("ChangeLogURL");
        if(m_LogURLField != null)
        {
            m_LogURL = m_LogURLField.value;
            m_LogURLField.RegisterValueChangedCallback((value) => { m_LogURL = value.newValue; });
        }

        m_DependenciesToggleField = element.Q<Toggle>("DependenciesListToggle");
        if(m_DependenciesToggleField != null)
        {
            m_DependenciesToggle = m_DependenciesToggleField.value;
            m_DependenciesToggleField.RegisterValueChangedCallback((value) => { m_DependenciesToggle = value.newValue; });
        }

        m_DependenciesField = element.Q<ListView>("DependenciesList");
        if(m_DependenciesField != null)
        {
            var item = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIToolkit/UXML/UXML_PackageJsonDependencyItem.uxml");

            m_DependenciesField.makeItem = () => item.CloneTree();
            m_DependenciesField.bindItem = (element, index) => {
                var nameField = element.Q<TextField>("NameField");
                if(nameField != null)
                {
                    nameField.value = m_Dependencies[index].Name;
                    nameField.RegisterValueChangedCallback((value) =>
                    {
                        DependencyItem item = m_Dependencies[index];
                        item.Name = value.newValue;
                        m_Dependencies[index] = item;
                    });
                }

                var versionField = element.Q<TextField>("VersionField");
                if(versionField != null)
                {
                    versionField.value = m_Dependencies[index].Version;
                    versionField.RegisterValueChangedCallback((value) =>
                    {
                        DependencyItem item = m_Dependencies[index];
                        item.Version = value.newValue;
                        m_Dependencies[index] = item;
                    });
                }
            };

            m_DependenciesField.itemsSource = m_Dependencies;
            m_DependenciesField.selectionType = SelectionType.None;
            m_DependenciesField.itemIndexChanged += ((prevValue, newValue) => { });
            m_DependenciesField.RefreshItems();
        }

        m_DocumentURLToggleField = element.Q<Toggle>("DocumentationURLToggle");
        if(m_DocumentURLToggleField != null)
        {
            m_DocumentURLToggle = m_DocumentURLToggleField.value;
            m_DocumentURLToggleField.RegisterValueChangedCallback((value) => { m_DocumentURLToggle = value.newValue; });
        }

        m_DocumentURLField = element.Q<TextField>("DocumentationURL");
        if(m_DocumentURLField != null)
        {
            m_DocumentURL = m_DocumentURLField.value;
            m_DocumentURLField.RegisterValueChangedCallback((value) => { m_DocumentURL = value.newValue; });
        }

        /*
        m_HideInEditorToggleField = element.Q<Toggle>("HideInEditorToggle");
        if(m_HideInEditorToggleField != null)
        {
            m_HideInEditorToggle = m_HideInEditorToggleField.value;
            m_HideInEditorToggleField.RegisterValueChangedCallback((value) => { m_HideInEditorToggle = value.newValue; });
        }

        m_HideInEditorField = element.Q<Toggle>("HideInEditor");
        if(m_HideInEditorField != null)
        {
            m_HideInEditor = m_HideInEditorField.value;
            m_HideInEditorField.RegisterValueChangedCallback((value) => { m_HideInEditor = value.newValue; });
        }
        */

        m_KeywordToggleField = element.Q<Toggle>("KeywordsToggle");
        if(m_KeywordToggleField != null)
        {
            m_KeywordToggle = m_KeywordToggleField.value;
            m_KeywordToggleField.RegisterValueChangedCallback((value) => { m_KeywordToggle = value.newValue; });
        }

        m_KeywordField = element.Q<TextField>("Keywords");
        if (m_KeywordField != null)
        {
            m_Keyword = m_KeywordField.value;
            m_KeywordField.RegisterValueChangedCallback((value) => { m_Keyword = value.newValue; });
        }

        m_LicenseToggleField = element.Q<Toggle>("LicenseToggle");
        if(m_LicenseToggleField != null)
        {
            m_LicenseToggle = m_LicenseToggleField.value;
            m_LicenseToggleField.RegisterValueChangedCallback((value) => { m_LicenseToggle = value.newValue; });
        }

        m_LicenseField = element.Q<TextField>("License");
        if(m_LicenseField != null)
        {
            m_License = m_LicenseField.value;
            m_LicenseField.RegisterValueChangedCallback((value) => { m_License = value.newValue; });
        }

        m_LicenseURLToggleField = element.Q<Toggle>("LicensesURLToggle");
        if(m_LicenseURLToggleField != null)
        {
            m_LicenseURLToggle = m_LicenseURLToggleField.value;
            m_LicenseURLToggleField.RegisterValueChangedCallback((value) => { m_LicenseURLToggle = value.newValue; });
        }

        m_LicenseURLField = element.Q<TextField>("LicensesURL");
        if(m_LicenseURLField != null)
        {
            m_LicenseURL = m_LicenseURLField.value;
            m_LicenseURLField.RegisterValueChangedCallback((value) => { m_LicenseURL = value.newValue; });
        }

        m_SamplesToggleField = element.Q<Toggle>("SamplesToggle");
        if(m_SamplesToggleField != null)
        {
            m_SamplesToggle = m_SamplesToggleField.value;
            m_SamplesToggleField.RegisterValueChangedCallback((value) => { m_SamplesToggle = value.newValue; });
        }

        m_SampleField = element.Q<ListView>("SampleLists");
        if (m_SampleField != null)
        {
            var treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIToolkit/UXML/UXML_PackageJsonSampleItem.uxml");

            m_SampleField.makeItem = () => treeAsset.CloneTree();
            m_SampleField.bindItem = (element, index) => {
                var displayName = element.Q<TextField>("DisplayName");
                if(displayName != null)
                {
                    displayName.value = m_Samples[index].DisplayName;
                    displayName.RegisterValueChangedCallback((value) =>
                    {
                        SampleItem item = m_Samples[index];
                        item.DisplayName = value.newValue;
                        m_Samples[index] = item;
                    });
                }

                var description = element.Q<TextField>("Description");
                if(description != null)
                {
                    description.value = m_Samples[index].Desciption;
                    description.RegisterValueChangedCallback((value) =>
                    {
                        SampleItem item = m_Samples[index];
                        item.Desciption = value.newValue;
                        m_Samples[index] = item;
                    });
                }

                var path = element.Q<TextField>("Path");
                if(path != null)
                {
                    path.value = m_Samples[index].Path;
                    path.RegisterValueChangedCallback((value) =>
                    {
                        SampleItem item = m_Samples[index];
                        item.Path = value.newValue;
                        m_Samples[index] = item;
                    });
                }
            };

            m_SampleField.itemsSource = m_Samples;
            m_SampleField.selectionType = SelectionType.None;
            m_SampleField.RefreshItems();
        }

        m_TypeToggleField = element.Q<Toggle>("TypeToggle");
        if(m_TypeToggleField != null)
        {
            m_TypeToggle = m_TypeToggleField.value;
            m_TypeToggleField.RegisterValueChangedCallback((value) => { m_TypeToggle = value.newValue; });
        }

        m_TypeField = element.Q<TextField>("Type");
        {
            m_Type = m_TypeField.value;
            m_TypeField.RegisterValueChangedCallback((value) => { m_Type = value.newValue; });
        }

        m_UnityReleaseToggleField = element.Q<Toggle>("UnityReleaseToggle");
        if(m_UnityReleaseToggleField != null)
        {
            m_UnityReleaseToggle = m_UnityReleaseToggleField.value;
            m_UnityReleaseToggleField.RegisterValueChangedCallback((value) => { m_UnityReleaseToggle = value.newValue; });
        }

        m_UnityReleaseField = element.Q<TextField>("UnityRelease");
        if(m_UnityReleaseField != null)
        {
            m_UnityRelease = m_UnityReleaseField.value;
            m_UnityReleaseField.RegisterValueChangedCallback((value) => { m_UnityRelease = value.newValue; });
        }

        var optionalBtn = element.Q<Button>("OptionalBtn");
        if (optionalBtn != null)
        {
            optionalBtn.clickable = null;
            optionalBtn.clicked += () => {
                m_AuthorNameToggleField.value = !m_AuthorNameToggleField.value;
                m_AuthorEmailToggleField.value = !m_AuthorEmailToggleField.value;
                m_AuthorURLToggleField.value = !m_AuthorURLToggleField.value;

                m_LogURLToggleField.value = !m_LogURLToggleField.value;
                m_DependenciesToggleField.value = !m_DependenciesToggleField.value;
                m_DocumentURLToggleField.value = !m_DocumentURLToggleField.value;
                //m_HideInEditorToggleField.value = !m_HideInEditorToggleField.value;
                m_KeywordToggleField.value = !m_KeywordToggleField.value;
                m_LicenseToggleField.value = !m_LicenseToggleField.value;
                m_LicenseURLToggleField.value = !m_LicenseURLToggleField.value;
                m_SamplesToggleField.value = !m_SamplesToggleField.value;
                m_TypeToggleField.value = !m_TypeToggleField.value;
                m_UnityReleaseToggleField.value = !m_UnityReleaseToggleField.value;
            };
        }
        #endregion

        var applyBtn = element.Q<Button>("ApplyBtn");
        if(applyBtn != null)
        {
            applyBtn.clicked += () => {
                if(CheckRequireElements())
                {
                    Apply();
                    EditorUtility.DisplayDialog("Confirm", "저장이 완료되었습니다", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "필수 요소가 비어있습니다.", "Confirm");
                }
            };
        }

        rootVisualElement.Add(element);
    }

    private bool CheckRequireElements()
    {
        return (string.IsNullOrEmpty(m_PackageNameValue) == false && string.IsNullOrEmpty(m_PackageVersionValue) == false);
    }

    private void Load()
    {
        if (m_FileSource != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(m_FileSource);
            string fullPath = Path.GetFullPath(assetPath);
            m_FolderField.value = Path.GetDirectoryName(fullPath);
            m_FileField.value = Path.GetFileName(fullPath);

            string json = File.ReadAllText(m_FolderField.value + "//" + m_FileField.value);
            sDataWrapper data = JsonUtility.FromJson<sDataWrapper>(json);

            #region Require Element
            m_PackageNameToggleField.value = (string.IsNullOrEmpty(data.name) == false);
            if (m_PackageNameToggleField.value)
                m_PackageNameField.value = data.name;

            m_PackageVersionToggleField.value = (string.IsNullOrEmpty(data.version) == false);
            if(m_PackageVersionToggleField.value)
                m_PackageVersionField.value = data.version;
            #endregion

            #region Recommand Element
            m_DescriptionToggleField.value = (string.IsNullOrEmpty(data.description) == false);
            if (m_DescriptionToggleField.value)
                m_DescriptionField.value = data.description;

            m_DisplayToggleField.value = (string.IsNullOrEmpty(data.displayName) == false);
            if (m_DisplayToggleField.value)
                m_DisplayField.value = data.displayName;

            m_UnityVersionToggleField.value = (string.IsNullOrEmpty(data.unity) == false);
            if (m_UnityVersionToggleField.value)
                m_UnityVersionField.value = data.unity;
            #endregion

            #region Optional Element
            m_AuthorNameToggleField.value = (string.IsNullOrEmpty(data.author.name) == false);
            if(m_AuthorNameToggleField.value)
                m_AuthorNameField.value = data.author.name;

            m_AuthorEmailToggleField.value = (string.IsNullOrEmpty(data.author.email) == false);
            if (m_AuthorEmailToggleField.value)
                m_AuthorEmailField.value = data.author.email;

            m_AuthorURLToggleField.value = (string.IsNullOrEmpty(data.author.url) == false);
            if (m_AuthorURLToggleField.value)
                m_AuthorURLField.value = data.author.url;

            m_LogURLToggleField.value = (string.IsNullOrEmpty(data.changelogUrl) == false);
            if (m_LogURLToggleField.value)
                m_LogURLField.value = data.changelogUrl;

            m_DependenciesToggleField.value = (data.dependencies.Count > 0);
            if(m_DependenciesToggleField.value)
            {
                m_Dependencies.Clear();
                m_DependenciesField.Clear();

                foreach(var item in data.dependencies)
                {
                    DependencyItem newItem = new DependencyItem();
                    newItem.Convert(item);
                    m_Dependencies.Add(newItem);
                }
                m_DependenciesField.RefreshItems();
            }

            m_DocumentURLToggleField.value = (string.IsNullOrEmpty(data.documentationUrl) == false);
            if(m_DocumentURLToggleField.value)
                m_DocumentURLField.value = data.documentationUrl;

            //m_HideInEditorToggleField.value = data.hideInEditor;
            //m_HideInEditorField.value = data.hideInEditor;

            m_KeywordToggleField.value = (data.keywords != null && data.keywords.Length > 0);
            if(m_KeywordToggleField.value)
            {
                StringBuilder keywordBuilder = new StringBuilder();
                foreach(var keyword in  data.keywords)
                {
                    if (keywordBuilder.Length > 0)
                        keywordBuilder.Append(",");
                    keywordBuilder.Append(keyword);
                }
                m_KeywordField.value = keywordBuilder.ToString();
            }

            m_LicenseToggleField.value = (string.IsNullOrEmpty(data.license) == false);
            if(m_LicenseToggleField.value)
                m_LicenseField.value = data.license;

            m_LicenseURLToggleField.value = (string.IsNullOrEmpty(data.licensesUrl) == false);
            if(m_LicenseURLToggleField.value)
                m_LicenseURLField.value= data.licensesUrl;

            m_SamplesToggleField.value = (data.samples.Count > 0);
            if(m_SamplesToggleField.value)
            {
                m_Samples.Clear();
                m_SampleField.Clear();

                foreach (var item in data.samples)
                {
                    SampleItem newItem = new SampleItem();
                    newItem.Convert(item);;
                    m_Samples.Add(newItem);
                }
                m_SampleField.RefreshItems();
            }

            m_TypeToggleField.value = (string.IsNullOrEmpty(data.type) == false);
            if(m_TypeToggleField.value)
                m_TypeField.value = data.type;

            m_UnityReleaseToggleField.value = (string.IsNullOrEmpty (data.unityRelease) == false);
            if(m_UnityReleaseToggleField.value)
                m_UnityReleaseField.value = data.unityRelease;
            #endregion
        }
        else
        {
            m_FolderField.value = string.Empty;
            m_FileField.value = string.Empty;
        }
    }

    private void Apply()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("{");
        #region Require
        if (m_PackageNameToggle)
            AppendStringItem(sb, "name", m_PackageNameValue, false);

        if(m_PackageVersionToggle)
            AppendStringItem(sb, "version", m_PackageVersionValue);
        #endregion

        #region Recommand
        if (m_DescriptionToggle)
            AppendStringItem(sb, "description", m_Description);

        if (m_DisplayToggle)
            AppendStringItem(sb, "displayName", m_DisplayName);

        if (m_UnityVersionToggle)
            AppendStringItem(sb, "unity", m_UnityVersion);
        #endregion

        #region Optional
        if(m_AuthorNameToggle || m_AuthorEmailToggle || m_AuthorURLToggle)
        {
            AppendStringBeginGroupItem(sb, "author");

            if (m_AuthorNameToggle)
                AppendStringItem(sb, "name", m_Author.Name, false);

            if(m_AuthorEmailToggle)
                AppendStringItem(sb, "email", m_Author.Email);

            if (m_AuthorURLToggle)
                AppendStringItem(sb, "url", m_Author.URL);

            AppendStringEndGroupItem(sb);
        }

        if (m_LogURLToggle)
            AppendStringItem(sb, "changelogUrl", m_LogURL);

        if(m_DependenciesToggle)
        {
            AppendStringBeginGroupItem(sb, "dependencies");

            if(m_Dependencies.Count > 0)
            {
                for(int i = 0, end =  m_Dependencies.Count; i < end; i++)
                {
                    AppendStringItem(sb, m_Dependencies[i].Name, m_Dependencies[i].Version, i != 0);
                }
            }

            AppendStringEndGroupItem(sb);
        }

        if (m_DocumentURLToggle)
            AppendStringItem(sb, "documentationUrl", m_DocumentURL);

        /*
        if (m_HideInEditorToggle)
            AppendBoolItem(sb, "hideInEditor", m_HideInEditor);
        */

        if (m_KeywordToggle)
        {
            AppendArrayBeginGroup(sb, "keywords");
            if(string.IsNullOrEmpty(m_Keyword) == false)
            {
                string[] sampleArr = m_Keyword.Split(',');

                for(int i = 0, end = sampleArr.Length; i < end; i++)
                {
                    AppendStringItem(sb, sampleArr[i], i != 0);
                }
            }
            AppendArrayEndGroup(sb);
        }

        if (m_LicenseToggle)
            AppendStringItem(sb, "license", m_License);

        if (m_LicenseURLToggle)
            AppendStringItem(sb, "licensesUrl", m_LicenseURL);

        if(m_SamplesToggle)
        {
            AppendArrayBeginGroup(sb, "samples");

            if (m_Samples.Count > 0)
            {
                for (int i = 0, end = m_Samples.Count; i < end; i++)
                {
                    AppendArrayItemBeginGroup(sb, i != 0);
                    AppendStringItem(sb, "displayName", m_Samples[i].DisplayName, false);
                    AppendStringItem(sb, "description", m_Samples[i].Desciption);
                    AppendStringItem(sb, "path", m_Samples[i].Path);
                    AppendArrayItemEndGroup(sb);
                }
            }

            AppendArrayEndGroup(sb);
        }

        if (m_TypeToggle)
            AppendStringItem(sb, "type", m_Type);

        if (m_UnityReleaseToggle)
            AppendStringItem(sb, "unityRelease", m_UnityRelease);

        #endregion
        AppendLine(sb);
        sb.Append("}");
        File.WriteAllText(string.Format("{0}\\{1}", m_FolderName, m_FileName), sb.ToString());

        AssetDatabase.Refresh();
    }

    private void AppendStringBeginGroupItem(StringBuilder inBuilder, string inName, bool bAddComma = true)
    {
        AppendComma(inBuilder, bAddComma);
        AppendLine(inBuilder);
        inBuilder.AppendFormat("\"{0}\":", inName);
        inBuilder.Append("{");
    }

    private void AppendStringEndGroupItem(StringBuilder inBuilder)
    {
        AppendLine(inBuilder);
        inBuilder.Append("}");
    }

    private void AppendArrayBeginGroup(StringBuilder inBuilder, string inName, bool bAddComma = true)
    {
        AppendComma(inBuilder, bAddComma);
        AppendLine(inBuilder);
        inBuilder.AppendFormat("\"{0}\":", inName);
        inBuilder.Append("[");
    }

    private void AppendArrayEndGroup(StringBuilder inBuilder)
    {
        AppendLine(inBuilder);
        inBuilder.Append("]");
    }

    private void AppendArrayItemBeginGroup(StringBuilder inBuilder, bool bAddComma = true)
    {
        AppendComma(inBuilder, bAddComma);
        AppendLine(inBuilder);
        inBuilder.Append("{");
    }

    private void AppendArrayItemEndGroup(StringBuilder inBuilder)
    {
        inBuilder.Append("}");
    }

    private void AppendComma(StringBuilder inBuilder, bool bAddComma)
    {
        if (bAddComma) 
            inBuilder.Append(",");
    }

    private void AppendLine(StringBuilder inBuilder)
    {
        if(m_PrettyPrint) 
            inBuilder.AppendLine();
    }

    private void AppendStringItem(StringBuilder inBuilder, string inName, string inValue, bool bAddComma = true)
    {
        AppendComma(inBuilder, bAddComma);
        AppendLine(inBuilder);
        inBuilder.AppendFormat("\"{0}\": \"{1}\"", inName, inValue);
    }

    private void AppendStringItem(StringBuilder inBuilder, string inValue, bool bAddComma = true)
    {
        AppendComma(inBuilder, bAddComma);
        AppendLine(inBuilder);
        inBuilder.AppendFormat("\"{0}\"", inValue);
    }

    private void AppendBoolItem(StringBuilder inBuilder, string inName, bool inValue, bool bAddComma = true)
    {
        AppendComma(inBuilder, bAddComma);
        AppendLine(inBuilder);
        inBuilder.AppendFormat("\"{0}\": {1}", inName, inValue);
    }
}
