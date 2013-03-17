using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;

namespace HtmlHelpers.BeginCollectionItem.Tests
{
    public static class HtmlPrefixScopeExtensionsFacts
    {
        [TestFixture]
        public class TheBeginCollectionItemMethod
        {
            [Test]
            public void WritesCollectionIndexHiddenInput_WhenThereIsNothingInRequestData()
            {
                const string collectionName = "CollectionName";
                var httpContext = new Mock<HttpContextBase>();
                var httpContextItems = new Dictionary<string, object>();
                httpContext.Setup(p => p.Items).Returns(httpContextItems);

                var httpRequest = new Mock<HttpRequestBase>();
                httpContext.Setup(p => p.Request).Returns(httpRequest.Object);

                var viewContext = new ViewContext();
                var writer = new StringWriter();
                viewContext.Writer = writer;

                var html = new HtmlHelper(viewContext, new FakeViewDataContainer());
                viewContext.HttpContext = httpContext.Object;

                using (var result = html.BeginCollectionItem(collectionName))
                {
                    Assert.That(result, Is.Not.Null);
                }

                var text = writer.ToString();
                Assert.That(text,
                            Is.Not.Null
                              .And.Not.Empty
                              .And.StartsWith(string.Format(@"<input type=""hidden"" name=""{0}.index"" autocomplete=""off"" value=""", collectionName))
                              .And.Contains(@""" />"));
            }

            [Test]
            public void WritesExpectedCollectionIndexHiddenInput_WhenThereIsAnIndexInRequestData()
            {
                const string collectionName = "CollectionName";
                var index0 = Guid.NewGuid();
                var index1 = Guid.NewGuid();
                var indexes = string.Format("{0},{1}", index0, index1);
                var httpContext = new Mock<HttpContextBase>();
                var httpContextItems = new Dictionary<string, object>();
                httpContext.Setup(p => p.Items).Returns(httpContextItems);

                var httpRequest = new Mock<HttpRequestBase>();
                httpRequest.Setup(i => i[It.Is<string>(s => s == string.Format("{0}.index", collectionName))])
                    .Returns(indexes);
                httpContext.Setup(p => p.Request).Returns(httpRequest.Object);

                var viewContext = new ViewContext();
                var writer = new StringWriter();
                viewContext.Writer = writer;

                var html = new HtmlHelper(viewContext, new FakeViewDataContainer());
                viewContext.HttpContext = httpContext.Object;

                using (var result = html.BeginCollectionItem(collectionName))
                {
                    Assert.That(result, Is.Not.Null);
                }

                var text = writer.ToString();
                Assert.That(text,
                            Is.Not.Null
                              .And.Not.Empty
                              .And.StringStarting(string.Format(@"<input type=""hidden"" name=""{0}.index"" autocomplete=""off"" value=""{1}"" />", collectionName, index0)));
            }
        }

        [TestFixture]
        public class TheBeginCollectionItemMethodOverload
        {
            [Test]
            public void WritesCollectionIndexHiddenInput_WhenThereIsNothingInRequestData()
            {
                const string collectionName = "CollectionName";
                var httpContext = new Mock<HttpContextBase>();
                var httpContextItems = new Dictionary<string, object>();
                httpContext.Setup(p => p.Items).Returns(httpContextItems);

                var httpRequest = new Mock<HttpRequestBase>();
                httpContext.Setup(p => p.Request).Returns(httpRequest.Object);

                var viewContext = new ViewContext();
                var writer = new StringWriter();
                viewContext.Writer = writer;

                var html = new HtmlHelper(viewContext, new FakeViewDataContainer());
                viewContext.HttpContext = httpContext.Object;

                using (var result = html.BeginCollectionItem(collectionName, html.ViewContext.Writer))
                {
                    Assert.That(result, Is.Not.Null);
                }

                var text = writer.ToString();
                Assert.That(text,
                            Is.Not.Null
                              .And.Not.Empty
                              .And.StringStarting(string.Format(@"<input type=""hidden"" name=""{0}.index"" autocomplete=""off"" value=""", collectionName))
                              .And.Contains(@""" />"));
            }

            [Test]
            public void WritesExpectedCollectionIndexHiddenInput_WhenThereIsAnIndexInRequestData()
            {
                const string collectionName = "CollectionName";
                var index0 = Guid.NewGuid();
                var index1 = Guid.NewGuid();
                var indexes = string.Format("{0},{1}", index0, index1);
                var httpContext = new Mock<HttpContextBase>();
                var httpContextItems = new Dictionary<string, object>();
                httpContext.Setup(p => p.Items).Returns(httpContextItems);

                var httpRequest = new Mock<HttpRequestBase>();
                httpRequest.Setup(i => i[It.Is<string>(s => s == string.Format("{0}.index", collectionName))])
                    .Returns(indexes);
                httpContext.Setup(p => p.Request).Returns(httpRequest.Object);

                var viewContext = new ViewContext();
                var writer = new StringWriter();
                viewContext.Writer = writer;

                var html = new HtmlHelper(viewContext, new FakeViewDataContainer());
                viewContext.HttpContext = httpContext.Object;

                using (var result = html.BeginCollectionItem(collectionName, html.ViewContext.Writer))
                {
                    Assert.That(result, Is.Not.Null);
                }

                var text = writer.ToString();
                Assert.That(text,
                            Is.Not.Null
                              .And.Not.Empty
                              .And.StartsWith(string.Format(@"<input type=""hidden"" name=""{0}.index"" autocomplete=""off"" value=""{1}"" />", collectionName, index0)));
            }
        }

        [TestFixture]
        public class TheBeginHtmlFieldPrefixScopeMethod
        {
            [Test]
            public void Returns_IDisposable()
            {
                var viewContext = new ViewContext();
                var html = new HtmlHelper(viewContext, new FakeViewDataContainer());

                using (var result = html.BeginHtmlFieldPrefixScope(string.Empty)
                    as HtmlPrefixScopeExtensions.HtmlFieldPrefixScope)
                {
                    Assert.That(result,
                                Is.Not.Null
                                  .And.InstanceOf<IDisposable>());
                }
            }

            [Test]
            public void Wraps_HtmlHelper_ViewData_TemplateInfo()
            {
                var viewContext = new ViewContext();
                var html = new HtmlHelper(viewContext, new FakeViewDataContainer());

                using (var result = html.BeginHtmlFieldPrefixScope(string.Empty)
                    as HtmlPrefixScopeExtensions.HtmlFieldPrefixScope)
                {
                    Assert.That(result, Is.Not.Null);
                    Assert.That(result.TemplateInfo,
                                Is.Not.Null
                                  .And.EqualTo(html.ViewData.TemplateInfo));
                }
            }

            [Test]
            public void Changes_HtmlHelper_ViewData_TemplateInfo_HtmlFieldPrefix_WhenUsed()
            {
                const string nextFieldPrefix = "InnerItems";
                const string prevFieldPrefix = "OuterItems";
                var viewContext = new ViewContext();
                var html = new HtmlHelper(viewContext, new FakeViewDataContainer());
                html.ViewData.TemplateInfo.HtmlFieldPrefix = prevFieldPrefix;

                using (var result = html.BeginHtmlFieldPrefixScope(nextFieldPrefix)
                    as HtmlPrefixScopeExtensions.HtmlFieldPrefixScope)
                {
                    Assert.That(result, Is.Not.Null);
                    Assert.That(result.PreviousHtmlFieldPrefix,
                                Is.Not.Null
                                  .And.EqualTo(prevFieldPrefix));
                    Assert.That(html.ViewData.TemplateInfo.HtmlFieldPrefix, Is.EqualTo(nextFieldPrefix));
                }
            }

            [Test]
            public void Restores_HtmlHelper_ViewData_TemplateInfo_HtmlFieldPrefix_WhenDisposed()
            {
                const string nextFieldPrefix = "InnerItems";
                const string prevFieldPrefix = "OuterItems";
                var viewContext = new ViewContext();
                var html = new HtmlHelper(viewContext, new FakeViewDataContainer());
                html.ViewData.TemplateInfo.HtmlFieldPrefix = prevFieldPrefix;

                using (var result = html.BeginHtmlFieldPrefixScope(nextFieldPrefix)
                    as HtmlPrefixScopeExtensions.HtmlFieldPrefixScope)
                {
                    Assert.That(result, Is.Not.Null);
                }
                Assert.That(html.ViewData.TemplateInfo.HtmlFieldPrefix, Is.EqualTo(prevFieldPrefix));
            }
        }

        private class FakeViewDataContainer : IViewDataContainer
        {
            private ViewDataDictionary _viewData = new ViewDataDictionary();
            public ViewDataDictionary ViewData
            {
                get { return _viewData; }
                set { _viewData = value; }
            }
        }
    }
}