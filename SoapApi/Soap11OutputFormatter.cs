﻿using System.Xml;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Channels;
using Microsoft.AspNet.Mvc.Internal;
using Microsoft.AspNet.Mvc.Formatters;

namespace SoapApi
{
    public class Soap11OutputFormatter : OutputFormatter
    {
        public Soap11OutputFormatter()
        {
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);

            SupportedMediaTypes.Add(KnownMediaTypes.TextXmlSoap11);

            WriterSettings = new XmlWriterSettings();
        }

        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return true;
            //return context.HttpContext.Request.Headers.ContainsKey("SOAPAction");
        }

        public XmlWriterSettings WriterSettings { get; }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var writerSettings = WriterSettings.Clone();
            var encoding = context.ContentType?.Encoding ?? Encoding.UTF8;
            var action = context.HttpContext.Request.Headers["SOAPAction"];

            using (var textWriter = context.WriterFactory(context.HttpContext.Response.Body, encoding))
            {
                var writer = XmlWriter.Create(textWriter, new XmlWriterSettings());

                var msg = Message.CreateMessage(MessageVersion.Soap11, action, writer);

                msg.WriteMessage(writer);

                writer.Flush();
            }

            return TaskCache.CompletedTask;
        }
    }
}
