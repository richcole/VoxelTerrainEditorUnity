using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

[Serializable]
public class RJPart
{
    String name;
}

[Serializable]
public class RJAttachment
{
    String on;
    String at;
    String attach;
}

[Serializable]
public class RJCompositeDescription
{
    public List<RJPart> parts;
    public List<RJAttachment> attachments;
}

[ScriptedImporter(1, "rjc")]
class RJCompositeImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        GameObject gameObject = new GameObject("main");
        ctx.AddObjectToAsset("main", gameObject);
        ctx.SetMainObject(gameObject);
    }
}
