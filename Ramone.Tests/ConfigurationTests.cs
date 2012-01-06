﻿using System.ServiceModel.Syndication;
using System.Xml;
using NUnit.Framework;
using Ramone.MediaTypes.Hal;


namespace Ramone.Tests
{
  [TestFixture]
  public class ConfigurationTests : TestHelper
  {
    [Test]
    public void CanAddStandardCodecs()
    {
      // Act
      IRamoneService service = RamoneConfiguration.NewService(BaseUrl)
                                                  .WithStandardCodecs();

      // Assert
      Assert.IsNotNull(service.CodecManager.GetReader(typeof(XmlDocument), "application/xml"));
      Assert.IsNotNull(service.CodecManager.GetReader(typeof(object), "application/json"));
      Assert.IsNotNull(service.CodecManager.GetReader(typeof(SyndicationItem), "application/atom+xml"));
      Assert.IsNotNull(service.CodecManager.GetReader(typeof(SyndicationFeed), "application/atom+xml"));
    }
  }
}
