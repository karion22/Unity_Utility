// package 파일에 여러 옵션을 설정할 수 있도록 도와주는 스크립트

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
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

    #region Require Elements
    private bool m_NameToggle = true;
    private string m_NameValue = string.Empty;

    private bool m_VersionToggle = true;
    private string m_VersionValue = string.Empty;
    #endregion

    #region Recommand Elements
    private bool m_DescriptionToggle = true;
    private string m_Description = string.Empty;

    private bool m_DisplayToggle = true;
    private string m_DisplayName = string.Empty;

    private bool m_UnityVersionToggle = true;
    private string m_UnityVersion = string.Empty;
    #endregion

    #region Optional Elements
    private bool m_AuthorNameToggle = false;
    private bool m_AuthorEmailToggle = false;
    private bool m_AuthorURLToggle = false;
    private AuthorItem m_Author = default;

    private bool m_LogURLToggle = false;
    private string m_LogURL = string.Empty;

    private bool m_DependenciesToggle = false;
    private List<DependencyItem> m_Dependencies = new List<DependencyItem>();

    private bool m_DocumentToggle = false;
    private string m_DocumentURL = string.Empty;

    private bool m_HideInEditorToggle = false;
    private bool m_HideInEditor = true;

    private bool m_KeywordToggle = false;
    private string m_Keyword = string.Empty;

    private bool m_LicenseToggle = false;
    private string m_License = string.Empty;

    private bool m_LicenseURLToggle = false;
    private string m_LicenseURL = string.Empty;

    private bool m_SamplesToggle = false;
    private List<SampleItem> m_Samples = new List<SampleItem>();

    private bool m_TypeToggle = false;
    private string m_Type = string.Empty;

    private bool m_UnityReleaseToggle = false;
    private string m_UnityRelease = string.Empty;
    #endregion

    private void CreateGUI()
    {
        var elementAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIToolkit/UXML/UXML_PackageJsonBuilder.uxml");
        var element = elementAsset.CloneTree();

        #region Require Elements - 절대 바뀔 수 없다.
        // Package Name
        var nameToggle = element.Q<Toggle>("PackageNameToggle");
        if (nameToggle != null)
        {
            nameToggle.value = true;
            nameToggle.SetEnabled(false);

            nameToggle.RegisterValueChangedCallback((value) => { m_NameToggle = value.newValue; });
        }

        element.Q<TextField>("PackageName")?.RegisterValueChangedCallback((value) => { m_NameValue = value.newValue; });

        // Package Version
        var versionToggle = element.Q<Toggle>("PackageVersionToggle");
        if (versionToggle != null)
        {
            versionToggle.value = true;
            versionToggle.SetEnabled(false);
        }

        element.Q<TextField>("PackageVersion")?.RegisterValueChangedCallback((value) => { m_VersionValue = value.newValue; }); ;
        #endregion

        #region Recommand Elements - 왠만하면 작성 하자.
        // Description
        element.Q<Toggle>("DescriptionToggle")?.RegisterValueChangedCallback((value) => { m_DescriptionToggle = value.newValue; });
        element.Q<TextField>("Description")?.RegisterValueChangedCallback((value) => { m_Description = value.newValue; });

        // DisplayName
        element.Q<Toggle>("DisplayNameToggle")?.RegisterValueChangedCallback((value) => { m_DisplayToggle = value.newValue; });
        element.Q<TextField>("DisplayName")?.RegisterValueChangedCallback((value) => { m_DisplayName = value.newValue; });

        // UnityVersion
        element.Q<Toggle>("UnityVersionToggle")?.RegisterValueChangedCallback((value) => { m_VersionToggle = value.newValue; });
        element.Q<TextField>("UnityVersion")?.RegisterValueChangedCallback((value) => { m_VersionValue = value.newValue; });
        #endregion

        #region Optional Elements - 필요할 때만 넣자
        element.Q<Toggle>("AuthorNameToggle")?.RegisterValueChangedCallback((value) => { m_AuthorNameToggle = value.newValue; });
        element.Q<TextField>("AuthorName")?.RegisterValueChangedCallback((value) => { m_Author.Name = value.newValue; });

        element.Q<Toggle>("AuthorEmailToggle")?.RegisterValueChangedCallback((value) => { m_AuthorEmailToggle = value.newValue; });
        element.Q<TextField>("AuthorEmail")?.RegisterValueChangedCallback((value) => { m_Author.Email = value.newValue; });

        element.Q<Toggle>("AuthorURLToggle")?.RegisterValueChangedCallback((value) => { m_AuthorURLToggle = value.newValue; });
        element.Q<TextField>("AuthorURL")?.RegisterValueChangedCallback((value) => { m_Author.URL = value.newValue; });

        element.Q<Toggle>("ChangeLogURLToggle")?.RegisterValueChangedCallback((value) => { m_LogURLToggle = value.newValue; });
        element.Q<TextField>("ChangeLogURL")?.RegisterValueChangedCallback((value) => { m_LogURL = value.newValue; });

        element.Q<Toggle>("DependenciesListToggle")?.RegisterValueChangedCallback((value) => { m_DependenciesToggle = value.newValue; });
        //element.Q<TextField>("DependenciesList")?.RegisterValueChangedCallback((value) => { m_VersionValue = value.newValue; });

        element.Q<Toggle>("DocumentationURLToggle")?.RegisterValueChangedCallback((value) => { m_DocumentToggle = value.newValue; });
        element.Q<TextField>("DocumentationURL")?.RegisterValueChangedCallback((value) => { m_DocumentURL = value.newValue; });

        element.Q<Toggle>("HideInEditorToggle")?.RegisterValueChangedCallback((value) => { m_HideInEditorToggle = value.newValue; });
        element.Q<Toggle>("HideInEditor")?.RegisterValueChangedCallback((value) => { m_HideInEditor = value.newValue; });

        element.Q<Toggle>("KeywordsToggle")?.RegisterValueChangedCallback((value) => { m_KeywordToggle = value.newValue; });
        element.Q<TextField>("Keywords")?.RegisterValueChangedCallback((value) => { m_Keyword = value.newValue; });

        element.Q<Toggle>("LicenseToggle")?.RegisterValueChangedCallback((value) => { m_LicenseToggle = value.newValue; });
        element.Q<TextField>("License")?.RegisterValueChangedCallback((value) => { m_License = value.newValue; });

        element.Q<Toggle>("LicensesURLToggle")?.RegisterValueChangedCallback((value) => { m_LicenseURLToggle = value.newValue; });
        element.Q<TextField>("LicensesURL")?.RegisterValueChangedCallback((value) => { m_LicenseURL = value.newValue; });

        element.Q<Toggle>("SamplesToggle")?.RegisterValueChangedCallback((value) => { m_SamplesToggle = value.newValue; });
        //element.Q<TextField>("SampleLists")?.RegisterValueChangedCallback((value) => { m_VersionValue = value.newValue; });
        
        element.Q<Toggle>("TypeToggle")?.RegisterValueChangedCallback((value) => { m_TypeToggle = value.newValue; });
        element.Q<TextField>("Type")?.RegisterValueChangedCallback((value) => { m_Type = value.newValue; });

        element.Q<Toggle>("UnityReleaseToggle")?.RegisterValueChangedCallback((value) => { m_UnityReleaseToggle = value.newValue; });
        element.Q<TextField>("UnityRelease")?.RegisterValueChangedCallback((value) => { m_UnityRelease = value.newValue; });
        #endregion

        var applyBtn = element.Q<Button>("ApplyBtn");
        if(applyBtn != null)
        {
            applyBtn.clicked += () => {
                if(CheckRequireElements())
                {
                    Apply();
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
        return (string.IsNullOrEmpty(m_NameValue) == false && string.IsNullOrEmpty(m_VersionValue) == false);
    }

    private void Apply()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("{");
        #region Require
        if (m_NameToggle)
        {
            sb.AppendLine();
            sb.AppendFormat("\"name\": \"{0}\"", m_NameValue);
        }

        if(m_VersionToggle)
            AppendStringItem(sb, "version", m_VersionValue);
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

        #endregion

        //
        sb.AppendLine();
        sb.Append("}");
        File.WriteAllText(UnityEngine.Application.dataPath + "/Test/test.json", sb.ToString());
    }

    private void AppendStringItem(StringBuilder inBuilder, string inName, string inValue, bool bAddComma = true)
    {
        if(bAddComma) inBuilder.Append(",");

        inBuilder.AppendLine();
        inBuilder.AppendFormat("\"{0}\": \"{1}\"", inName, inValue);
    }
}
