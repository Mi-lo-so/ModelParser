using System.Xml.Linq;
using ModelParserApp.Models;

namespace ModelParserApp.Services;

public static class LxfmlParser
{
    public static List<BrickInfo> Parse(Stream stream)
    {
        var streamedDocument = XDocument.Load(stream);

        var bricks = streamedDocument.Descendants("Brick")
            .Select(brick => new BrickInfo
            {
                Id = (string?)brick.Attribute("uuid") ?? "",
                DesignId = (string?)brick.Attribute("designID") ?? "",
                Parts = brick.Elements("Part")
                    .Select(part => new PartInfo
                    {
                        Id = (string?)part.Attribute("uuid") ?? "",
                        DesignId = (string?)part.Attribute("designID") ?? "",
                        Materials = ((string?)part.Attribute("materials")).Split(',')
                            .Select(material => new MaterialInfo
                            {
                                Id = material.Split(':')[0],
                                Layer = material.Split(':')[1]
                            })
                            .ToList(),
                        Bones = part.Elements("Bone")
                            .Select(bone => new BoneInfo
                            {
                                Id = (string?)bone.Attribute("uuid") ?? "",
                                Transformation = (string?)bone.Attribute("transformation") ?? ""
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .ToList();

        return bricks;
    }
}