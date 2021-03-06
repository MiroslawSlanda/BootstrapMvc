﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Moq;
using Xunit;

namespace BootstrapMvc.Core
{
    public class ContentElementTests
    {
        private MockRepository mocks;
        private Mock<IBootstrapContext> contextMock;

        private Stack<object> stack;

        public ContentElementTests()
        {
            stack = new Stack<object>();

            mocks = new MockRepository(MockBehavior.Strict);
            contextMock = mocks.Create<IBootstrapContext>();
            contextMock.Setup(x => x.HtmlEncode(It.IsAny<string>())).Returns((string s) => WebUtility.HtmlEncode(s));
            contextMock.Setup(x => x.Push(It.IsAny<object>())).Callback((object x) => stack.Push(x));
            contextMock.Setup(x => x.PopIfEqual(It.IsAny<object>())).Callback((object x) => stack.Pop());
        }

        [Fact]
        public void Test_Writes_Ok()
        {
            using (var sw = new StringWriter())
            {
                contextMock.SetupGet(x => x.Writer).Returns(sw);
                using (var cnt = new DummyContentElement().BeginContent(sw, contextMock.Object))
                {
                    sw.Write("-value-");
                }
                Assert.Equal("start-value-end", sw.ToString());
            }
        }

        private class DummyDisposableContext : DisposableContent
        {
            public DummyDisposableContext(IBootstrapContext context)
            {
                // Nothing
            }
        }

        private class DummyContentElement : ContentElement<DummyDisposableContext>
        {
            protected override DummyDisposableContext CreateContentContext(IBootstrapContext context)
            {
                return new DummyDisposableContext(context);
            }

            protected override void WriteSelfStart(TextWriter writer, IBootstrapContext context)
            {
                writer.Write("start");
            }

            protected override void WriteSelfEnd(TextWriter writer, IBootstrapContext context)
            {
                writer.Write("end");
            }
        }
    }
}
