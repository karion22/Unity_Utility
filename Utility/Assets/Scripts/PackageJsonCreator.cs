using System.Collections;
using System.Collections.Generic;
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
    private bool m_AuthorToggle = false;
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
        }

        var nameTextField = element.Q<TextField>("PackageName");
        if(nameTextField != null)
        {
            nameTextField.RegisterValueChangedCallback((value) => { m_NameValue = value.newValue; });
        }

        // Package Version
        var versionToggle = element.Q<Toggle>("PackageVersionToggle");
        if (versionToggle != null)
        {
            versionToggle.value = true;
            versionToggle.SetEnabled(false);
        }

        var versionTextField = element.Q<TextField>("PackageVersion");
        if (versionTextField != null)
        {
            versionTextField.RegisterValueChangedCallback((value) => { m_VersionValue = value.newValue; });
        }
        #endregion

        #region Recommand Elements - 왠만하면 작성 하자.

        #endregion

        #region Optional Elements - 필요할 때만 넣자
        #endregion

        rootVisualElement.Add(element);
    }
}
