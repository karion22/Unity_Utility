// package 파일에 여러 옵션을 설정할 수 있도록 도와주는 스크립트

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class PackageJsonCreator : EditorWindow
{
    #region Sub Items
    [System.Serializable]
    public struct DependencyItem
    {
        public string Name;
        public string Version;
    }

    [System.Serializable]
    public struct AuthorItem
    {
        public string Name;
        public string Email;
        public string URL;
    }

    [System.Serializable]
    public struct SampleItem
    {
        public string DisplayName;
        public string Desciption;
        public string Path;
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
    private Object m_FileSource = null;
    private TextField m_FolderField = null;
    private string m_FolderName = string.Empty;
    private TextField m_FileField = null;
    private string m_FileName = string.Empty;
    private Toggle m_PrettyField = null;
    private bool m_PrettyPrint = true;
    #endregion

    #region Require Elements
    private bool m_PackageNameToggle = true;
    private TextField m_PackageNameField = null;
    private string m_PackageNameValue = string.Empty;

    private bool m_PackageVersionToggle = true;
    private TextField m_PackageVersionField = null;
    private string m_PackageVersionValue = string.Empty;
    #endregion

    #region Recommand Elements
    private bool m_DescriptionToggle = true;
    private TextField m_DescriptionField = null;
    private string m_Description = string.Empty;

    private bool m_DisplayToggle = true;
    private TextField m_DisplayField = null;
    private string m_DisplayName = string.Empty;

    private bool m_UnityVersionToggle = true;
    private TextField m_UnityVersionField = null;
    private string m_UnityVersion = string.Empty;
    #endregion

    #region Optional Elements
    private TextField m_AuthorNameField = null;
    private bool m_AuthorNameToggle = false;
    private TextField m_AuthorEmailField = null;
    private bool m_AuthorEmailToggle = false;
    private TextField m_AuthorURLField = null;
    private bool m_AuthorURLToggle = false;
    private AuthorItem m_Author = default;

    private bool m_LogURLToggle = false;
    private TextField m_LogField = null;
    private string m_LogURL = string.Empty;

    private bool m_DependenciesToggle = false;
    private ListView m_DependenciesField = null;
    private List<DependencyItem> m_Dependencies = new List<DependencyItem>();

    private bool m_DocumentToggle = false;
    private TextField m_DocumentField = null;
    private string m_DocumentURL = string.Empty;

    private bool m_HideInEditorToggle = false;
    private Toggle m_HideInEditorField = null;
    private bool m_HideInEditor = true;

    private bool m_KeywordToggle = false;
    private TextField m_KeywordField = null;
    private string m_Keyword = string.Empty;

    private bool m_LicenseToggle = false;
    private TextField m_LicenseField = null;
    private string m_License = string.Empty;

    private bool m_LicenseURLToggle = false;
    private TextField m_LicenseURLField = null;
    private string m_LicenseURL = string.Empty;

    private bool m_SamplesToggle = false;
    private ListView m_SampleField = null;
    private List<SampleItem> m_Samples = new List<SampleItem>();

    private bool m_TypeToggle = false;
    private TextField m_TypeField = null;
    private string m_Type = string.Empty;

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
        var nameToggle = element.Q<Toggle>("PackageNameToggle");
        if (nameToggle != null)
        {
            nameToggle.value = true;
            nameToggle.SetEnabled(false);

            nameToggle.RegisterValueChangedCallback((value) => { m_PackageNameToggle = value.newValue; });
        }

        m_PackageNameField = element.Q<TextField>("PackageName");
        if(m_PackageNameField != null)
        {
            m_PackageNameValue = m_PackageNameField.value;
            m_PackageNameField.RegisterValueChangedCallback((value) => { m_PackageNameValue = value.newValue; });
        }

        // Package Version
        var packageVersionToggle = element.Q<Toggle>("PackageVersionToggle");
        if (packageVersionToggle != null)
        {
            packageVersionToggle.value = true;
            packageVersionToggle.SetEnabled(false);
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
        var descriptionToggle = element.Q<Toggle>("DescriptionToggle");
        if(descriptionToggle != null)
        {
            m_DependenciesToggle = descriptionToggle.value;
            descriptionToggle.RegisterValueChangedCallback((value) => { m_DescriptionToggle = value.newValue; });
        }

        m_DescriptionField = element.Q<TextField>("Description");
        if(m_DescriptionField != null)
        {
            m_Description = m_DescriptionField.value;
            m_DescriptionField.RegisterValueChangedCallback((value) => { m_Description = value.newValue; });
        }

        // DisplayName
        var displayNameToggle = element.Q<Toggle>("DisplayNameToggle");
        if(displayNameToggle != null)
        {
            m_DisplayToggle = displayNameToggle.value;
            displayNameToggle.RegisterValueChangedCallback((value) => { m_DisplayToggle = value.newValue; });
        }

        m_DisplayField = element.Q<TextField>("DisplayName");
        if(m_DisplayField != null)
        {
            m_DisplayName = m_DisplayField.value;
            m_DisplayField.RegisterValueChangedCallback((value) => { m_DisplayName = value.newValue; });
        }

        // UnityVersion
        var unityVersionToggle = element.Q<Toggle>("UnityVersionToggle");
        if(unityVersionToggle != null)
        {
            m_PackageVersionToggle = unityVersionToggle.value;
            unityVersionToggle.RegisterValueChangedCallback((value) => { m_PackageVersionToggle = value.newValue; });
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
                descriptionToggle.value = !descriptionToggle.value;
                displayNameToggle.value = !displayNameToggle.value;
                unityVersionToggle.value = !unityVersionToggle.value;
            };
        }
        #endregion

        #region Optional Elements - 필요할 때만 넣자
        var authorNameToggle = element.Q<Toggle>("AuthorNameToggle");
        if(authorNameToggle != null)
            authorNameToggle.RegisterValueChangedCallback((value) => { m_AuthorNameToggle = value.newValue; });

        m_AuthorNameField = element.Q<TextField>("AuthorName");
        if(m_AuthorNameField != null)
        {
            m_Author.Name = m_AuthorNameField.value;
            m_AuthorNameField.RegisterValueChangedCallback((value) => { m_Author.Name = value.newValue; });
        }

        var authorEmailToggle = element.Q<Toggle>("AuthorEmailToggle");
        if(authorEmailToggle != null)
            authorEmailToggle.RegisterValueChangedCallback((value) => { m_AuthorEmailToggle = value.newValue; });

        m_AuthorEmailField = element.Q<TextField>("AuthorEmail");
        if(m_AuthorEmailField != null)
        {
            m_Author.Email = m_AuthorEmailField.value;
            m_AuthorEmailField.RegisterValueChangedCallback((value) => { m_Author.Email = value.newValue; });
        }

        var authorURLToggle = element.Q<Toggle>("AuthorURLToggle");
        if(authorURLToggle != null)
            authorURLToggle.RegisterValueChangedCallback((value) => { m_AuthorURLToggle = value.newValue; });

        m_AuthorURLField = element.Q<TextField>("AuthorURL");
        if (m_AuthorURLField != null)
        {
            m_Author.URL = m_AuthorURLField.value;
            m_AuthorURLField.RegisterValueChangedCallback((value) => { m_Author.URL = value.newValue; });
        }

        var logURLToggle = element.Q<Toggle>("ChangeLogURLToggle");
        if(logURLToggle != null)
            logURLToggle.RegisterValueChangedCallback((value) => { m_LogURLToggle = value.newValue; });

        m_LogField = element.Q<TextField>("ChangeLogURL");
        if(m_LogField != null)
        {
            m_LogURL = m_LogField.value;
            m_LogField.RegisterValueChangedCallback((value) => { m_LogURL = value.newValue; });
        }

        var dependenciesToggle = element.Q<Toggle>("DependenciesListToggle");
        if(dependenciesToggle != null)
        {
            m_DependenciesToggle = dependenciesToggle.value;
            dependenciesToggle.RegisterValueChangedCallback((value) => { m_DependenciesToggle = value.newValue; });
        }

        m_DependenciesField = element.Q<ListView>("DependenciesList");
        if(m_DependenciesField != null)
        {
            var item = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIToolkit/UXML/UXML_PackageJsonDependencyItem.uxml");

            m_DependenciesField.makeItem = () => item.CloneTree();
            m_DependenciesField.bindItem = (element, index) => {
                element.Q<TextField>("NameField").RegisterValueChangedCallback((value) => 
                {
                    DependencyItem item = m_Dependencies[index];
                    item.Name = value.newValue;
                    m_Dependencies[index] = item;
                });

                element.Q<TextField>("VersionField").RegisterValueChangedCallback((value) =>
                {
                    DependencyItem item = m_Dependencies[index];
                    item.Version = value.newValue;
                    m_Dependencies[index] = item;
                });
            };

            m_DependenciesField.itemsSource = m_Dependencies;
            m_DependenciesField.selectionType = SelectionType.None;
            m_DependenciesField.itemIndexChanged += ((prevValue, newValue) => { });
            m_DependenciesField.RefreshItems();
        }

        var documentURLToggle = element.Q<Toggle>("DocumentationURLToggle");
        if(documentURLToggle != null)
        {
            m_DocumentToggle = documentURLToggle.value;
            documentURLToggle.RegisterValueChangedCallback((value) => { m_DocumentToggle = value.newValue; });
        }

        m_DocumentField = element.Q<TextField>("DocumentationURL");
        if(m_DocumentField != null)
        {
            m_DocumentURL = m_DocumentField.value;
            m_DocumentField.RegisterValueChangedCallback((value) => { m_DocumentURL = value.newValue; });
        }        

        var hideInEditorToggle = element.Q<Toggle>("HideInEditorToggle");
        if(hideInEditorToggle != null)
        {
            m_HideInEditorToggle = hideInEditorToggle.value;
            hideInEditorToggle.RegisterValueChangedCallback((value) => { m_HideInEditorToggle = value.newValue; });
        }

        m_HideInEditorField = element.Q<Toggle>("HideInEditor");
        if(m_HideInEditorField != null)
        {
            m_HideInEditor = m_HideInEditorField.value;
            m_HideInEditorField.RegisterValueChangedCallback((value) => { m_HideInEditor = value.newValue; });
        }

        var keywordToggle = element.Q<Toggle>("KeywordsToggle");
        if(keywordToggle != null)
        {
            m_KeywordToggle = keywordToggle.value;
            keywordToggle.RegisterValueChangedCallback((value) => { m_KeywordToggle = value.newValue; });
        }

        m_KeywordField = element.Q<TextField>("Keywords");
        if (m_KeywordField != null)
        {
            m_Keyword = m_KeywordField.value;
            m_KeywordField.RegisterValueChangedCallback((value) => { m_Keyword = value.newValue; });
        }        

        var licenseToggle = element.Q<Toggle>("LicenseToggle");
        if(licenseToggle != null)
        {
            m_LicenseToggle = licenseToggle.value;
            licenseToggle.RegisterValueChangedCallback((value) => { m_LicenseToggle = value.newValue; });
        }

        m_LicenseField = element.Q<TextField>("License");
        if(m_LicenseField != null)
        {
            m_License = m_LicenseField.value;
            m_LicenseField.RegisterValueChangedCallback((value) => { m_License = value.newValue; });
        }        

        var licenseURLToggle = element.Q<Toggle>("LicensesURLToggle");
        if(licenseURLToggle != null)
        {
            m_LicenseURLToggle = licenseURLToggle.value;
            licenseURLToggle.RegisterValueChangedCallback((value) => { m_LicenseURLToggle = value.newValue; });
        }

        m_LicenseURLField = element.Q<TextField>("LicensesURL");
        if(m_LicenseURLField != null)
        {
            m_LicenseURL = m_LicenseURLField.value;
            m_LicenseURLField.RegisterValueChangedCallback((value) => { m_LicenseURL = value.newValue; });
        }

        var sampleToggle = element.Q<Toggle>("SamplesToggle");
        if(sampleToggle != null)
        {
            m_SamplesToggle = sampleToggle.value;
            sampleToggle.RegisterValueChangedCallback((value) => { m_SamplesToggle = value.newValue; });
        }

        m_SampleField = element.Q<ListView>("SampleLists");
        if (m_SampleField != null)
        {
            var item = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIToolkit/UXML/UXML_PackageJsonSampleItem.uxml");

            m_SampleField.makeItem = () => item.CloneTree();
            m_SampleField.bindItem = (element, index) => {
                element.Q<TextField>("DisplayName").RegisterValueChangedCallback((value) =>
                {
                    SampleItem item = m_Samples[index];
                    item.DisplayName = value.newValue;
                    m_Samples[index] = item;
                });

                element.Q<TextField>("Description").RegisterValueChangedCallback((value) =>
                {
                    SampleItem item = m_Samples[index];
                    item.Desciption = value.newValue;
                    m_Samples[index] = item;
                });

                element.Q<TextField>("Path").RegisterValueChangedCallback((value) =>
                {
                    SampleItem item = m_Samples[index];
                    item.Path = value.newValue;
                    m_Samples[index] = item;
                });

            };

            m_SampleField.itemsSource = m_Samples;
            m_SampleField.selectionType = SelectionType.None;
            m_SampleField.itemIndexChanged += ((prevValue, newValue) => { });
            m_SampleField.RefreshItems();
        }

        var typeToggle = element.Q<Toggle>("TypeToggle");
        if(typeToggle != null)
        {
            m_TypeToggle = typeToggle.value;
            typeToggle.RegisterValueChangedCallback((value) => { m_TypeToggle = value.newValue; });
        }

        m_TypeField = element.Q<TextField>("Type");
        {
            m_Type = m_TypeField.value;
            m_TypeField.RegisterValueChangedCallback((value) => { m_Type = value.newValue; });
        }

        var unityReleaseToggle = element.Q<Toggle>("UnityReleaseToggle");
        if(unityReleaseToggle != null)
        {
            m_UnityReleaseToggle = unityReleaseToggle.value;
            unityReleaseToggle.RegisterValueChangedCallback((value) => { m_UnityReleaseToggle = value.newValue; });
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
                authorNameToggle.value = !authorNameToggle.value;
                authorEmailToggle.value = !authorEmailToggle.value;
                authorURLToggle.value = !authorURLToggle.value;

                logURLToggle.value = !logURLToggle.value;
                dependenciesToggle.value = !dependenciesToggle.value;
                documentURLToggle.value = !documentURLToggle.value;
                hideInEditorToggle.value = !hideInEditorToggle.value;
                keywordToggle.value = !keywordToggle.value;
                licenseToggle.value = !licenseToggle.value;
                licenseURLToggle.value = !licenseURLToggle.value;
                sampleToggle.value = !sampleToggle.value;
                typeToggle.value = !typeToggle.value;
                unityReleaseToggle.value = !unityReleaseToggle.value;
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

        if (m_DocumentToggle)
            AppendStringItem(sb, "documentationUrl", m_DocumentURL);

        if (m_HideInEditorToggle)
            AppendBoolItem(sb, "hideInEditor", m_HideInEditor);

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
        inBuilder.Append("{");

        AppendLine(inBuilder);
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
