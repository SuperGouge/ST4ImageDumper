using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JDP {
    public abstract class FormData {
        public FormData() {
            FormFields = new List<FormField>();
        }

        public List<FormField> FormFields { get; private set; }

        public abstract string ContentType { get; }

        public abstract long GetContentLength();

        public abstract void WriteContent(Stream stream);
    }

    public class MultipartFormData : FormData {
        protected string _boundary;

        public MultipartFormData() :
            base() 
        {
            _boundary = DateTime.UtcNow.Ticks.ToString().PadLeft(40, '-');
        }

        public override string ContentType {
            get { return "multipart/form-data; boundary=" + _boundary; }
        }

        protected byte[] GetBoundaryBytes() {
            return Encoding.ASCII.GetBytes(_boundary);
        }

        public override long GetContentLength() {
            long contentLength = 0;
            long boundaryLength = GetBoundaryBytes().LongLength;
            foreach (FormField field in FormFields) {
                if (!field.Submit) continue;
                contentLength += boundaryLength + field.GetTotalLength() + 6L;
            }
            contentLength += boundaryLength + 6L;
            return contentLength;
        }

        public override void WriteContent(Stream stream) {
            byte[] boundary = GetBoundaryBytes();
            byte[] dashes = new byte[] { (byte)'-', (byte)'-' };
            byte[] newLine = new byte[] { (byte)'\r', (byte)'\n' };

            foreach (FormField field in FormFields) {
                if (!field.Submit) continue;

                stream.Write(dashes, 0, dashes.Length);
                stream.Write(boundary, 0, boundary.Length);
                stream.Write(newLine, 0, newLine.Length);

                byte[] header = field.GetHeader();
                stream.Write(header, 0, header.Length);

                field.WriteContent(stream);

                stream.Write(newLine, 0, newLine.Length);
            }

            stream.Write(dashes, 0, dashes.Length);
            stream.Write(boundary, 0, boundary.Length);
            stream.Write(dashes, 0, dashes.Length);
            stream.Write(newLine, 0, newLine.Length);
        }
    }

    public class URLEncodedFormData : FormData {
        public URLEncodedFormData() :
            base() { }

        public override string ContentType {
            get { return "application/x-www-form-urlencoded"; }
        }

        public override long GetContentLength() {
            using (MemoryStream ms = new MemoryStream()) {
                WriteContent(ms);
                return ms.Length;
            }
        }

        public override void WriteContent(Stream stream) {
            byte[] equals = new byte[] { (byte)'=' };
            byte[] ampersand = new byte[] { (byte)'&' };
            bool isFirst = true;

            foreach (FormField field in FormFields) {
                if (!field.Submit) continue;

                if (!isFirst) {
                    stream.Write(ampersand, 0, ampersand.Length);
                }

                byte[] name = Encoding.ASCII.GetBytes(Uri.EscapeDataString(field.Name));
                stream.Write(name, 0, name.Length);

                stream.Write(equals, 0, equals.Length);

                byte[] content = Encoding.ASCII.GetBytes(Uri.EscapeDataString(field.GetBasicContent()));
                stream.Write(content, 0, content.Length);

                isFirst = false;
            }
        }
    }

    public abstract class FormField {
        protected FormField(string name) {
            Name = name;
        }

        public string Name { get; protected set; }

        public bool Disabled { get; set; }

        public virtual bool Submit {
            get {
                return !String.IsNullOrEmpty(Name) && !Disabled;
            }
        }

        protected virtual string[] GetExtraDispositionValues() {
            return new string[0];
        }

        protected virtual MIMEHeaderField[] GetExtraHeaderFields() {
            return new MIMEHeaderField[0];
        }

        public byte[] GetHeader() {
            MIMEHeaderField contentDisposition = new MIMEHeaderField("Content-Disposition");
            contentDisposition.Values.Add("form-data");
            contentDisposition.Values.Add("name=\"" + Name + "\"");
            contentDisposition.Values.AddRange(GetExtraDispositionValues());

            List<MIMEHeaderField> headerFields = new List<MIMEHeaderField>();
            headerFields.Add(contentDisposition);
            headerFields.AddRange(GetExtraHeaderFields());

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < headerFields.Count; i++) {
                sb.Append(headerFields[i]);
                sb.Append("\r\n");
            }
            sb.Append("\r\n");

            return new UTF8Encoding(false).GetBytes(sb.ToString());
        }

        public abstract long GetContentLength();

        public abstract void WriteContent(Stream stream);

        public long GetTotalLength() {
            return GetHeader().LongLength + GetContentLength();
        }

        public abstract string GetBasicContent();
    }

    public class MIMEHeaderField {
        public string Name { get; protected set; }
        public List<string> Values { get; protected set; }

        public MIMEHeaderField(string name) {
            Name = name;
            Values = new List<string>();
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(": ");
            for (int i = 0; i < Values.Count; i++) {
                if (i != 0) sb.Append("; ");
                sb.Append(Values[i]);
            }
            return sb.ToString();
        }
    }

    public abstract class SimpleFormField : FormField {
        public SimpleFormField(string name) :
            base(name) { }

        private byte[] GetContentBytes() {
            string content = GetBasicContent();
            return String.IsNullOrEmpty(content) ? new byte[0] : new UTF8Encoding(false).GetBytes(content);
        }

        public override long GetContentLength() {
            return GetContentBytes().LongLength;
        }

        public override void WriteContent(Stream stream) {
            byte[] bytes = GetContentBytes();
            stream.Write(bytes, 0, bytes.Length);
        }
    }

    public class TextFormField : SimpleFormField {
        public TextFormField(string name) :
            base(name) { }

        public string Text { get; set; }

        public override string GetBasicContent() {
            return Text;
        }
    }

    public class BooleanFormField : SimpleFormField {
        public BooleanFormField(string name) :
            base(name) { }

        public string Value { get; set; }

        public bool Checked { get; set; }

        public override bool Submit {
            get {
                return base.Submit && Checked;
            }
        }

        public override string GetBasicContent() {
            return Value;
        }
    }

    public class FileFormField : FormField {
        public FileFormField(string name) :
            base(name) { }

        public string FilePath { get; set; }

        protected override string[] GetExtraDispositionValues() {
            return new[] { "filename=\"" + Path.GetFileName(FilePath) + "\"" };
        }

        protected override MIMEHeaderField[] GetExtraHeaderFields() {
            MIMEHeaderField contentType = new MIMEHeaderField("Content-Type");
            contentType.Values.Add("application/octet-stream");
            return new[] { contentType };
        }

        public override long GetContentLength() {
            return new FileInfo(FilePath).Length;
        }

        public override void WriteContent(Stream stream) {
            using (FileStream fileStream = File.OpenRead(FilePath)) {
                General.CopyStream(fileStream, stream);
            }
        }

        public override string GetBasicContent() {
            return Path.GetFileName(FilePath);
        }
    }
}