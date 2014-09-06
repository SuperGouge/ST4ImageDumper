using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace JDP {
    public static class GUI {
        public static void SetFontAndScaling(Form form) {
            form.SuspendLayout();
            form.Font = new Font("Tahoma", 8.25F);
            if (form.Font.Name != "Tahoma") form.Font = new Font("Arial", 8.25F);
            form.AutoScaleMode = AutoScaleMode.Font;
            form.AutoScaleDimensions = new SizeF(6F, 13F);
            form.ResumeLayout(false);
        }

        public static void ClearImage(PictureBox pictureBox) {
            Image image = pictureBox.Image;
            if (image != null) {
                pictureBox.Image = null;
                image.Dispose();
            }
        }

        public static string LabelEscape(string str) {
            return str.Replace("&", "&&");
        }

        public static string LabelUnescape(string str) {
            return str.Replace("&&", "&");
        }

        public static void URLDropOnDragEnter(DragEventArgs e) {
            if (e.Data.GetDataPresent("UniformResourceLocatorW") ||
                e.Data.GetDataPresent("UniformResourceLocator")) 
            {
                if ((e.AllowedEffect & DragDropEffects.Copy) != 0) {
                    e.Effect = DragDropEffects.Copy;
                }
                else if ((e.AllowedEffect & DragDropEffects.Link) != 0) {
                    e.Effect = DragDropEffects.Link;
                }
            }
        }

        public static string URLDropOnDragDrop(DragEventArgs e) {
            string url = null;
            if (e.Data.GetDataPresent("UniformResourceLocatorW")) {
                byte[] data = ((MemoryStream)e.Data.GetData("UniformResourceLocatorW")).ToArray();
                url = Encoding.Unicode.GetString(data, 0, General.StrLenW(data) * 2);
            }
            else if (e.Data.GetDataPresent("UniformResourceLocator")) {
                byte[] data = ((MemoryStream)e.Data.GetData("UniformResourceLocator")).ToArray();
                url = Encoding.Default.GetString(data, 0, General.StrLen(data));
            }
            return General.CleanPageURL(url);
        }

        public static void SelectListValueOrDefault<T>(ListControl list, T? value, T defaultValue)
            where T : struct
        {
            list.SelectedValue = value ?? defaultValue;
            if (list.SelectedIndex == -1) {
                list.SelectedValue = defaultValue;
            }
        }
    }

    public class ListItemInt32 {
        public int Value { get; private set; }
        public string Text { get; private set; }

        public ListItemInt32(int value, string text) {
            Value = value;
            Text = text;
        }
    }
}