using UnityEditor;
using UnityEngine;

public class ChangeChildTags : MonoBehaviour
{

    [MenuItem("GameObject/Change Children to Parent Tag")]
    public static void ChangeChildrenTags()
    {
        GameObject currentObject = Selection.activeGameObject;
        string parentTag = currentObject.tag;
        if (currentObject != null && currentObject.transform.childCount > 0)
        {
            if (EditorUtility.DisplayDialog("Change child tags to parent tag", "Do you really want to change every child tag to " + parentTag + "?", "Change tags", "Cancel"))
            {
                Transform[] transforms = Selection.GetTransforms(SelectionMode.Deep | SelectionMode.Editable);
                float numberOfTransforms = transforms.Length;
                float counter = 0.0f;
                foreach (Transform childTransform in transforms)
                {
                    counter++;
                    EditorUtility.DisplayProgressBar("Changing tags", "Changing all child object tags to " + parentTag +
                        "\n  (" + (int)counter + "/" + (int)numberOfTransforms + ")",
                        counter / numberOfTransforms);
                    childTransform.gameObject.tag = parentTag;
                }
                EditorUtility.ClearProgressBar();
            }

        }
    }
}