﻿using System.Xml;
using System.Xml.Serialization;
using OpenRasta.Codecs;
using Ramone.Tests.Common.CMS;
using Ramone.Tests.Server.Codecs;


namespace Ramone.Tests.Server.CMS.Codecs
{
  [MediaType(CMSConstants.CMSMediaTypeId)]
  [MediaType("application/xml")]
  public class DossierDocumentsCodec : XmlCodecBase<DossierDocumentList>
  {
    XmlSerializer Serializer = new XmlSerializer(typeof(DossierDocumentList));


    protected override void WriteTo(DossierDocumentList docs, XmlWriter writer)
    {
      Serializer.Serialize(writer, docs);
    }


    protected override DossierDocumentList ReadFrom(XmlReader reader)
    {
      return (DossierDocumentList)Serializer.Deserialize(reader);
    }
  }
}